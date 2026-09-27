using Narendra4News.Application.DTOs.Categories;

namespace Narendra4News.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(bool activeOnly, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(UpsertCategoryRequest request, CancellationToken ct = default);
    Task<CategoryDto> UpdateAsync(int id, UpsertCategoryRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
