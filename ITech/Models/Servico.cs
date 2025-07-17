using ITech.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITech.Models
{
    [Table("Servicos")]
     public class Servico
    {
        [Key]
        public int ServicoId { get; set; }


        [Required(ErrorMessage = "A descrição do Servico deve ser informada!")]
        [Display(Name = "Descrição do Servico")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "A {0} deve ter no mínimo {1} e no máximo {2} caracteres!")]
        public string DescricaoCurta { get; set; }

        
        [Required(ErrorMessage = "A descrição detalhada do Servico deve ser informada!")]
        [Display(Name = "Descrição detalhada do Servico")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "A {0} deve ter no mínimo {1} e no máximo {2} caracteres!")]
        public string DescricaoDetalhada { get; set; }


        [Required(ErrorMessage = "O valor do Servico deve ser informado!")]
        [Display(Name = "Valor")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(1, 999.99, ErrorMessage = "O valor deve estar entre R$1.00 e R$999.99!")]
        public decimal Valor { get; set; }


        [Display(Name = "Em disposição?")]
        public bool EmDisposicao { get; set; }


        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }

        public int TecnicoId { get; set; }
        public virtual Tecnico Tecnicos { get; set; }
    }
}
