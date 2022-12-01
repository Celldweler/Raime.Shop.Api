using Shop.Domain.Dto.Chat;
using Shop.Domain.Entities.ChatEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.EntitiesMapper
{
    public class MessageMapper
    {
        // Message to MessageVm
        public MessageViewModel MapToViewModel(Message message)
        {
            return new MessageViewModel
            {
                Id = message.Id,
                Content = message.Content,
                Created = message.Created.ToString("HH:mm MM.dd.yyyy"),
                ProductId = message.Chat.ProductId,
                UserName = message.UserName,
                ChatRoomName = message.Chat.Name,
                ChatId = message.Chat.Id,
            };
        }

        // MessageVm to Message
        public Message Map(MessageViewModel message)
        {
            return new Message
            {

            };
        }
    }
}
