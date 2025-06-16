using ApiCatalogo.Context;
using ApiCatalogo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class ProdutosUnitTestController
    {
        private readonly IUnitOfWork _uof;

        public static DbContextOptions<AppDbContext> dbContextOptions { get; }

        public static string connectionString =
        "Server=localhost;Database=CatalogoDB;Uid=root;Pwd=alfacinho";

        static ProdutosUnitTestController()
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
    }
}