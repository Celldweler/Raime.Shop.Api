using Shop.DAL;
using Shop.Domain.Entities.ChatEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.SeedData
{
    public class ChatStore
    {
        private ApplicationDbContext _ctx;

        public List<Message> Messages { get; set; } = new List<Message>();
        public List<Chat> Chat { get; set; } = new List<Chat>();
        public ChatStore(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
    }
}
