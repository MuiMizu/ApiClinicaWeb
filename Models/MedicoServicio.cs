namespace APIClinica.Models
{
    public class MedicoServicio
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public int ServicioId { get; set; }

        public Medico Medico { get; set; } = null!;
        public Servicio Servicio { get; set; } = null!;
    }
}