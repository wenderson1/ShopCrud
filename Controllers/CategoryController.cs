using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


//https://localhost:5001/categories
[Route("v1/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("{id:int}")]//{} encara como parametro e : como restrição de rota
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync();
        return category;
    }

    [HttpGet]
    [Route("")]//{} encara como parametro e : como restrição de rota
    [AllowAnonymous]
    public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
    {
        //var categories = await context.Categories.AsNoTracking().OrderBy(x => x.Title).ToListAsync();
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles="employee")]
    public async Task<ActionResult<List<Category>>> Post([FromBody]Category model,
                                                         [FromServices]DataContext context)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new {message="Não foi possível criar a categoria"});
        }
    }

    [HttpPut]
    [Route("{id}:int")]
    [Authorize(Roles="employee")]
    public async Task<ActionResult<List<Category>>> Put(int id, [FromBody]Category model,[FromServices]DataContext context)
    {
        //verifica se o ID informado e o mesmo do modelo
        if(id != model.Id)
            return NotFound(new{message="Categoria não encontrada"});

        //verifica se os dados são válidos
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try{
            context.Entry<Category>(model).State = EntityState.Modified;
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
    [Authorize(Roles="employee")]
    public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices]DataContext context)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x=>x.Id == id);
       
        if(category == null)
            return NotFound(new {message="Categoria não encontrada"});

        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new {message="Categoria removida com sucesso!!"});
        }        
        
        catch(Exception)
        {
            return BadRequest(new{message="Não foi possível remover a categoria"});
        }
    }
}
