using ApiCatalogo.Models;

namespace ApiCatalogo.DTOs
{
    public static class CategoriaMapper
    {
        public static CategoriaDTO? MapperParaCategoriaDTO(this Categoria entidade)
        {
            if (entidade is null) return null;

            return new CategoriaDTO
            {
                CategoriaId = entidade.CategoriaId,
                ImageUrl = entidade.ImageUrl,
                Nome = entidade.Nome
            };
        }

        public static Categoria? MapperParaCategoria(this CategoriaDTO entidade)
        {
            if (entidade is null) return null;

            return new Categoria
            {
                CategoriaId = entidade.CategoriaId,
                ImageUrl = entidade.ImageUrl,
                Nome = entidade.Nome
            };
        }

        public static IEnumerable<CategoriaDTO> MapperParaListaCategoriaDTO(this IEnumerable<Categoria> entidade)
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
