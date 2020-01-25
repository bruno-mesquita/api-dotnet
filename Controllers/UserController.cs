using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Services;
using Shop.Models;

namespace Shop.Controllers {

    [Route("users")]
    public class UserController : ControllerBase {

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Get([FromServices]DataContext context) {
            
            var users = await context.Users.AsNoTracking().ToListAsync();

            return Ok(users);
        }
    
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromServices]DataContext context, [FromBody]User model) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            model.Role = "employee";

            try {
                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";

                return Ok(model);
            } catch (Exception) {
                return BadRequest(new { message = "Não foi possível criar um usuário" });
            }
        }
    
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put([FromServices]DataContext context, [FromBody]User model, int id) {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if(id != model.Id) {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            try {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            } catch (Exception) {
                return BadRequest(new { message = "Não foi possível atualizar um usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromServices]DataContext context, [FromBody]User model) {
            
            var user = await context.Users
                .AsNoTracking()
                .Where(user => user.Username == model.Username && user.Password == model.Password)
                .FirstOrDefaultAsync();
            
            if(user == null) {
                return NotFound(new { message = "Usuário ou senha inválidos" });
            }

            user.Password = "";

            var token = TokenService.GenerateToken(user);
            return new {
                user = user,
                token = token
            };
        }
    }
}