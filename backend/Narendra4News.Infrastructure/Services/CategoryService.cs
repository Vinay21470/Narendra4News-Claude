using Microsoft.EntityFrameworkCore;
using Narendra4News.Application.DTOs.Categories;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Entities;
using Narendra4News.Infrastructure.Data;

namespace Narendra4News.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;
    public CategoryService(ApplicationDbContext db) => _db = db;

    public async Task<List<CategoryDto>> GetAllAsync(bool activeOnly, CancellationToken ct = default)
    {
        var q = _db.Categories.AsQueryable();
        if (activeOnly) q = q.Where(c => c.IsActive);
        return await q.OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id, Name = c.Name, Slug = c.Slug, Description = c.Description,
                DisplayOrder = c.DisplayOrder, IsActive = c.IsActive,
                ArticleCount = c.Articles.Count
            }).ToListAsync(ct);
    }

    public async Task<CategoryDto> CreateAsync(UpsertCategoryRequest request, CancellationToken ct = default)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug) ? DbInitializer.Slugify(request.Name) : request.Slug;
        var category = new Category
        {
            Name = request.Name, Slug = slug, Description = request.Description,
            DisplayOrder = request.DisplayOrder, IsActive = request.IsActive
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return new CategoryDto { Id = category.Id, Name = category.Name, Slug = category.Slug, Description = category.Description, DisplayOrder = category.DisplayOrder, IsActive = category.IsActive };
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpsertCategoryRequest request, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct) ?? throw new KeyNotFoundException("Category not found");
        category.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Slug)) category.Slug = request.Slug;
        category.Description = request.Description;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.UpdatedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return new CategoryDto { Id = category.Id, Name = category.Name, Slug = category.Slug, Description = category.Description, DisplayOrder = category.DisplayOrder, IsActive = category.IsActive };
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct);
        if (category is not null) { _db.Categories.Remove(category); await _db.SaveChangesAsync(ct); }
    }
}
