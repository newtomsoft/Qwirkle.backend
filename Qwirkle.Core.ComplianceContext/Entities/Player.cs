namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public byte BoardPosition { get; set; }
        public bool IsTurn { get; set; }
        public byte Points { get; set; }
    }
}
