using BlinkIt.Repository.Models;

namespace BlinkIt.Service.Interfaces;

public interface ICategoryService
{
    public List<Category> GetCategory();
}