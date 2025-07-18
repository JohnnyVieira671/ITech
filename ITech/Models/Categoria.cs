﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITech.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [StringLength(100, ErrorMessage ="O tamanho máximo é 100 caracteres!")]
        [Required(ErrorMessage ="Informe o nome da Categoria!")]
        [Display(Name ="Nome")]
        public string CategoriaNome { get; set; }


        [StringLength(200, ErrorMessage = "O tamanho máximo é 200 caracteres!")]
        [Required(ErrorMessage = "Informe a Descrição da Categoria!")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }


        public List<Servico> Servicos { get; set;}
    }
}
