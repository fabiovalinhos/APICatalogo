using Catalogo.Domain.Validation;

namespace Catalogo.Domain.Entities
{
    public sealed class Categoria: Entity
    {
        public string Nome { get; private set; }

        public string ImagemUrl { get; private set; }

        public ICollection<Produto> Produtos { get; set; }


        public Categoria(string nome, string imagemUrl)
        {
            ValidateDomain(nome, imagemUrl);
        }

        public Categoria(int id, string nome, string imagemUrl)
        {
            DomainExceptionValidation.When(id < 0,
            "Id inválido. O id não pode ser negativo.");
            Id = id;

            ValidateDomain(nome, imagemUrl);
        }

        private void ValidateDomain(string nome, string imagemUrl)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(nome),
            "Nome inválido. O nome é obrigatório.");

            DomainExceptionValidation.When(string.IsNullOrEmpty(imagemUrl),
            "ImagemUrl inválido. A imagem é obrigatória.");

            DomainExceptionValidation.When(imagemUrl.Length < 5,
            "ImagemUrl inválido. A imagem deve ter pelo menos 5 caracteres.");

            DomainExceptionValidation.When(nome.Length < 3,
            "Nome inválido. O nome deve ter pelo menos 3 caracteres.");

            Nome = nome.Trim();
            ImagemUrl = imagemUrl.Trim();
        }
    }
}