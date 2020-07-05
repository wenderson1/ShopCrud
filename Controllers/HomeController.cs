using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>>Get([FromServices] DataContext context)
        {
            var employee = new User{Id = 1, Username="Robin",Role="Employee"};
            var manager = new User{Id = 2, Username="batman",Role="Manager"};
            var category = new Category{Id=1,Title="Informatica"};
            var product = new Product{Id=1,Category=category,Title="Mouse",Price=300,Description="Mouse Gamer"};

            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message="Dados configurados"
            });
        }
    }
}