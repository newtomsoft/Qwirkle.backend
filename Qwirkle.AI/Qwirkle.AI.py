from Color import Color
from Shape import Shape
from Tile import Tile
from Coordinates import Coordinates
from TileOnBoard import TileOnBoard
from Board import Board

tileOnBoard1 = TileOnBoard(Tile(1, Shape.Circle, Color.Purple), Coordinates(0,0))
tileOnBoard2 = TileOnBoard(Tile(2, Shape.Square, Color.Yellow), Coordinates(0,1))
tileOnBoard3 = TileOnBoard(Tile(3, Shape.EightPointStar, Color.Green), Coordinates(1,0))
tilesOnBoard =  [tileOnBoard1, tileOnBoard2, tileOnBoard3]
board = Board(tilesOnBoard)

for tile in board.Tiles:
  print(f"{tile.Color} {tile.Shape} {tile.Coordinates.X} {tile.Coordinates.Y}")


