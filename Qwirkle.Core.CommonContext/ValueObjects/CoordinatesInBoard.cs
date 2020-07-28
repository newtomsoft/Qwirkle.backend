namespace Qwirkle.Core.CommonContext.ValueObjects
{
    using Abscissa = System.SByte;
    using Ordinate = System.SByte;
    public struct CoordinatesInBoard
    {
        public Abscissa X { get; }
        public Ordinate Y { get; }

        public CoordinatesInBoard(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        public CoordinatesInBoard Right() => new CoordinatesInBoard((Abscissa)(X + 1), Y);
        public CoordinatesInBoard Left() => new CoordinatesInBoard((Abscissa)(X - 1), Y);
        public CoordinatesInBoard Top() => new CoordinatesInBoard(X, (Ordinate)(Y + 1));
        public CoordinatesInBoard Bottom() => new CoordinatesInBoard(X, (Ordinate)(Y - 1));


        public static bool operator ==(CoordinatesInBoard c1, CoordinatesInBoard c2) => c1.Equals(c2);
        public static bool operator !=(CoordinatesInBoard c1, CoordinatesInBoard c2) => !c1.Equals(c2);
    }
}
