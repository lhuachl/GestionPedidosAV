using AutoMapper;
using GestionPedidosAV.Application.DTOs;
using GestionPedidosAV.Application.Interfaces;
using GestionPedidosAV.Domain.Entities;
using GestionPedidosAV.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionPedidosAV.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IValidationService validationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationService = validationService;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAvailableProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(ProductSearchDto searchDto)
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        var query = products.AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (p.Description != null && p.Description.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (p.Category != null && p.Category.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrEmpty(searchDto.Category))
        {
            query = query.Where(p => p.Category == searchDto.Category);
        }

        if (searchDto.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= searchDto.MinPrice.Value);
        }

        if (searchDto.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);
        }

        if (searchDto.Status.HasValue)
        {
            query = query.Where(p => p.Status == searchDto.Status.Value);
        }

        return _mapper.Map<IEnumerable<ProductDto>>(query.ToList());
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var validation = await _validationService.ValidateAsync(createProductDto);
        if (!validation.IsValid)
        {
            throw new ArgumentException(string.Join(", ", validation.Errors));
        }

        var product = _mapper.Map<Product>(createProductDto);
        product.CreatedAt = DateTime.UtcNow;
        product.Status = Domain.Enums.ProductStatus.Active;

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProductDto)
    {
        var existingProduct = await _unitOfWork.Products.GetByIdAsync(updateProductDto.Id);
        if (existingProduct == null)
        {
            throw new ArgumentException("Producto no encontrado");
        }

        _mapper.Map(updateProductDto, existingProduct);
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(existingProduct);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(existingProduct);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
        {
            throw new ArgumentException("Producto no encontrado");
        }

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
    {
        return await _unitOfWork.Products.IsStockAvailableAsync(productId, quantity);
    }

    public async Task UpdateStockAsync(int productId, int quantity)
    {
        await _unitOfWork.Products.UpdateStockAsync(productId, quantity);
        await _unitOfWork.SaveChangesAsync();
    }
}