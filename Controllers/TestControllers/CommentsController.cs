using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raime.Shop.Api.Services;
using Shop.DAL;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers.TestControllers
{
    [Route("comments")]
    public class CommentsController : ApiController
    {
        private ApplicationDbContext _ctx;
        private UserManager<IdentityUser> _userManager;

        public CommentsController(ApplicationDbContext ctx, UserManager<IdentityUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public IEnumerable<CommentViewModel> GetCommentsOfProduct(int productId)
        {
            var users = _userManager.Users.Select(x => new { x.Id, x.UserName }).ToArray();
            var product = _ctx.Products.Include(c => c.Comments).ThenInclude(r => r.Replies)
                                       .Where(x => x.ProductId == productId).First();

            return product.Comments
                .OrderByDescending(x => x.Created)
                .Select(x => new CommentViewModel
                {
                    Id = x.CommentId,
                    Content = x.Content,
                    Created = x.Created.ToDateCreatedViewModel(),
                    UserName = users.Where(u => u.Id == x.CustomerId).First().UserName,
                    Replies = x.Replies.Select(y => new ReplyVm
                    {
                        Id = y.Id,
                        Content = y.Content,
                        ParentId = y.ParentId,
                        UserName = users.Where(u => u.Id == y.CustomerId).First().UserName,
                        Created = y.Created.ToDateCreatedViewModel(),
                    }).ToList(),
                })
                .ToArray();
        }

        [HttpGet("{parentId}/replies")]
        public IActionResult GetReplies(int parentId)
        {
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCommentAsync(int id)
        {
            var comment = _ctx.Comments.Where(x => x.CommentId == id).FirstOrDefault();

            if (comment == null)
                return BadRequest();
            _ctx.Comments.Remove(comment);

            await _ctx.SaveChangesAsync();

            return Ok();
        }
        [HttpDelete("replies/{id}")]
        public async Task<IActionResult> Reply(int id)
        {
            var comment = _ctx.Replies.Where(x => x.Id == id).FirstOrDefault();

            if (comment == null)
                return BadRequest();
            _ctx.Replies.Remove(comment);

            await _ctx.SaveChangesAsync();

            return Ok();
        }
        // create comment
        // /comments
        [HttpPost]
        [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateComment comment)
        {
            if(comment == null)
                return BadRequest("request comment is null object");

            Comment newComment = null; 
            try 
            {
                var product = _ctx.Products.FirstOrDefault(x => x.ProductId.Equals(comment.ProductId));
                newComment = new Comment 
                {
                    Content = comment.Content,
                    Created = DateTime.Now,
                    ProductId = product.ProductId,
                    CustomerId = UserId,
                };
                product.Comments.Add(newComment);
                await _ctx.SaveChangesAsync();
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
            return Ok(new CommentViewModel
            {
                Id= newComment.CommentId,
                Content = newComment.Content,
                Created = newComment.Created.ToDateCreatedViewModel(),

            });
        }

        
        [HttpPost("{parentCommentId}/replies")]
        [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
        public async Task<IActionResult> ReplyToAsync(int parentCommentId, [FromBody] CreateReply reply)
        {
            var newReply = new Reply
            {
                Content= reply.Content,
                Created = DateTime.Now,  
                CustomerId= UserId,
            };
            var comment = _ctx.Comments.FirstOrDefault(x => x.CommentId == parentCommentId);

            if (comment == null)
                return BadRequest();

            comment.Replies.Add(newReply);
            await _ctx.SaveChangesAsync();

            return Ok(new ReplyVm
            {
                Id = newReply.Id,
                ParentId = newReply.ParentId,
                Content = newReply.Content,
                UserName = UserName,
                Created = newReply.Created.ToDateCreatedViewModel(),
            });
        }
    }
  
    public class CreateComment
    {
        public string Content { get; set; }
        public int ProductId { get; set; }
    }
    public class CreateReply
    {
        public string Content { get; set; }
    }
    public class ReplyVm
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string UserName { get; set; }
        public string Created { get; set; }
        public string Content { get; set; }
    }
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int ParentId { get; set; }
        public string Created { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public string CustomerId { get; set; }
        public List<ReplyVm> Replies { get; set; } = new List<ReplyVm>();
    }
}
