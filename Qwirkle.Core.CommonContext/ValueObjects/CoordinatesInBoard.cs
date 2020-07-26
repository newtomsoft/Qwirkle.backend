namespace Qwirkle.Core.CommonContext.ValueObjects
{
    public struct CoordinatesInBoard
    {
        public sbyte X { get; }
        public sbyte Y { get; }

        public CoordinatesInBoard(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        public CoordinatesInBoard Right() => new CoordinatesInBoard((sbyte)(X + 1), Y);
        public CoordinatesInBoard Left() => new CoordinatesInBoard((sbyte)(X - 1), Y);
        public CoordinatesInBoard Top() => new CoordinatesInBoard(X, (sbyte)(Y + 1));
        public CoordinatesInBoard Bottom() => new CoordinatesInBoard(X, (sbyte)(Y - 1));


        public static bool operator ==(CoordinatesInBoard c1, CoordinatesInBoard c2) => c1.Equals(c2);
        public static bool operator !=(CoordinatesInBoard c1, CoordinatesInBoard c2) => !c1.Equals(c2);
    }
}
