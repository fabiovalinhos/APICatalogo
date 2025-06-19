using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class GetProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _produtosController;

        public GetProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _produtosController = new ProdutosController(controller._uof);
        }

        [Fact]
        public async Task GetProdutoById_OKResult()
        {
            //Arrange
            var prodId = 2;

            // Act
            var data = await _produtosController.Get(prodId);

            //Assert (xunit)
            // var okResult = Assert.IsType<OkObjectResult>(data);
            // Assert.Equal(200, okResult.StatusCode);

            // Assert (fluent assertions)
            data.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetProdutoById_Return_NotFound()
        {
            //Arrange
            var prodId = 999;

            // Act
            var data = await _produtosController.Get(prodId);

            //Assert (xunit)
            // var okResult = Assert.IsType<OkObjectResult>(data);
            // Assert.Equal(200, okResult.StatusCode);

            // Assert (fluent assertions)
            data.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetProdutoById_Return_BadRequest()
        {
            //Arrange
            var prodId = -1;

            // Act
            var data = await _produtosController.Get(prodId);

            //Assert (xunit)
            // var okResult = Assert.IsType<OkObjectResult>(data);
            // Assert.Equal(200, okResult.StatusCode);

            // Assert (fluent assertions)
            data.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetProdutos_Return_ListOfProdutoDTO()
        {
            // Act
            var data = await _produtosController.Get();

            // Assert (fluent assertions)
            data.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should()
                .BeAssignableTo<IEnumerable<ProdutoDTO>>()
                .And.NotBeNull();


        }
        
        [Fact]
        public async Task GetProdutos_Return_BadRequestResult()
        {
            // Act
            var data = await _produtosController.Get();

            // Assert (fluent assertions)
            data.Result.Should().BeOfType<BadRequestResult>();
        }
    }
}