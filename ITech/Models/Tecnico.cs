using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ITech.Models
{
        [Table("Tecnicos")]
        public class Tecnico
        {
            [Key]
            public int TecnicoId { get; set; }

            [Display(Name = "Pessoa Jurídica (s) ou fisica (n)?")]
            public bool IsJuridica { get; set; }

            [Required(ErrorMessage = "O CPF/CNPJ deve ser infomado!!")]
            [Display(Name = "CPF/CNPJ")]
            [StringLength(200, MinimumLength = 8, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres!")]
            public string DocIdentificacao { get; set; }

            [Required(ErrorMessage = "O nome do Técnico deve ser informado!")]
            [Display(Name = "Nome do Técnico")]
            [StringLength(80, MinimumLength = 3, ErrorMessage = "O {0} deve ter no mínimo {2} e no máximo {1} caracteres!")]
            public string TecnicoNome { get; set; }

            [Required(ErrorMessage = "O email do Técnico deve ser informado!")]
            [Display(Name = "Email do Técnico")]
            [StringLength(100, MinimumLength = 5, ErrorMessage = "O {0} deve ter no mínimo {2} e no máximo {1} caracteres!")]
            public string Email { get; set; }

            [Required(ErrorMessage = "O telefone do Técnico deve ser informado!")]
            [Display(Name = "Telefone do Técnico")]
            [StringLength(80, MinimumLength = 8, ErrorMessage = "O {0} deve ter no mínimo {2} e no máximo {1} caracteres!")]
            public string Telefone { get; set; }

            [Required(ErrorMessage = "O endereço completo do Técnico deve ser informado!")]
            [Display(Name = "Endereco do Técnico")]
            [StringLength(500, MinimumLength = 10, ErrorMessage = "O {0} deve ter no mínimo {2} e no máximo {1} caracteres!")]
            public string Endereco { get; set; }


            public List<Servico> Servicos { get; set; }
        }
    }
