namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WindowManager;

public class AreaDimension
{
    public int Width { get; }
    public int Height { get; }

    public AreaDimension(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public WindowOrientation Orientation => Width > Height ? WindowOrientation.Landscape : WindowOrientation.Portrait;

    public AreaDimension ReducedByWidth(int width) => new(Width - width, Height);
    public AreaDimension ReducedByHeight(int height) => new(Width, Height - height);
    public AreaDimension ReducedByArea(AreaDimension area) => new(Width - area.Width, Height - area.Height);
}