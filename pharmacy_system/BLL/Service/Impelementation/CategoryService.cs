using BLL.DTOs;
using BLL.Service.Abstraction;
using DAL.Entities;
using DAL.Repo.Abstraction;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepo _repo;

    public CategoryService(ICategoryRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var data = await _repo.GetAllAsync();

        return data.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);

        if (c == null) return null;

        return new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        };
    }

    public async Task AddAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        await _repo.AddAsync(category);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null) return;

        if (category.Products != null && category.Products.Any())
            throw new InvalidOperationException("Cannot delete category with existing products");

        _repo.Delete(category);
        await _repo.SaveChangesAsync();
    }
}