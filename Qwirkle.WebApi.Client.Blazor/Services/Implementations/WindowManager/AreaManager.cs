namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WindowManager;

public class AreaManager : IAreaManager
{
    public BoardLimit BoardLimit { get; }
    public string BoardSquareSize { get; private set; } = "height:150px; width:150px;"; //Todo

    private readonly IJSRuntime _jsRuntime;
    private readonly AreaDimension _menuDimension = new(200, 56); //todo use it for menu
    private readonly AreaDimension _commandArea = new(200, 200); //todo use it for command

    public AreaManager(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        BoardLimit = new BoardLimit(new[] { Coordinate.From(0, 0) });
    }

    public async Task OnTilesOnBoardPlayed(object source, TilesOnBoardPlayedEventArgs eventArgs)
    {
        Console.WriteLine("AreaManager: tiles On Board changed");
        var coordinatesInBoard = new List<Coordinate>(eventArgs.TilesOnBoard.Select(t => t.Coordinate));
        BoardLimit.Enlarge(coordinatesInBoard);
        await UpdateBoardSquareSizeAsync();
    }
    public async Task OnTileOnBoardDropped(object sender, TileOnBoardEventArgs eventArgs)
    {
        Console.WriteLine("AreaManager: tile dropped on board");
        BoardLimit.Enlarge(eventArgs.Coordinate);
        await UpdateBoardSquareSizeAsync();
    }

    public async Task OnTileOnBoardDragged(object sender, TileOnBoardEventArgs eventArgs)
    {
        Console.WriteLine("AreaManager: tile dragged out board");
        BoardLimit.Reduce(eventArgs.Coordinate);
        await UpdateBoardSquareSizeAsync();
    }

    private async Task UpdateBoardSquareSizeAsync()
    {
        var dimension = await GetSquareDimensionAsync(BoardLimit);
        BoardSquareSize = $"height:{dimension.Height}px; width:{dimension.Width}px;";
    }

    private async Task<AreaDimension> GetSquareDimensionAsync(BoardLimit boardLimit)
    {
        var boardDimension = await ComputeBoardDimensionAsync();
        var columnNumber = boardLimit.ColumnNumber;
        var lineNumber = boardLimit.LinesNumber;
        var maxWidth = boardDimension.Width / columnNumber;
        var maxHeight = boardDimension.Height / lineNumber;
        var widthHeight = Math.Min(maxWidth, maxHeight);
        return new AreaDimension(widthHeight, widthHeight);
    }

    private async Task<AreaDimension> ComputeBoardDimensionAsync()
    {
        var windowDimension = await GetWindowDimensionAsync();
        return windowDimension.Orientation == WindowOrientation.Portrait
            ? windowDimension.ReducedByHeight(_menuDimension.Height + _commandArea.Height)
            : windowDimension.ReducedByWidth(_menuDimension.Width + _commandArea.Width);
    }

    private async Task<AreaDimension> GetWindowDimensionAsync() => await _jsRuntime.InvokeAsync<AreaDimension>("getWindowDimensions");
}