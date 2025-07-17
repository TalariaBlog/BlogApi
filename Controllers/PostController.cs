using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using BlogApp_SharedModels.Models;
using Blog.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace Blog.Controllers
{
    [Route("api/Posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDBContext _dbcontext;

        public PostController(AppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [AllowAnonymous]
        [HttpGet("GetPosts")]
        public IActionResult GetAllPosts()
        {
            int? userId = null;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    userId = int.Parse(userIdClaim);
                }
            }

            var posts = _dbcontext.posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.createdat)
                .Select(p => new
                {
                    id = p.id,
                    userid = p.userid,
                    imageurl = p.imageurl,
                    description = p.description,
                    createdat = p.createdat,
                    username = p.User!.name,
                    userimage = p.User.profilepicture,
                    likesCount = p.Likes.Count(l => l.commentid == null),

                    hasLiked = userId != null
                        ? p.Likes.Any(l => l.userid == userId && l.commentid == null)
                        : false
                })
                .ToList();

            return Ok(posts);
        }

        [Authorize]
        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePost([FromBody] PostDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.imageurl))
            {
                return BadRequest("No se ha seleccionado ninguna imagen...");
            }

            var userid = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userid == null || !int.TryParse(userid.Value, out int userId))
            {
                return Unauthorized(new { message = "Usuario no válido" });
            }
            var post = new Post { imageurl = model.imageurl, description = model.description, userid = userId};
            _dbcontext.posts.Add(post);
            await _dbcontext.SaveChangesAsync();
            return Ok(new {message = "Post creado con éxito", post });
        }

        [Authorize]
        [HttpDelete("DeletePost/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid == null || !int.TryParse(userid, out int userId))
            {
                return Unauthorized(new {message = "Usuario no válido..."});
            }

            var post = await _dbcontext.posts.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post no encontrado..." });
            }

            if (post.userid != userId)
            {
                return Forbid();
            }

            _dbcontext.posts.Remove(post);
            await _dbcontext.SaveChangesAsync();
            return Ok(new { message = "Post eliminado con éxito..." });
        }

        [Authorize]
        [HttpGet("GetPostsByUser/{userId}")]
        public async Task<IActionResult> GetPostsByUser(int userId)
        {
            var posts = await _dbcontext.posts
                .Where(p => p.userid == userId)
                .OrderByDescending(p => p.createdat)
                .ToListAsync();

            return Ok(posts);
        }
    }
}
