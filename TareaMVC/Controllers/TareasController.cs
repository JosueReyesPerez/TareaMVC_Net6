using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareaMVC.Entities;
using TareaMVC.Services;

namespace TareaMVC.Controllers
{
    [Route("api/tareas")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly AplicationDbContext _context;
        private readonly IServicioUsuarios _usuarios;

        public TareasController(AplicationDbContext context, IServicioUsuarios usuarios)
        {
            _context = context;
            this._usuarios = usuarios;
        }

        


        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = _usuarios.ObtenerUsuarioId();
            var existenTareas = await _context.Tareas.AnyAsync(t => t.IdentityUserId == usuarioId);
            var ordenMayor = 0;
            if (existenTareas)
                ordenMayor = await _context.Tareas.Where(t => t.IdentityUserId == usuarioId).Select(i => i.Orden).MaxAsync();

            Tarea tarea = new Tarea()
            {
                Titulo = titulo,
                IdentityUserId = usuarioId,
                FechaCreacion = DateTime.Now,
                Orden = ordenMayor + 1
            };

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return tarea;
        }

    }
}
