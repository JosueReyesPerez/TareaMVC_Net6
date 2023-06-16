namespace TareaMVC.Entities
{
    public class Paso
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }

        public int TareaId { get; set; }
        public Tarea Tarea { get; set; }
    }
}
