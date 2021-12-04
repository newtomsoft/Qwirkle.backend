using System.Diagnostics;
using HtmlAgilityPack;

Console.WriteLine("scraping program");

var url = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
var web = new HtmlWeb();
var doc = web.Load(url);

//< img src = "gamegfx/p4.png" class= "ui-draggable ui-draggable-handle" style = "position: relative;" >
//class="ui-droppable"
var scorePlayer = int.Parse(doc.GetElementbyId("score_player").InnerText);
var scoreComputer = int.Parse(doc.GetElementbyId("score_computer").InnerText);


Process.Start("nppp\\notepad++.exe", "args");

//var uri = "https://www.google.com";
//var psi = new System.Diagnostics.ProcessStartInfo();
//psi.UseShellExecute = true;
//psi.FileName = uri;
//System.Diagnostics.Process.Start(psi);

var toto = 69;