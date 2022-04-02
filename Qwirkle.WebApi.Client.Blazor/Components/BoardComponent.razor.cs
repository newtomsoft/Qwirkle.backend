namespace Qwirkle.WebApi.Client.Blazor.Components;

public partial class BoardComponent : ComponentBase
{
    [Parameter] public IDragNDropManager DragNDropManager { get; set; } = default!;
    [Parameter] public IAreaManager AreaManager { get; set; } = default!;
}
