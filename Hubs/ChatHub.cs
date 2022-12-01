using Microsoft.AspNetCore.SignalR;

namespace Raime.Shop.Api.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnetionId() => Context.ConnectionId;
    }
}
