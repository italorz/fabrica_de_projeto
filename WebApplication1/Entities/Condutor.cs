using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("condutor")]
    public class Condutor
    {
        [Key]
        [Column("id_condutor")]
        public int Id { get; set; }

        [Column("nome")]
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("cpf")]
        [Required]
        [StringLength(14)]
        public string CPF { get; set; } = string.Empty;

        [Column("cpf_uf")]
        [StringLength(2)]
        public string? CPF_UF { get; set; }

        [Column("cnh")]
        [Required]
        [StringLength(20)]
        public string CNH { get; set; } = string.Empty;

        [Column("categoria_cnh")]
        [StringLength(5)]
        public string? CategoriaCNH { get; set; }

        // Relacionamentos
        public virtual ICollection<Multa>? Multas { get; set; }
    }
}
