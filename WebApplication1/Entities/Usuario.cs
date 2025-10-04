using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nome")]
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("cpf")]
        [StringLength(14)]
        public string? CPF { get; set; }

        [Column("email")]
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("senha")]
        [Required]
        [StringLength(100)]
        public string Senha { get; set; } = string.Empty;

        [Column("tipo")]
        [Required]
        public string Tipo { get; set; } = string.Empty; // enum (agente, supervisor, administrador)

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; }

        // Relacionamentos
        public virtual ICollection<Multa>? Multas { get; set; }
    }
}
