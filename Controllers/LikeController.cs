using System.Security.Claims;
using Blog.Data;
using BlogApp_SharedModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [Route("api/Likes")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        public LikeController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet("GetPostLikes/{PostId}")]
        public IActionResult GetPostLikes(int PostId)
        {
            var Likes = _dbContext.likes.Include(c => c.User).Where(c => c.postid == PostId).OrderByDescending(c => c.createdat).Select(c => new { c.id, c.createdat, username = c.User!.name }).ToList();
            if (Likes == null || Likes.Count == 0)
            {
                return NotFound();
            }
            return Ok(Likes);
        }

        [AllowAnonymous]
        [HttpGet("GetComentLikes/{ComentId}")]
        public IActionResult GetComentLikes(int ComentId)
        {
            var Likes = _dbContext.likes.Include(c => c.User).Where(c => c.commentid == ComentId).OrderByDescending(c => c.createdat).Select(c => new { c.id, c.createdat, username = c.User!.name }).ToList();
            if (Likes == null || Likes.Count == 0)
            {
                return NotFound();
            }
            return Ok(Likes);
        }

        [Authorize]
        [HttpPost("LikePost/{PostId}")]
        public async Task<IActionResult> LikePost(int PostId)
        {
            var useridString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(useridString, out int userid))
            {
                return Unauthorized(new { message = "Debes iniciar sesión para dar like a este post..." });
            }
            var existinglike = await _dbContext.likes.FirstOrDefaultAsync(l => l.userid == userid && l.postid == PostId && l.commentid == null);
            if (existinglike != null)
                return BadRequest(new { message = "Ya diste like a este post..." });

            var like = new Like { userid = userid, postid = PostId };
            _dbContext.likes.Add(like);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Like agregado al post..." });
        }

        [Authorize]
        [HttpPost("LikeComment/{ComentId}")]
        public async Task<IActionResult> LikeComment(int ComentId)
        {
            var useridString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existinglike = await _dbContext.likes.FirstOrDefaultAsync(l => l.userid == int.Parse(useridString!) && l.commentid == ComentId && l.postid == null);
            if (existinglike != null)
                return BadRequest(new { message = "Ya diste like a este comentario..." });

            var like = new Like { userid = int.Parse(useridString!), commentid = ComentId };
            _dbContext.likes.Add(like);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Like agregado al comentario..." });
        }

        [Authorize]
        [HttpDelete("UnlikePost/{PostId}")]
        public async Task<IActionResult> UnlikePost(int PostId)
        {
            var useridString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var Like = await _dbContext.likes.FirstOrDefaultAsync(l => l.userid == int.Parse(useridString!) && l.postid == PostId && l.commentid == null);
            _dbContext.likes.Remove(Like!);
            await _dbContext.SaveChangesAsync();
            return Ok(new {message = "Like eliminado del post..."});
        }

        [Authorize]
        [HttpDelete("UnlikeComent/{ComentId}")]
        public async Task<IActionResult> UnlikeComent(int ComentId)
        {
            var useridString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var Like = await _dbContext.likes.FirstOrDefaultAsync(l => l.userid == int.Parse(useridString!) && l.commentid == ComentId && l.postid == null);
            _dbContext.likes.Remove(Like!);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Like eliminado del comentario..." });
        }
    }
}
