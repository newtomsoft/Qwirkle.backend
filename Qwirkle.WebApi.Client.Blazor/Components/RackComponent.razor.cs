namespace Qwirkle.WebApi.Client.Blazor.Components;

public partial class RackComponent : ComponentBase
{
    [Parameter] public IDragNDropManager DragNDropManager { get; set; } = default!;
}
