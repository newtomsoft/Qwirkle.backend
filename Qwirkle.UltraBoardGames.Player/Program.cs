using Qwirkle.UltraBoardGames.Player;

Console.WriteLine("scraping program");

var scraper = new Scraper();
var playerPoints = scraper.GetPlayerPoints();
var opponentPoints = scraper.GetOpponentPoints();
var tilesOnBag = scraper.GetTilesOnBag();
var tilesOnPlayer = scraper.GetTilesOnPlayer();
var tilesOnBoard = scraper.GetTilesOnBoard();


var toto = 69;