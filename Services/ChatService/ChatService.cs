using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.MainModels.ChatModels;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PayBridgeAPI.Services.ChatService
{
    public class ChatService : Hub, IChatService
    {
        private readonly ConcurrentDictionary<string, UserConnection> _connections;
        private readonly PayBridgeDbContext _context;

        public ChatService(ConcurrentDictionary<string, UserConnection> connections, PayBridgeDbContext context)
        {
            _connections = connections;
            _context = context;
        }

        public async Task CreateChatRoom(ChatRoom chatRoom)
        {
            await _context.AddAsync(chatRoom);
            await _context.SaveChangesAsync();
        }

        public async Task EditMessage(ChatLine chatLine)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out _))
            {
                _context.Update(chatLine);
                await _context.SaveChangesAsync();
            }
        }

        public async Task JoinChatRoom(UserConnection userConnection)
        {
                await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);
                userConnection.ChatLines = await _context.ChatLines.Where(u => u.ChatRoom.ChatName == userConnection.ChatRoom).ToListAsync();
                _connections[Context.ConnectionId] = userConnection;
                await Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveMessage", "Chat Bot", $"{userConnection.UserName} приєднався до чату {userConnection.ChatRoom}", userConnection.ChatLines);
        }


        public async Task SendMessage(ChatLine chatLine)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveMessage", userConnection.UserName, chatLine.TextMessage);
                await _context.AddAsync(chatLine);
                await _context.SaveChangesAsync();
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId, out _);
                Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveMessage", "Chat Bot", $"{userConnection.UserName} вийшов з чату.");
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
