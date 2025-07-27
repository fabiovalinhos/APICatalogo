using Catalogo.Application.DTOs;
using Catalogo.Domain.Interfaces;
using Catalogo.Application.Mapping;
using Catalogo.Application.Interfaces;

namespace Catalogo.Application.Services;

public class CategoriaService : ICategoriaService
{
    private ICategoriaRepository _categoryRepository;
    public CategoriaService(ICategoriaRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoriaDTO>> GetCategorias()
    {
        try
        {
            var categoriesEntity = await _categoryRepository.GetCategoriasAsync();
            var categoriasDto = categoriesEntity.Select(DtoConverter.ToCategoriaDto);
            return categoriasDto;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<CategoriaDTO> GetById(int? id)
    {
        var categoryEntity = await _categoryRepository.GetByIdAsync(id);
        return DtoConverter.ToCategoriaDto(categoryEntity);
    }

    public async Task Add(CategoriaDTO categoryDto)
    {
        var categoryEntity = DtoConverter.ToCategoria(categoryDto);
        await _categoryRepository.CreateAsync(categoryEntity);
    }

    public async Task Update(CategoriaDTO categoryDto)
    {
        var categoryEntity = DtoConverter.ToCategoria(categoryDto);
        await _categoryRepository.UpdateAsync(categoryEntity);
    }

    public async Task Remove(int? id)
    {
        var categoryEntity = await _categoryRepository.GetByIdAsync(id);
        await _categoryRepository.RemoveAsync(categoryEntity);
    }
}
