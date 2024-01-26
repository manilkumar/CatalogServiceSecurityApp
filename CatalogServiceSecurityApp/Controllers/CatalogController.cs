namespace CatalogServiceSecurityApp.Controllers
{
    using CatalogServiceSecurityApp.Models.DbModels;
    using CatalogServiceSecurityApp.Services;
    using CatalogServiceSecurityApp.Models.DbModels;
    using CatalogServiceSecurityApp.Models.InputModels;
    using CatalogServiceSecurityApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CatalogController : Controller
    {
        private readonly ILogger logger;
        private readonly CategoryService categoryService;

        public CatalogController(ILogger logger, CategoryService categoryService)
        {
            this.categoryService = categoryService;
            this.logger = logger;
        }

        [Authorize(Roles = "Buyer,Manager")]
        [HttpGet]
        public ActionResult<List<Category>> Get()
        {
            try
            {
                var categories = this.categoryService.GetAll();
                return Ok(categories);
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult Create([FromBody] Category category)
        {
            try
            {
                this.categoryService.CreateCateogry(category);
                return Ok();
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut]
        public ActionResult Update([FromBody] Category category)
        {
            try
            {
                this.categoryService.UpdateCateogry(category);
                return Ok();
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete]
        public ActionResult Delete(int categoryId)
        {
            try
            {
                this.categoryService.DeleteCateogry(categoryId);
                return Ok();
            }
            catch (Exception error)
            {
                logger.LogError(error.Message);
                return StatusCode(500);
            }
        }
    }
}
