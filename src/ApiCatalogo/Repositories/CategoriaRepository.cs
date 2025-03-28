﻿using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
        {
            var categorias = await GetAllAsync(null);

            var categoriasOrdenadas = categorias
                .OrderBy(p => p.CategoriaId)
                .AsQueryable();

            var resultado =
                PagedList<Categoria>.ToPagedList(
                    categoriasOrdenadas, categoriasParameters.PageNumber, categoriasParameters.PageSize);

            return resultado;
        }

        public async Task<PagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome)
        {
            IEnumerable<Categoria> categorias;

            if (!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
            {
                categorias = 
                    await GetAllAsync(c => c.Nome.Contains(categoriasFiltroNome.Nome));
            }
            else
            {
                categorias = await GetAllAsync(null);
            }

                var categoriasFiltradas =
                PagedList<Categoria>.ToPagedList(
                    categorias.AsQueryable(), categoriasFiltroNome.PageNumber, categoriasFiltroNome.PageSize
                );

            return categoriasFiltradas;
        }
    }
}
