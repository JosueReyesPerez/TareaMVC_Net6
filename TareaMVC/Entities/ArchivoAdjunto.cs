using Microsoft.EntityFrameworkCore;

namespace TareaMVC.Entities
{
    public class ArchivoAdjunto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }

        [Unicode]
        public string Url { get; set; }
        public int Order { get; set; }
        public DateTime  FechaCreacion {get; set;}


        public int TareaId { get; set; }
        public Tarea Tarea { get; set; }
    }
}
