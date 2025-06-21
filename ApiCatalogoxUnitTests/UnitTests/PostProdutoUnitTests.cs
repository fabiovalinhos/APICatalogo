using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PostProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _produtosController;

        public PostProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _produtosController = new ProdutosController(controller._uof);
        }


        // Métodos de testes para POST

        [Fact]
        public async Task PostProduto_Return_CreatedStatusCode()
        {
            // Arrange
            var novoProduto = new ProdutoDTO
            {
                Nome = "Novo Produto",
                Descricao = "Descrição do novo produto",
                Preco = 100.00m,
                ImageUrl = "imagemfake1.jpg",
                CategoriaId = 2
            };

            // Act
            var data = await _produtosController.Post(novoProduto);

            // Assert
            var createdResult =
            data.Result.Should().BeOfType<CreatedAtRouteResult>();
            createdResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task PostProduto_Return_BadRequest()
        {
            // Arrange
            ProdutoDTO prod = null;

            // Act
            var data = await _produtosController.Post(prod);

            // Assert
            var badRequestResult = data.Result.Should().BeOfType<BadRequestResult>();
            badRequestResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}