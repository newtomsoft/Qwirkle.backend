using System;

namespace Qwikle.SignalR
{
    public struct Player
    {
        public string ConnectionId { get; set; }
        public string Pseudo { get; set; }
        public bool Connected { get; set; }

        public Player(string connectionId, string pseudo, bool connected = true)
        {
            ConnectionId = connectionId;
            Pseudo = pseudo;
            Connected = connected;
        }

        public bool Equals(Player player) => ConnectionId == player.ConnectionId;

        public override int GetHashCode() => HashCode.Combine(ConnectionId);
    }
}
