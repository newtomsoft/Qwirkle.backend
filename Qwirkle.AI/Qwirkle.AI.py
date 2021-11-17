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

url_logout = 'https://localhost:5001/User/Loginout'
url_login = 'https://localhost:5001/User/Login'
url_userGames = 'https://localhost:5001/Game/UserGames'
url_game = 'https://localhost:5001/Game/'

response_logout = requests.get(url_logout, verify=False)

param_login = {'pseudo': 'JC', 'password': 'qwirkle', 'isRemember': True}
response_login = requests.post(url_login, json=param_login, verify=False)
if response_login.text == 'false' :
    quit()

cookies = response_login.cookies
response_gamesIds = requests.get(url_userGames, cookies=cookies, verify=False)
if response_gamesIds.status_code != 200 :
    quit()

gamesNumbers = json.loads(response_gamesIds.text)

#todo foreach sur gamesNumbers
gameNumber = gamesNumbers[0]
print(f'game number : {gameNumber}')
response_game = requests.get(f'{url_game}{gameNumber}', cookies=cookies, verify=False)
if response_game.status_code != 200 :
    quit()

game = json.loads(response_game.text, object_hook=lambda d: SimpleNamespace(**d))
tilesOnBoard = [TileOnBoard(tile) for tile in game.board.tiles]
tilesOnBag = [TileOnBag(tile) for tile in game.bag.tiles]

gameOver = game.gameOver
if gameOver == True :
    quit()

#todo best get tilesOnPlayer
for player in game.players:
    if player.rack.tiles != None:
        goodTilesOnPlayer =   [TileOnPlayer(tile) for tile in player.rack.tiles]

board = Board(tilesOnBoard)

xmin = board.xMin()
xmax = board.xMax()
ymin = board.yMin()
ymax = board.yMax()

breakpoint=0