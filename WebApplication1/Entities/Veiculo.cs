using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("veiculos")]
    public class Veiculo
    {
        [Key]
        [Column("id_veiculo")]
        public int Id { get; set; }

        [Column("placa")]
        [Required]
        [StringLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Column("marca")]
        [StringLength(50)]
        public string? Marca { get; set; }

        [Column("modelo")]
        [StringLength(50)]
        public string? Modelo { get; set; }

        [Column("tipo")]
        [StringLength(30)]
        public string? Tipo { get; set; }

        [Column("proprietario")]
        [StringLength(100)]
        public string? Proprietario { get; set; }

        // Relacionamentos
        public virtual ICollection<Multa>? Multas { get; set; }
    }
}
