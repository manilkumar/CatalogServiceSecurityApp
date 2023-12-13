using CatalogServiceSecurityApp.Models.DbModels;
using HRApplication.Data;

namespace CatalogServiceSecurityApp.Services
{
    public class CategoryService
    {
        private readonly DataContext dataContext;

        public CategoryService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<Category> GetAll()
        {
            return this.dataContext.Categories.ToList();
        }
        public void CreateCateogry(Category category)
        {
            dataContext.Categories.Add(category);
            dataContext.SaveChanges();
        }

        public void UpdateCateogry(Category updateCategory)
        {
            var category = this.dataContext.Categories.Where(i => i.Id == updateCategory.Id).FirstOrDefault();
            if (category != null)
            {
                category.Name = updateCategory.Name;
                category.ImageURL = updateCategory.ImageURL;
            }
            dataContext.SaveChanges();
        }

        public void DeleteCateogry(int categoryId)
        {
            var category = this.dataContext.Categories.Where(i => i.Id == categoryId).FirstOrDefault();
            if (category != null)
            {
                dataContext.Remove(category);
            }
            dataContext.SaveChanges();
        }
    }
}
