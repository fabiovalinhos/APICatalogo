using ApiCatalogo.Models;

namespace ApiCatalogo.DTOs
{
    public static class CategoriaMapper
    {
        public static CategoriaDTO? ParaDTOMapper(this Categoria entidade)
        {
            if (entidade is null) return null;

            return new CategoriaDTO
            {
                CategoriaId = entidade.CategoriaId,
                ImageUrl = entidade.ImageUrl,
                Nome = entidade.Nome
            };
        }

        public static Categoria? ParaCategoriaMapper(this CategoriaDTO entidade)
        {
            if (entidade is null) return null;

            return new Categoria
            {
                CategoriaId = entidade.CategoriaId,
                ImageUrl = entidade.ImageUrl,
                Nome = entidade.Nome
            };
        }

        public static IEnumerable<CategoriaDTO> ParaDTOListaMapper(this IEnumerable<Categoria> entidade)
        {
            if (entidade is null || !entidade.Any())
            {
                return new List<CategoriaDTO>();
            }

            return entidade.Select(categ => new CategoriaDTO
            {
                CategoriaId = categ.CategoriaId,
                ImageUrl = categ.ImageUrl,
                Nome = categ.Nome
            }).ToList();
        }
    }
}
