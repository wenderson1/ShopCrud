using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;
namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {

        [HttpGet]
        [Route("")]
        [Authorize(Roles="manager")]        
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context)
        {
            var users = await context
                .Users
                .Include(x=>x.Username)
                .AsNoTracking()
                .ToListAsync();
            
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles="manager")]  
        public async Task<ActionResult<User>> Post([FromBody]User model,
                                                            [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password="";
                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message="Não foi possível criar um Usuário"});
            }
        }

    [HttpPut]
    [Route("{id}:int")]
    public async Task<ActionResult<User>> Put(int id, [FromBody]User model,[FromServices]DataContext context)
    {
        //verifica se o ID informado e o mesmo do modelo
        if(id != model.Id)
            return NotFound(new{message="Funcionario não encontrada"});

        //verifica se os dados são válidos
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try{
            context.Entry<User>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch(DbUpdateConcurrencyException)
        {
            return BadRequest(new {message="Este registro já foi atualizado"});
        }
        catch(Exception)
        {
            return BadRequest(new{message="Não foi possível atualizar a categoria"});
        }
    }

    [HttpDelete]
    [Route("{id}:int")]
    [Authorize(Roles="manager")]
    public async Task<ActionResult<List<User>>> Delete(int id, [FromServices]DataContext context)
    {
        var user = await context.Users.FirstOrDefaultAsync(x=>x.Id == id);
       
        if(user == null)
            return NotFound(new {message="Categoria não encontrada"});

        try
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return Ok(new {message="Funcionario removido com sucesso!!"});
        }        
        
        catch(Exception)
        {
            return BadRequest(new{message="Não foi possível remover o funcionário"});
        }
    }


        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>>Authenticate(
                    [FromServices] DataContext context1,
                    [FromBody]User model)
        {
            var user = await context1.Users
                .AsNoTracking()
                .Where(x=>x.Username==model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if(user==null)
                return NotFound(new {message = "Usuário ou senha inválidos"});

            var token = TokenService.GenerateToken(user);
            
            user.Password="";
            return new
            {
                user=user,
                token=token
            };
        }
    

    }
}