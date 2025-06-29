using ApiCatalogo.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class DeleteProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _produtosController;

        public DeleteProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _produtosController = new ProdutosController(controller._uof);
        }

        //Testes para o DELETE
        [Fact]
        public async Task DeleteProdutoById_Return_OkResult()
        {
            var produtID = 19;

            //Act
            var result = await _produtosController.Delete(produtID);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteProdutoById_Return_NotFound()
        {
            var produtID = 1999;

            //Act
            var result = await _produtosController.Delete(produtID);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}