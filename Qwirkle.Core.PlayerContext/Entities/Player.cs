namespace Qwirkle.Core.PlayerContext.Entities.Player
{
    public class Player
    {
        public int Id { get; }
        public string Pseudo { get; }
        public byte Points { get; }
        public byte BoardPosition { get; }
        public bool IsTurn { get; }
    }
}
