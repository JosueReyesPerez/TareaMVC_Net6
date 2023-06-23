using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareaMVC.Entities;
using TareaMVC.Models;
using TareaMVC.Services;

namespace TareaMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasosController : ControllerBase
    {
        private readonly AplicationDbContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        public PasosController(AplicationDbContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }


        // POST: api/Pasos
        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<Paso>> Post(int tareaId, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId && t.IdentityUserId == usuarioId);

            if (tarea is null)
                return NotFound();

            var existenPasos = await _context.Pasos.AnyAsync(p => p.TareaId == tareaId);
            int ordenPaso = 0;
            if (existenPasos)
                ordenPaso = await _context.Pasos.Where(p => p.TareaId == tareaId).Select(p => p.Orden).MaxAsync();

            Paso paso = new()
            {
                TareaId = tarea.Id,
                Orden = (ordenPaso + 1),
                Descripcion = pasoCrearDTO.Descripcion,
                Realizado = pasoCrearDTO.Realizado
            };

            _context.Pasos.Add(paso);
            await _context.SaveChangesAsync();

            return paso;
        }

       
        // PUT: api/Pasos
        [HttpPut("{Id}")]
        public async Task<ActionResult> Put(Guid Id, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var userId = _servicioUsuarios.ObtenerUsuarioId();
            var paso = await _context.Pasos.Include(p => p.Tarea) //Para incluir la propiedad de navegacin de tarea
                                .FirstOrDefaultAsync(p => p.Id == Id);

            if (paso is null || paso.Tarea.IdentityUserId != userId)
                return NotFound();

            paso.Descripcion = pasoCrearDTO.Descripcion;
            paso.Realizado = pasoCrearDTO.Realizado;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var paso = await _context.Pasos.Include(p => p.Tarea).FirstOrDefaultAsync(t => t.Id == id);

            if (paso is null || paso.Tarea.IdentityUserId != usuarioId)
                return NotFound();

            _context.Pasos.Remove(paso);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("ordenar/{tareaId:int}")]
        public async Task<ActionResult> Ordenar(int tareaId, [FromBody] Guid[] ids)
        {
            var userId = _servicioUsuarios.ObtenerUsuarioId();
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId && t.IdentityUserId == userId);

            if (tarea is null)
                return NotFound();

            var pasos = await _context.Pasos.Where(x => x.TareaId == tareaId).ToListAsync();

            var pasosIds = pasos.Select(x => x.Id);

            var idsPasosNoPertenecenALaTarea = ids.Except(pasosIds).ToList();

            if (idsPasosNoPertenecenALaTarea.Any())
                return BadRequest("No todos los pasos estan presentes...");

            var pasosDiccionario = pasos.ToDictionary(p => p.Id);

            for(int i = 0; i<ids.Length; i++)
            {
                var pasoId = ids[i];
                var paso = pasosDiccionario[pasoId];
                paso.Orden = i + 1;

            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
