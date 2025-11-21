using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIClinica.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string Documento { get; set; } = string.Empty;
        [StringLength(20)]
        public string? Telefono { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public int SeguroId { get; set; }

        [ForeignKey(nameof(SeguroId))]
        public Seguro Seguro { get; set; } = null!;
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}