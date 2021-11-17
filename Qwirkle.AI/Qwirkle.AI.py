from Color import Color
from Bag import Bag
from Shape import Shape
from Tile import Tile
from Coordinates import Coordinates
from TileOnBag import TileOnBag
from TileOnBoard import TileOnBoard
from TileOnPlayer import TileOnPlayer
from Board import Board
import requests
import json
from types import SimpleNamespace

def searchCoordinates(xmin, xmax, ymin, ymax):
    coordinates = []
    for x in range(xmin-1, xmax+1+1):
        for y in range(ymin-1, ymax+1+1):
            coordinates.append(Coordinates(x, y))
    return coordinates

host = 'https://localhost:5001'
logout = '/User/Logout'
login = '/User/Login'
userGamesIds = '/Game/UserGamesIds'
game = '/Game/'
playTilesSimulation = '/Action/PlayTilesSimulation'

url_logout = f'{host}{logout}'
url_login = f'{host}{login}'
url_userGamesIds = f'{host}{userGamesIds}'
url_game = f'{host}{game}'
url_playTilesSimulation = f'{host}{playTilesSimulation}'

response_logout = requests.get(url_logout, verify=False)

param_login = {'pseudo': 'bot1', 'password': 'bot1password', 'isRemember': True}
response_login = requests.post(url_login, json=param_login, verify=False)
if response_login.text == 'false':
    quit()

cookies = response_login.cookies
response_gamesIds = requests.get(url_userGamesIds, cookies=cookies, verify=False)
if response_gamesIds.status_code != 200:
    quit()

gamesNumbers = json.loads(response_gamesIds.text)

#todo foreach sur gamesNumbers
gameNumber = gamesNumbers[0]
print(f'game number : {gameNumber}')
response_game = requests.get(f'{url_game}{gameNumber}', cookies=cookies, verify=False)
if response_game.status_code != 200:
    quit()

game = json.loads(response_game.text, object_hook=lambda d: SimpleNamespace(**d))
tilesOnBoard = [TileOnBoard(tile) for tile in game.board.tiles]
tilesOnBag = [TileOnBag(tile) for tile in game.bag.tiles]

gameOver = game.gameOver
if gameOver == True:
    quit()

#todo best get tilesOnPlayer
for player in game.players:
    if player.rack.tiles != None:
        tilesOnPlayer =   [TileOnPlayer(tile) for tile in player.rack.tiles]

board = Board(tilesOnBoard)


searchCoordinates = board.searchCoordinates()


moves = []
moveNumber = 0
for tile in tilesOnPlayer:
    for coordinates in searchCoordinates:
        moveNumber += 1
        #todo using playerId get by new api Player/ByGameId
        param_play = [{'playerId': 51, 'tileId': tile.Id, 'x': coordinates.X, 'y':coordinates.Y}]
        response_playSimulation = requests.post(f'{url_playTilesSimulation}', json=param_play, cookies=cookies, verify=False)
        points = json.loads(response_playSimulation.text, object_hook=lambda d: SimpleNamespace(**d)).points
        if points > 0:
            moves.append((coordinates, tile.Id, points))


breakpoint=0



