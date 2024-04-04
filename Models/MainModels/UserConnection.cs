using PayBridgeAPI.Models.MainModels.ChatModels;

namespace PayBridgeAPI.Models.MainModels
{
    public class UserConnection
    {
        public string UserName { get; set; }
        public string ChatRoom { get; set; }
        public List<ChatLine> ChatLines { get; set; } = new();
    }
}
