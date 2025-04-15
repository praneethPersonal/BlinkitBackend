using BlinkIt.Repository.Interfaces;
using BlinkIt.Repository.Models;
using BlinkIt.Service.Interfaces;

namespace BlinkIt.Service.Implementation;

public class CategoryService : ICategoryService
{
    private readonly IBlinkItRepository _blinkItRepository;

    public CategoryService(IBlinkItRepository blinkItRepository)
    {
        _blinkItRepository = blinkItRepository;
    }
    
    public List<Category> GetCategory()
    {
        var products = _blinkItRepository.GetCategoryFromRepo();
        return products;
    }
}