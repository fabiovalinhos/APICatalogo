using ApiCatalogo.Context;
using ApiCatalogo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class ProdutosUnitTestController
    {
        public readonly IUnitOfWork _uof;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionString =
        "Server=localhost;Database=CatalogoDB;Uid=root;Pwd=alfacinho";

        static ProdutosUnitTestController() // inicia primeiro e uma única vez
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        }
        public ProdutosUnitTestController()
        {
            var context = new AppDbContext(dbContextOptions);
            _uof = new UnitOfWork(context);
        }


        // Se eu nao fosse usar o banco de dados real e fosse mockar
        // faria assim
        //
        // //
        // static ProdutosUnitTestController()
        // {
        //     dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
        //         .UseInMemoryDatabase("NomeQualquerParaGuardarDadosEmMemoria")
        //         .Options;
        // }

        // public ProdutosUnitTestController()
        // {
        //     var context = new AppDbContext(dbContextOptions);

        //     // Seed inicial para garantir que o produto com ID = 2 exista no teste
        //     if (!context.Produtos.Any())
        //     {
        //         //Aqui eu deveria colocar o Dominio(model) correto do Produto. Preguiça de escrever o Dominio no exemplo
        //         context.Produtos.AddRange(
        //             new Produto { ProdutoId = 1, Nome = "Produto 1", Preco = 10 },
        //             new Produto { ProdutoId = 2, Nome = "Produto 2", Preco = 20 }
        //         );
        //         context.SaveChanges();
        //     }

        //     _uof = new UnitOfWork(context);
        // }
    }
}