using Qwirkle.Domain.Enums;

namespace Qwirkle.Web.Api.ViewModels;

public class TileViewModel
{
    public int PlayerId { get; set; }
    public TileColor Color { get; set; }
    public TileShape Shape { get; set; }
    public sbyte X { get; set; }
    public sbyte Y { get; set; }
}
