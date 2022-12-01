using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Raime.Shop.Api.EntitiesMapper;
using Raime.Shop.Api.Hubs;
using Shop.DAL;
using Shop.Domain.Dto.Chat;
using Shop.Domain.Entities.ChatEntities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [Route("chats")]
    [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
    public class ChatController : ApiController
    {
        private ApplicationDbContext _ctx;
        private IHubContext<ChatHub> _hubContext;
        private UserManager<IdentityUser> _userManager;
        private readonly MessageMapper mapper;

        public ChatController(ApplicationDbContext ctx,
            IHubContext<ChatHub> hubContext,
            UserManager<IdentityUser> userManager,
            MessageMapper _mapper)
        {
            _ctx = ctx;
            _hubContext = hubContext;
            _userManager = userManager;
            mapper = _mapper;
        }

        [HttpGet("")]
        public IActionResult GetAllChatsForCurrentUser()
        {
            var chats = _ctx.ChatUsers
                .Include(y => y.Customer)
                .Include(x => x.Chat)
                .ThenInclude(m => m.Messages)
                .Where(x => x.CustomerId == UserId)
                .Select(c => new ChatDto
                {
                    Id = c.ChatId,
                    Name = c.Chat.Name,
                    Created = c.Chat.Created.ToString("MM.dd.yyyy"),
                    AdminShopName = "Raime Shop Admin",
                    Messages = c.Chat.Messages.OrderBy(x => x.Created).Select(m => new MessageViewModel
                    {
                        Id = m.Id,
                        Content = m.Content,
                        Created = m.Created.ToString("HH:mm MM.dd.yyyy"),
                        ProductId = m.Chat.ProductId,
                        UserName = m.UserName,
                        ChatRoomName = m.Chat.Name,
                        ChatId = m.Chat.Id,
                    }).ToList(),
                    Users = c.Chat.Users,
                    ProductId = c.Chat.ProductId,
                })
                .ToList();

            return Ok(chats);
        }

        [HttpGet("{chatId}")]
        public IActionResult GetAllMessagesFromChat(int chatId)
        {
            var messages = _ctx.Messages.Where(x => x.ChatId == chatId).ToList();

            return Ok(messages);
        }
        public class CreateMessageRequest
        {
            public string Content { get; set; }
            public string ChatRoomName { get; set; }
            public int ChatId { get; set; }
            public int ProductId { get; set; }
        }
      
        [HttpPost("createMessage")]
        public async Task<IActionResult> CreateMessageAsync(CreateMessageRequest messageRequest)
        {
            var userName = (await _userManager.FindByIdAsync(UserId)).UserName;
            var newMessage = new Message
            {
                Content = messageRequest.Content,
                Created = DateTime.Now,
                IsEdit = false,
                UserName = userName,
            };
            
            var chatIsAlreadyExist = _ctx.ChatUsers
                .Where(x => x.CustomerId == UserId && x.Chat.Name == messageRequest.ChatRoomName)
                .Select(y => y.Chat)
                .FirstOrDefault();
            //_ctx.Chats.FirstOrDefault(x => x.Name == messageRequest.ChatRoomName);
            if (chatIsAlreadyExist != null)
            {
                // chat already exist you dont need create new chat room
                try {
                    newMessage.ChatId = chatIsAlreadyExist.Id;

                    _ctx.Messages.Add(newMessage);
                    await _ctx.SaveChangesAsync();
                }
                catch (Exception) {
                    throw;
                }

                return Ok(mapper.MapToViewModel(newMessage));
            }
            else
            {
                var newChatRoom = new Chat
                {
                    Name = messageRequest.ChatRoomName,
                    ProductId = messageRequest.ProductId,
                    Created = DateTime.Now,
                };
                newChatRoom.Messages.Add(newMessage);
                try {
                    //newMessage.ChatId = newChatRoom.Id;
                    //await _ctx.SaveChangesAsync();

                    //var user = _ctx.Customers.FirstOrDefault(x => x.Id == UserId);
                    //user.Chats.Add(new ChatUser
                    //{
                    //    UserId = UserId,
                    //    ChatId = newChatRoom.Id,
                    //});
                    newChatRoom.Users.Add(new ChatUser
                    {
                        CustomerId = UserId,
                    });
                   
                    _ctx.Chats.Add(newChatRoom);
                    await _ctx.SaveChangesAsync();
                }
                catch (Exception) {
                    throw;
                }

                return Ok(mapper.MapToViewModel(newMessage));
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChatAsync(string roomName)
        {
            var newChatRoom = new Chat
            {
                Name = roomName,
                Created = DateTime.Now,
            };

            newChatRoom.Users.Add(new ChatUser
            {
                CustomerId = UserId,
            });
            _ctx.Chats.Add(newChatRoom);
            await _ctx.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("join-to-room/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinToRoomAsync(string roomName, string connectionId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, roomName);

            if(roomName == "testChatRoom")
                return Ok("Ты уже находишся в чат комнате 'testChatRoom'");


            var newChatRoom = new Chat
            {
                Name = roomName,
                Created = DateTime.Now,
            };

            newChatRoom.Users.Add(new ChatUser
            {
                CustomerId = UserId,
            });

            _ctx.Chats.Add(newChatRoom);
            await _ctx.SaveChangesAsync();

            return Ok(newChatRoom);
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(MessageRequest message)
        {
            var userName = (await _userManager.FindByIdAsync(UserId)).UserName;

            var _message = new Message
            {
                ChatId = message.ChatId,
                Content = message.Message,
                UserName = userName,
                Created = DateTime.Now,
            };
            try
            {
                _ctx.Messages.Add(_message);
                await _ctx.SaveChangesAsync();
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
            await _hubContext.Clients.Group(message.RoomName).SendAsync("recieveMessage", _message);

            return Ok(_message);
        }
        public class MessageRequest
        {
            public string Message { get; set; }
            public string RoomName { get; set; }
            public int ChatId { get; set; }
        }
    }
}
