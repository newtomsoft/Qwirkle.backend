using System.Text.RegularExpressions;

namespace Qwirkle.UltraBoardGames.Player.WebElementExtensions;

public static class WebElementExtensions
{
    public static int TextToInt(this IWebElement element, int index = 0)
    {
        var toConvert = element.Text;
        var numbers = Regex.Split(toConvert, @"\D+");
        return int.Parse(numbers[index]);
        // toConvert = "10 (4 last turn)" index = 0 return 10 ; index = 1 return 4
    }
}