using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narendra4News.Application.Common;
using Narendra4News.Application.DTOs.Categories;
using Narendra4News.Application.Interfaces;
using Narendra4News.Domain.Enums;

namespace Narendra4News.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll([FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        var result = await _categoryService.GetAllAsync(activeOnly, ct);
        return Ok(ApiResponse<List<CategoryDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(UpsertCategoryRequest request, CancellationToken ct)
    {
        var category = await _categoryService.CreateAsync(request, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(category, "Category created successfully"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(int id, UpsertCategoryRequest request, CancellationToken ct)
    {
        var category = await _categoryService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(category, "Category updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _categoryService.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Category deleted successfully"));
    }
}
