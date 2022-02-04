namespace Qwirkle.UltraBoardGames.Player.WebElementExtensions;

public static class WebElementExtensions
{
    private const char DefaultIgnoreCharacter = ' ';
    public static int TextToInt(this IWebElement element, char ignoreAfterCharacter = DefaultIgnoreCharacter)
    {
        var toConvert = element.Text;
        var indexToRemoveAfter = toConvert.IndexOf(ignoreAfterCharacter);
        var valueString = indexToRemoveAfter > 0 ? toConvert.Remove(indexToRemoveAfter) : toConvert;
        return int.Parse(valueString);
        // toConvert = "10 (4 last turn)" -> return 10
    }


}