using HtmlAgilityPack;
using PuppeteerSharp;

Console.WriteLine("scraping program");

const string gamePageUrl = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
//var web = new HtmlWeb();
//var doc = web.Load(gamePageUrl);

//< img src = "gamegfx/p4.png" class= "ui-draggable ui-draggable-handle" style = "position: relative;" >
//class="ui-droppable"
//var scorePlayer = int.Parse(doc.GetElementbyId("score_player").InnerText);
//var scoreComputer = int.Parse(doc.GetElementbyId("score_computer").InnerText);

//var process = new Process();
//process.StartInfo.UseShellExecute = true;
//process.StartInfo.FileName = url;
//process.Start();

//Process.Start(url);
//Process.Start("nppp\\notepad++.exe", "args");

//var uri = "https://www.google.com";
//var psi = new System.Diagnostics.ProcessStartInfo();
//psi.UseShellExecute = true;
//psi.FileName = uri;
//System.Diagnostics.Process.Start(psi);




await new BrowserFetcher(Product.Firefox).DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = false,
    Product = Product.Firefox
});
await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.ultraboardgames.com/qwirkle/game.php?startcomputer");

//await page.WaitForSelectorAsync("score_player");
var scorePlayer = await page.SelectAsync("score_player");
//await page.ScreenshotAsync("./google.png");

var links = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
var urls = await page.EvaluateExpressionAsync<string[]>(links);


List<string> programmerLinks = urls.ToList();


var toto = 69;