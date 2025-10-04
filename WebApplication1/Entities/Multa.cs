using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("multas")]
    public class Multa
    {
        [Key]
        [Column("id_multas")]
        public int Id { get; set; }

        [Column("id_veiculo")]
        [Required]
        public int VeiculoId { get; set; }

        [Column("id_usuario")]
        [Required]
        public int UsuarioId { get; set; }

        [Column("condutor_id")]
        public int? CondutorId { get; set; }

        [Column("data_hora")]
        [Required]
        public DateTime DataHora { get; set; }

        [Column("endereco")]
        [StringLength(255)]
        public string? Endereco { get; set; }

        [Column("descricao")]
        [StringLength(255)]
        public string? Descricao { get; set; }

        [Column("id_tipo_multa")]
        [Required]
        public int TipoMultaId { get; set; }

        // Relacionamentos
        [ForeignKey("VeiculoId")]
        public virtual Veiculo? Veiculo { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("CondutorId")]
        public virtual Condutor? Condutor { get; set; }

        [ForeignKey("TipoMultaId")]
        public virtual TipoMulta? TipoMulta { get; set; }
    }
}
