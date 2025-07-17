using Microsoft.AspNetCore.Authorization;
using Blog.Data;
using BlogApp_SharedModels.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Blog.Controllers
{
    [Route("api/Coments")]
    [ApiController]
    public class ComentController : ControllerBase
    {
        private readonly AppDBContext _dbContext;

        public ComentController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet("GetComents/{PostId}")]
        public IActionResult GetComents(int PostId)
        {
            var Coments = _dbContext.coments.Include(c => c.User).Where(c => c.postid == PostId).OrderByDescending(c => c.createdat).Select(c => new { c.id, c.text, c.createdat, username = c.User!.name }).ToList();
            return Ok(Coments);
        }

        [Authorize]
        [HttpPost("AddComent")]
        public async Task<IActionResult> AddComent([FromBody] Coment model)
        {
            if (string.IsNullOrEmpty(model.text))
            {
                return BadRequest("El comentario no puede estar vacío...");
            }
            var userid = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userid == null || !int.TryParse(userid.Value, out int UserId))
            {
                return Unauthorized("Usuario no válido...");
            }

            var coment = new Coment { postid = model.postid, userid = UserId, text = model.text };
            _dbContext.coments.Add(coment);
            await _dbContext.SaveChangesAsync();
            return Ok(new {message = "Comentario agregado...", coment});
        }

        [Authorize]
        [HttpDelete("DeleteComent/{id}")]
        public async Task<IActionResult> DeleteComent(int id)
        {
            var coment = await _dbContext.coments.FindAsync(id);
            if (coment == null)
            {
                return NotFound("Comentario no encontrado...");
            }
            var userid = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userid == null || !int.TryParse(userid.Value, out int UserId))
            {
                return Unauthorized(new { message = "Usuario no válido..." });
            }
            if (coment.userid != UserId)
            {
                return Unauthorized(new { message = "No tienes permisos para eliminar este comentario..." });
            }
            _dbContext.coments.Remove(coment);
            await _dbContext.SaveChangesAsync();
            return Ok("Comentario eliminado...");
        }
    }
}
