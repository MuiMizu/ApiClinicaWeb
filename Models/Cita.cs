using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APIClinica.Models.Enums;

namespace APIClinica.Models
{
    public class Cita
    {
        public int Id { get; set; }
        [Required]
        public int PacienteId { get; set; }
        [Required]
        public int MedicoId { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }
        [Required]
        [RegularExpression(@"^(0[8-9]|1[0-7]):00$", ErrorMessage = "La hora debe estar entre 08:00 y 17:00")]
        public string Hora { get; set; } = string.Empty;
        [Required]
        public EstadoCita Estado { get; set; } = EstadoCita.Programada;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PacienteId))]
        public Paciente Paciente { get; set; } = null!;
        [ForeignKey(nameof(MedicoId))]
        public Medico Medico { get; set; } = null!;
    }
}