using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareaMVC.Entities;
using TareaMVC.Models;
using TareaMVC.Services;

namespace TareaMVC.Controllers
{
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly AplicationDbContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IMapper _mapper;
        public TareasController(AplicationDbContext context, IServicioUsuarios servicioUsuarios, IMapper mapper)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userid = _servicioUsuarios.ObtenerUsuarioId();

            //Para ordenar OrderBy asendente OrderByDescending para desendente
            var tareas = await _context.Tareas
                   .Where(t => t.IdentityUserId == userid)
                       .OrderBy(t => t.Orden) 
                           .ProjectTo<TareaDTO>(_mapper.ConfigurationProvider) //Mapeo con AutoMapper
                           .ToListAsync();
            /*.Select(t => new TareaDTO(){ //Tambien podriamos retornar un objeto anonimo
                               Id = t.Id, 
                               Titulo = t.Titulo
                           })*/
            return Ok(tareas); //regresamos una lista de objetos anonimos.
            //return BadRequest("Error....");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tarea>> Get(int id)
        {
            var userId = _servicioUsuarios.ObtenerUsuarioId();
            var tarea = await _context.Tareas
                .Include(t => t.pasos.OrderBy(p => p.Orden)) //Para cargar los pasos desde las propuedades de navegacion
                .FirstOrDefaultAsync(t => t.IdentityUserId == userId && t.Id == id);
            return (tarea is null) ? NotFound() : tarea;
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var existenTareas = await _context.Tareas.AnyAsync(t => t.IdentityUserId == usuarioId);
            var ordenMayor = 0;
            if (existenTareas)
            {
                ordenMayor = await _context.Tareas.Where(t => t.IdentityUserId == usuarioId)
                    .Select(t => t.Orden).MaxAsync();
            }

            var tarea = new Tarea()
            {
                Titulo = titulo, 
                IdentityUserId = usuarioId, 
                FechaCreacion = DateTime.UtcNow, 
                Orden = ordenMayor + 1
            };

            _context.Add(tarea);
            await _context.SaveChangesAsync();
            
            return tarea;
        }

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tareasDiccionario = await _context.Tareas.Where(t => t.IdentityUserId == usuarioId).ToDictionaryAsync( t => t.Id);

            int orden = 0;
            foreach(var id in ids)
            {
                Console.WriteLine(id);
                var tarea = tareasDiccionario[id];
                tarea.Orden = (orden += 1);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("");
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTarea(int id, [FromBody] TareaEditarDTO tareaEditarDTO)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == id && t.IdentityUserId == usuarioId);

            if (tarea is null)
                return NotFound();

            tarea.Titulo = tareaEditarDTO.Titulo;
            tarea.Descripcion = tareaEditarDTO.Descripcion;

            await _context.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = _servicioUsuarios.ObtenerUsuarioId();

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == id && t.IdentityUserId == userId);

            if (tarea is null)
                return NotFound();

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
