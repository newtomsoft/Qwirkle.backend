namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.DragNDrop;

public interface IDragNDropManager
{
    void OnTilesOnBoardPlayed(object source, TilesOnBoardPlayedEventArgs eventArgs);
    void OnTilesOnRackChanged(object source, TilesOnRackChangedEventArgs eventArgs);

    void Initialize();

    public string ToBoardIdentifier(Coordinate coordinate);
    public string ToRackIdentifier(int rackIndex);
    public string ToBagIdentifier(int bagIndex);
    HashSet<DropItem> AllTilesInGame { get; }
    HashSet<DropItem> AllPlayerTiles { get; }
    HashSet<DropItem> TilesDroppedOnBoard { get; }
    HashSet<DropItem> TilesDroppedOnBag { get; }
    List<DropItem> TilesDroppedOnRack { get; }
    bool IsDisabled(DropItem item);
    bool IsDroppable(string identifier);
    void ItemDropped(MudItemDropInfo<DropItem> mudItemDropInfo);
}