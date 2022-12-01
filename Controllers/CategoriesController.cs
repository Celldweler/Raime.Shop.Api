using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.DAL;
using Shop.Dapper.DAL.AppRepositories;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private ApplicationDbContext _ctx;
        private DapperCategoriesRepository _categoriesRepos;

        public CategoriesController(ApplicationDbContext ctx, DapperCategoriesRepository dapperCategoriesRepository)
        {
            _ctx = ctx;
            _categoriesRepos = dapperCategoriesRepository;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_categoriesRepos.All());


        [HttpGet("{categoryId}")]
        public IActionResult GetById(int categoryId)
        {
            return Ok(_categoriesRepos.GetCategoryById(categoryId));
        }

        [HttpDelete("{categoryId}")]
        public IActionResult DeleteAsync(int categoryId)
        {
            //var category = _ctx.Categories.FirstOrDefault(x => x.Id == categoryId);
            //_ctx.Categories.Remove(category);

            //await _ctx.SaveChangesAsync();

            _categoriesRepos.RemoveCategory(categoryId);
            return Ok();
        }

        [HttpPost]
        public IActionResult Create([FromBody]Category category)
        {
            var result = _categoriesRepos.AddCategory(category.Name);

            return Ok(result);
        }
    }
}
