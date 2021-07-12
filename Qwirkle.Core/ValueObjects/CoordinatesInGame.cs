namespace Qwirkle.Core.ValueObjects
{
    using Abscissa = System.SByte;
    using Ordinate = System.SByte;
    public record CoordinatesInGame
    {
        public Abscissa X { get; }
        public Ordinate Y { get; }

        public CoordinatesInGame(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        public CoordinatesInGame Right() => new CoordinatesInGame((Abscissa)(X + 1), Y);
        public CoordinatesInGame Left() => new CoordinatesInGame((Abscissa)(X - 1), Y);
        public CoordinatesInGame Top() => new CoordinatesInGame(X, (Ordinate)(Y + 1));
        public CoordinatesInGame Bottom() => new CoordinatesInGame(X, (Ordinate)(Y - 1));
    }
}
