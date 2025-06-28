using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PutProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _produtosController;

        public PutProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _produtosController = new ProdutosController(controller._uof);
        }

        //testes de unidade para PUT
        [Fact]
        public async Task PutProduto_Return_OkResult()
        {
            // Arrange
            var prodId = 14;

            var UpdateProdutoDTO = new ProdutoDTO
            {
                ProdutoId = prodId,
                Nome = "Produto Atualizado Test",
                ImageUrl = "https://example.com/image-atualizada.jpg",
                Descricao = "Descrição atualizada do produto",
                CategoriaId = 2
            };

            //Act
            var result = await _produtosController.Put(prodId, UpdateProdutoDTO);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutProduto_Return_BadRequest()
        {
            // Arrange
            var prodId = 14;

            var UpdateProdutoDTO = new ProdutoDTO
            {
                ProdutoId = prodId,
                Nome = "Produto Atualizado Test",
                ImageUrl = "https://example.com/image-atualizada.jpg",
                Descricao = "Descrição atualizada do produto",
                CategoriaId = 2
            };

            // Act
            var result = await _produtosController.Put(prodId + 1, UpdateProdutoDTO);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);

        }
    }
}