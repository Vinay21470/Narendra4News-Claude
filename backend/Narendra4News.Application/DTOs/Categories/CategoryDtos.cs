namespace Narendra4News.Application.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int ArticleCount { get; set; }
}

public class UpsertCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
