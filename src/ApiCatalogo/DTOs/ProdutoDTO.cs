using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.DTOs
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(80)]
        public string? Nome { get; set; }

        [Required]
        [StringLength(300)]
        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string? ImageUrl { get; set; }

        public int CategoriaId { get; set; }

    }
}