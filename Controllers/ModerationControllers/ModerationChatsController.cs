using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raime.Shop.Api.Services;
using Shop.DAL;
using Shop.Domain.Dto.Chat;
using Shop.Domain.Entities.ChatEntities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers.ModerationControllers
{
    [Route("admin/chats")]
    [Authorize(Policy = "mod")]
    public class ModerationChatsController : ApiController
    {
        private ApplicationDbContext _ctx;

        public ModerationChatsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        } 

        [HttpGet]
        public IActionResult GetAllChats()
        {
            var chats = _ctx.Chats
               .Include(m => m.Messages)
               .Include(c => c.Users)
               .ThenInclude(p => p.Customer)
               .Select(c => new ChatAdminDto
               {
                   Id = c.Id,
                   Name = c.Name,
                   CustomerName = c.Users.FirstOrDefault(u => u.CustomerId != UserId && c.Id == u.ChatId).Customer.ToCustomerFullName(),
                   Created = c.Created.ToString("MM.dd.yyyy"),
                   Messages = c.Messages.OrderBy(x => x.Created).Select(m => new MessageViewModel
                   {
                       Id = m.Id,
                       Content = m.Content,
                       Created = m.Created.ToString("HH:mm MM.dd.yyyy"),
                       ProductId = m.Chat.ProductId,
                       UserName = m.UserName,
                       ChatRoomName = m.Chat.Name,
                       ChatId = m.Chat.Id,
                   }).ToList(),
                   //Users = c.Users,
                   ProductId = c.ProductId,
               })
               .ToList();

            return Ok(chats);
        }

        [HttpGet("/messages/{chatId}")]
        public IActionResult GetMessagesFromChat(int chatId)
        {
            var chat = _ctx.Chats.Include(x => x.Messages).FirstOrDefault(x => x.Id == chatId);

            return Ok(chat.Messages);
        }


        [HttpPost("joinToChat/{chatRoomName}")]
        public async Task<IActionResult> JoinToChatWithCustomerQuestinsAsync(string chatRoomName)
        {
            var chat = _ctx.Chats.FirstOrDefault(x => x.Name.Equals(chatRoomName));

            if (chat == null) return BadRequest("Чата не существует");

            var adminIsAlreeadyInThisChat = chat.Users.FirstOrDefault(x => x.CustomerId == UserId);
            if(adminIsAlreeadyInThisChat != null)
            {
                return Ok("Admin already exist in this chat room");
            }

            try
            {
                _ctx.ChatUsers.Add(new ChatUser
                {
                    CustomerId = base.UserId,
                    ChatId = chat.Id
                });

                await _ctx.SaveChangesAsync();
            }
            catch (Exception) {
                throw;
            }

            return Ok($"Сотрудник магазина Raime Store вступил в чат {chatRoomName}");
        }

        public class ReplyRequest
        {
            public string Text { get; set; }
            public int ChatId { get; set; }
        }

        [HttpPost("replyCustomer")]
        public async Task<IActionResult> ReplyToCustomerMessage(ReplyRequest replyRequest)
        {
            var adminIsAlreadyExistInChatRoom = _ctx.ChatUsers
                .Include(c => c.Chat)
                .FirstOrDefault(x => x.CustomerId == UserId);

            if(adminIsAlreadyExistInChatRoom == null)
            {
                var chat = _ctx.Chats.FirstOrDefault(x => x.Id == replyRequest.ChatId);
                chat.Users.Add(new ChatUser
                {
                    CustomerId = UserId,
                });
                await _ctx.SaveChangesAsync();
            }

            var reply = new Message
            {
                Content = replyRequest.Text,
                Created = DateTime.Now,
                UserName = "Администратор",
                ChatId = replyRequest.ChatId,
            };

            _ctx.Messages.Add(reply);
            await _ctx.SaveChangesAsync();

            return Ok(new
            {
                id = reply.Id,
                Content = reply.Content,
                Created = reply.Created.ToString(),
                userName = reply.UserName,
                chatId = reply.ChatId
            });
        }
    }
}
