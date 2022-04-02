namespace Qwirkle.WebApi.Client.Blazor.Components;

public partial class BagComponent : ComponentBase
{
    [Parameter] public IDragNDropManager DragNDropManager { get; set; } = default!;
}
