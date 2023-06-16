using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TareaMVC.Entities
{
    public class Tarea
    {
        public int Id { get; set; }

        //[StringLength(200)]
        //[Required]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion {get; set;}

        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }

        //Atributos de Navegacion.
        public List<Paso> pasos { get; set; }
        public List<ArchivoAdjunto> archivoAdjuntos { get; set; }

    }
}
