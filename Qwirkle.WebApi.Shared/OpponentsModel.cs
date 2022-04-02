#nullable enable
namespace Qwirkle.WebApi.Shared;

[Serializable]
public class OpponentsModel
{
    [Required]
    public string Opponent1 { get; set; }
    public string? Opponent2 { get; set; }
    public string? Opponent3 { get; set; }

    public OpponentsModel() => Opponent1 = string.Empty;

    public OpponentsModel(string opponent1, string? opponent2 = null, string? opponent3 = null)
    {
        Opponent1 = opponent1;
        Opponent2 = opponent2;
        Opponent3 = opponent3;
    }
}
