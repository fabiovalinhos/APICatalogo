using Catalogo.Application.DTOs;
using Catalogo.Application.Interfaces;
using Catalogo.Domain.Interfaces;
using Catalogo.Application.Mapping;

namespace Catalogo.Application.Services;

public class ProdutoService : IProdutoService
{
    private IProdutoRepository _productRepository;

    public ProdutoService(IProdutoRepository productRepository)
    {
        _productRepository = productRepository ??
             throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<IEnumerable<ProdutoDTO>> GetProdutos()
    {
        var productsEntity = await _productRepository.GetProdutosAsync();
        return productsEntity.Select(DtoConverter.ToProdutoDto);
    }

    public async Task<ProdutoDTO> GetById(int? id)
    {
        var productEntity = await _productRepository.GetByIdAsync(id);
        return DtoConverter.ToProdutoDto(productEntity);
    }

    public async Task Add(ProdutoDTO productDto)
    {
        var productEntity = DtoConverter.ToProduto(productDto);
        await _productRepository.CreateAsync(productEntity);
    }

    public async Task Update(ProdutoDTO productDto)
    {
        var productEntity = DtoConverter.ToProduto(productDto);
        await _productRepository.UpdateAsync(productEntity);
    }

    public async Task Remove(int? id)
    {
        var productEntity = await _productRepository.GetByIdAsync(id);
        await _productRepository.RemoveAsync(productEntity);
    }
}
