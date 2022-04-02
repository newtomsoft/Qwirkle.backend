namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.DragNDrop;

public class DragNDropManager : IDragNDropManager
{
    public HashSet<DropItem> AllTilesInGame
    {
        get
        {
            var allTiles = new HashSet<DropItem>();
            allTiles.UnionWith(_tilesFixedInBoard);
            allTiles.UnionWith(TilesDroppedOnRack);
            allTiles.UnionWith(TilesDroppedOnBoard);
            allTiles.UnionWith(TilesDroppedOnBag);
            return allTiles;
        }
    }

    public HashSet<DropItem> AllPlayerTiles
    {
        get
        {
            var tiles = new HashSet<DropItem>();
            tiles.UnionWith(TilesDroppedOnRack);
            tiles.UnionWith(TilesDroppedOnBoard);
            tiles.UnionWith(TilesDroppedOnBag);
            return tiles;
        }
    }

    public HashSet<DropItem> TilesDroppedOnBoard { get; } = new();
    public HashSet<DropItem> TilesDroppedOnBag { get; } = new();
    public List<DropItem> TilesDroppedOnRack { get; } = new();

    private readonly HashSet<DropItem> _tilesFixedInBoard = new();
    private readonly List<string> _dropZoneIdentifiersTaken = new();

    public event EventHandler<TileOnBoardEventArgs> TileOnBoardDropped;
    public event EventHandler<TileOnBoardEventArgs> TileOnBoardDragged;

    private const string BagIdentifierPrefix = "bag";
    private const string RackIdentifierPrefix = "rack";
    private const string BoardIdentifierPrefix = "board";
    private const char Separator = '_';

    public DragNDropManager(IAreaManager areaManager)
    {
        TileOnBoardDropped += (source, eventArgs) => areaManager.OnTileOnBoardDropped(source!, eventArgs);
        TileOnBoardDragged += (source, eventArgs) => areaManager.OnTileOnBoardDragged(source!, eventArgs);
    }

    public void Initialize()
    {
        TilesDroppedOnBoard.Clear();
        TilesDroppedOnRack.Clear();
        TilesDroppedOnBag.Clear();
        _tilesFixedInBoard.Clear();
        _dropZoneIdentifiersTaken.Clear();
    }

    public void OnTilesOnBoardPlayed(object source, TilesOnBoardPlayedEventArgs eventArgs) => FixTilesOnBoard(eventArgs.TilesOnBoard);

    public void OnTilesOnRackChanged(object source, TilesOnRackChangedEventArgs eventArgs) => UpdateRack(eventArgs.TilesOnRack);

    public bool IsDisabled(DropItem item)
    {
        var result = _dropZoneIdentifiersTaken.Contains(item.Identifier);
        return result;
    }

    public bool IsDroppable(string identifier) => _dropZoneIdentifiersTaken.All(x => x != identifier) && AllTilesInGame.All(x => x.Identifier != identifier);

    public void ItemDropped(MudItemDropInfo<DropItem> mudItemDropInfo)
    {
        var (dropItem, dropZoneIdentifier) = mudItemDropInfo;
        var fromDropZone = dropItem.DropZone;
        dropItem.Identifier = dropZoneIdentifier;
        if (fromDropZone == DropZone.Board) OnTileOnBoardDragged(dropItem.Coordinate);
        dropItem.DropZone = DropInZone(dropZoneIdentifier);
        switch (dropItem.DropZone)
        {
            case DropZone.Board:
                TilesDroppedOnBoard.Add(dropItem);
                TilesDroppedOnRack.RemoveAll(t => t.Tile == dropItem.Tile);
                TilesDroppedOnBag.RemoveWhere(t => t.Tile == dropItem.Tile);
                dropItem.Coordinate = ToCoordinate(dropItem.Identifier);
                OnTileOnBoardDropped(dropItem.Coordinate);
                return;
            case DropZone.Rack:
                TilesDroppedOnRack.RemoveAll(t => t.Tile == dropItem.Tile);
                var rackPositionTo = ToRackPosition(dropItem.Identifier);
                SwapRackPosition(dropItem.RackPosition, rackPositionTo);
                dropItem.RackPosition = rackPositionTo;
                TilesDroppedOnRack.Add(dropItem);
                TilesDroppedOnBoard.RemoveWhere(t => t.Tile == dropItem.Tile);
                TilesDroppedOnBag.RemoveWhere(t => t.Tile == dropItem.Tile);
                return;
            case DropZone.Bag:
                TilesDroppedOnBag.Add(dropItem);
                TilesDroppedOnRack.RemoveAll(t => t.Tile == dropItem.Tile);
                TilesDroppedOnBoard.RemoveWhere(t => t.Tile == dropItem.Tile);
                return;
            case DropZone.Undefined:
            default:
                throw new ArgumentOutOfRangeException(nameof(mudItemDropInfo));
        }
    }

    private void SwapRackPosition(byte rackPositionFrom, byte rackPositionTo)
    {
        var item = AllTilesInGame.FirstOrDefault(t => t.RackPosition == rackPositionTo);
        if (item is null) return;
        item.RackPosition = rackPositionFrom;
    }

    public string ToBoardIdentifier(Coordinate coordinate) => $"{BoardIdentifierPrefix}{Separator}{coordinate.X}{Separator}{coordinate.Y}";
    public string ToRackIdentifier(int rackIndex) => $"{RackIdentifierPrefix}{Separator}{rackIndex}";
    public string ToBagIdentifier(int bagIndex) => $"{BagIdentifierPrefix}{Separator}{bagIndex}";

    private void FixTilesOnBoard(IEnumerable<TileOnBoard> tilesOnBoard)
    {
        var addedTiles = tilesOnBoard.ToHashSet();
        var tilesAtSamePlaceThanAddedTiles = AllTilesInGame.Where(t => addedTiles.Select(t2 => t2.Coordinate).Contains(t.Coordinate)).ToHashSet();
        TilesDroppedOnBoard.ExceptWith(tilesAtSamePlaceThanAddedTiles);
        foreach (var tile in addedTiles)
        {
            var identifier = ToBoardIdentifier(tile.Coordinate);
            _tilesFixedInBoard.Add(new DropItem { Tile = tile, Identifier = identifier, Coordinate = tile.Coordinate, DropZone = DropZone.Board });
            _dropZoneIdentifiersTaken.Add(identifier);
        }
    }

    private void UpdateRack(IEnumerable<TileOnRack> tilesOnRack)
    {
        TilesDroppedOnBag.Clear();
        TilesDroppedOnRack.Clear();
        foreach (var tile in tilesOnRack)
        {
            var droppedTile = new DropItem { Tile = tile, Identifier = ToRackIdentifier(tile.RackPosition), DropZone = DropZone.Rack, RackPosition = tile.RackPosition };
            AllTilesInGame.Add(droppedTile);
            TilesDroppedOnRack.Add(droppedTile);
        }
    }

    private static Coordinate ToCoordinate(string dropItemIdentifier)
    {
        var spited = dropItemIdentifier.Split(Separator);
        var x = int.Parse(spited[1]);
        var y = int.Parse(spited[2]);
        return Coordinate.From(x, y);
    }

    private static RackPosition ToRackPosition(string dropItemIdentifier)
    {
        var spited = dropItemIdentifier.Split(Separator);
        return (RackPosition)int.Parse(spited[1]);
    }

    private static DropZone DropInZone(string dropZoneIdentifier)
    {
        if (dropZoneIdentifier.StartsWith(BoardIdentifierPrefix)) return DropZone.Board;
        if (dropZoneIdentifier.StartsWith(RackIdentifierPrefix)) return DropZone.Rack;
        if (dropZoneIdentifier.StartsWith(BagIdentifierPrefix)) return DropZone.Bag;
        return DropZone.Undefined;
    }

    private void OnTileOnBoardDropped(Coordinate coordinate) => TileOnBoardDropped.Invoke(this, new TileOnBoardEventArgs(coordinate));
    private void OnTileOnBoardDragged(Coordinate coordinate) => TileOnBoardDragged.Invoke(this, new TileOnBoardEventArgs(coordinate));
}