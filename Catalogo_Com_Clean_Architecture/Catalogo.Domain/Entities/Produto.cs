using Catalogo.Domain.Validation;

namespace Catalogo.Domain.Entities
{
    public class Produto : Entity
    {
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public decimal Preco { get; private set; }
        public string ImagemUrl { get; private set; }
        public int Estoque { get; private set; }
        public DateTime DataCadastro { get; private set; }

        /////
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        /// 

        public Produto(string nome, string descricao, decimal preco, string imagemUrl,
         int estoque, DateTime dataCadastro)
        {
            ValidateDomain(nome, descricao, preco, imagemUrl, estoque, dataCadastro);
        }

        public Produto(int id, string nome, string descricao, decimal preco, string imagemUrl,
         int estoque, DateTime dataCadastro)
        {
            DomainExceptionValidation.When(id < 0,
            "Id inválido. O id não pode ser negativo.");
            Id = id;

            ValidateDomain(nome, descricao, preco, imagemUrl, estoque, dataCadastro);
        }

        public void Update(string nome, string descricao,
            decimal preco, string imagemUrl, int estoque, DateTime dataCadastro, int categoriaId)
        {
            ValidateDomain(nome, descricao, preco, imagemUrl, estoque, dataCadastro);
            CategoriaId = categoriaId;
        } 

        private void ValidateDomain(string nome, string descricao,
        decimal preco, string imagemUrl, int estoque, DateTime dataCadastro)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(nome),
                "Nome inválido. O nome é obrigatório.");

            DomainExceptionValidation.When(nome.Length < 3,
            "Nome inválido. O nome deve ter pelo menos 3 caracteres.");

            DomainExceptionValidation.When(string.IsNullOrEmpty(descricao),
                "Descrição inválida. A descrição é obrigatória.");

            DomainExceptionValidation.When(preco < 0,
                "Preço inválido. O preço deve ser maior que zero.");

            DomainExceptionValidation.When(estoque < 0,
                "Estoque inválido. O estoque não pode ser negativo.");

            DomainExceptionValidation.When(imagemUrl.Length > 250,
                "ImagemUrl inválido. Excedeu o tamanho máximo de 250 caracteres.");

            Nome = nome.Trim();
            Descricao = descricao.Trim();
            ImagemUrl = imagemUrl.Trim();
            Preco = preco;
            Estoque = estoque;
            DataCadastro = dataCadastro;
        }
    }
}