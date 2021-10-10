using System;

namespace Qwikle.SignalR
{
    public record Player
    {
        public int PlayerId { get; set; }
        public string ConnectionId { get; set; }

        public Player(string connectionId, int id)
        {
            PlayerId = id;
            ConnectionId = connectionId;
        }
    }
}
