using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareaMVC;
using TareaMVC.Entities;

namespace TareaMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleTareasController : ControllerBase
    {
        private readonly AplicationDbContext _context;

        public ExampleTareasController(AplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ExampleTareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> GetTareas()
        {
          if (_context.Tareas == null)
          {
              return NotFound();
          }
            return await _context.Tareas.ToListAsync();
        }

        // GET: api/ExampleTareas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarea>> GetTarea(int id)
        {
          if (_context.Tareas == null)
          {
              return NotFound();
          }
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            return tarea;
        }

        // PUT: api/ExampleTareas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarea(int id, Tarea tarea)
        {
            if (id != tarea.Id)
            {
                return BadRequest();
            }

            _context.Entry(tarea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TareaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ExampleTareas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tarea>> PostTarea(Tarea tarea)
        {
          if (_context.Tareas == null)
          {
              return Problem("Entity set 'AplicationDbContext.Tareas'  is null.");
          }
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTarea", new { id = tarea.Id }, tarea);
        }

        // DELETE: api/ExampleTareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            if (_context.Tareas == null)
            {
                return NotFound();
            }
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TareaExists(int id)
        {
            return (_context.Tareas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
