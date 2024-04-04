using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.MainModels.ChatModels;

namespace PayBridgeAPI.Services.ChatService
{
    public interface IChatService
    {
        Task CreateChatRoom(ChatRoom chatRoom);
        Task JoinChatRoom(UserConnection connection);
        Task SendMessage(ChatLine chatLine);
        Task EditMessage(ChatLine chatLine);
    }
}
