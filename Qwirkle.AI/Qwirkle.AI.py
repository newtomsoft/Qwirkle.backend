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


def algo(board, searchCoordinates, tilesOnPlayer, moves):
    toto = []
    toto.remove()
    if moves.count == 0:
        for tile in tilesOnPlayer:
            for coordinates in searchCoordinates:
                param_play = [{'playerId': playerId, 'tileId': tile.Id, 'x': coordinates.X, 'y':coordinates.Y}]
                response_playSimulation = requests.post(f'{url_playTilesSimulation}', json=param_play, cookies=cookies, verify=False)
                points = json.loads(response_playSimulation.text, object_hook=lambda d: SimpleNamespace(**d)).points
                if points > 0:
                    moves.append((coordinates, tile.Id, points))
                    alteredSearchCoordinates = searchCoordinates.remove(coordinates)
                    tileToAdd = Tile(tile.Id, tile.Shape, tile.Color)
                    tileOnBoardToAdd = TileOnBoard(tileToAdd)
                    board.append(tileOnBoardToAdd)
                    newSearchCoordinates = board.searchCoordinates()
                    return algo(board, newSearchCoordinates, )
        return

    for move in moves:
        for tile in tilesOnPlayer:
            for coordinates in searchCoordinates:
                param_play = [{'playerId': playerId, 'tileId': tile.Id, 'x': coordinates.X, 'y':coordinates.Y}]
                response_playSimulation = requests.post(f'{url_playTilesSimulation}', json=param_play, cookies=cookies, verify=False)
                points = json.loads(response_playSimulation.text, object_hook=lambda d: SimpleNamespace(**d)).points
                if points > 0:
                    moves.append((coordinates, tile.Id, points))




host = 'https://localhost:5001'
logout = '/User/Logout'
login = '/User/Login'
userGamesIds = '/Game/UserGamesIds'
game = '/Game/'
playTilesSimulation = '/Action/PlayTilesSimulation'
getPlayerIdByGameId = '/Player/PlayerIdByGameId/'

url_logout = f'{host}{logout}'
url_login = f'{host}{login}'
url_userGamesIds = f'{host}{userGamesIds}'
url_game = f'{host}{game}'
url_playTilesSimulation = f'{host}{playTilesSimulation}'
url_getPlayerByGameId = f'{host}{getPlayerIdByGameId}'

response_logout = requests.get(url_logout, verify=False)

param_login = {'pseudo': 'bot1', 'password': 'botpassword', 'isRemember': True}
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
gameId = game.id
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

response_playerId = requests.get(f'{url_getPlayerByGameId}{gameId}', cookies=cookies, verify=False)
playerId = response_playerId.text

moves = []
moveNumber = 0
for tile in tilesOnPlayer:
    for coordinates in searchCoordinates:
        
        moveNumber += 1
        param_play = [{'playerId': playerId, 'tileId': tile.Id, 'x': coordinates.X, 'y':coordinates.Y}]
        response_playSimulation = requests.post(f'{url_playTilesSimulation}', json=param_play, cookies=cookies, verify=False)
        points = json.loads(response_playSimulation.text, object_hook=lambda d: SimpleNamespace(**d)).points
        if points > 0:
            moves.append((coordinates, tile.Id, points))


breakpoint=0



