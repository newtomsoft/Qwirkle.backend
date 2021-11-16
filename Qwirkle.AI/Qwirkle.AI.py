from Color import Color
from Shape import Shape
from Tile import Tile
from Coordinates import Coordinates
from TileOnBoard import TileOnBoard
from Board import Board
import requests
import json

#tileOnBoard1 = TileOnBoard(Tile(1, Shape.Circle, Color.Purple), Coordinates(0, 0))
#tileOnBoard2 = TileOnBoard(Tile(2, Shape.Circle, Color.Purple), Coordinates(0, 1))
#tileOnBoard3 = TileOnBoard(Tile(3, Shape.EightPointStar, Color.Green), Coordinates(1, 0))
#tilesOnBoard = [tileOnBoard1, tileOnBoard2, tileOnBoard3]
#board = Board(tilesOnBoard)

#for tile in board.Tiles:
#    print(f'{tile.Color} {tile.Shape} {tile.Coordinates.X} {tile.Coordinates.Y}')


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

game = json.loads(response_game.text)

gameOver = game['gameOver']
if gameOver == True :
    quit()

board = Board(game['board']['tiles'])
tilesOnBoard = (game['board'])['tiles']

bag = game['bag']
tilesOnBag = bag['tiles']

#todo best get tilesOnPlayer
for player in game['players']:
    if player['rack']['tiles'] != None:
        tilesOnPlayer = player['rack']['tiles']

#todo call api to play a tile in tilesOnPlayer to board


xmin = board.xMin()

breakpoint=0