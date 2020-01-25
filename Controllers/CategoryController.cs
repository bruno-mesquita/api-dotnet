using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers {

    [Route("categories")]
    public class CategoryController : ControllerBase {

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context) {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            
            return Ok(categories);
        }
        
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context) {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            
            return Ok(category);
        }
        
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post([FromBody]Category model, [FromServices]DataContext context) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

        try{
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                
                return Ok(model);
        } catch {  
            return BadRequest(new { message = "Não foi possivel criar a categoria" });
        }
        }
        
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(int id, [FromBody]Category model, [FromServices]DataContext context) {
            // Verifica se o ID informado é o mesmo do modelo
            if(id != model.Id) {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            // Verifica se os dados são válidos
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            } catch (DbUpdateConcurrencyException) {
                return BadRequest(new { message = "Esse registro já foi atualizad" });
            } catch (System.Exception) {
                return BadRequest(new { message = "Não foi possivel atualizar a categoria" });
            }

            
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices]DataContext context) {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(category == null) {
                return NotFound(new { message = "Categoria não encontrada" });
            }        
            
            try {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new { message = "Categoria removida com sucesso" });
            } catch (System.Exception) {
                return BadRequest(new { message = "Não foi possivel remover a categoria" });
            }
        }
    }

}
