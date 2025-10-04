using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Entities
{
    [Table("tipo_multa")]
    public class TipoMulta
    {
        [Key]
        [Column("id_tipo_multa")]
        public int Id { get; set; }

        [Column("codigo_multa")]
        [Required]
        [StringLength(20)]
        public string CodigoMulta { get; set; } = string.Empty;

        [Column("descricao")]
        [StringLength(255)]
        public string? Descricao { get; set; }

        [Column("valor_multa")]
        [Required]
        [Precision(10, 2)]
        public decimal ValorMulta { get; set; }

        [Column("gravidade")]
        [Required]
        public string Gravidade { get; set; } = string.Empty; // enum (leve, media, grave, gravissima)

        [Column("pontos")]
        public int Pontos { get; set; }

        // Relacionamentos
        public virtual ICollection<Multa>? Multas { get; set; }
    }
}