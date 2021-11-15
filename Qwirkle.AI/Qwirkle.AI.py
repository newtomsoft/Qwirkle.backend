from enum import Enum

class Color(Enum):
    Green = 1
    Blue = 2
    Purple = 3
    Red = 4
    Orange = 5
    Yellow = 6

class Shape(Enum):
    Circle = 1
    Square = 2
    Diamond = 3
    Clover = 4
    FourPointStar = 5
    EightPointStar = 6

class Tile(object):
    def __init__(self, id, shape, color):
        self.Id = id
        self.Shape = shape
        self.Color = color

class Coordinates(object):
    def __init__(self, x, y):
        self.X = x
        self.Y = y

class TileOnBoard(Tile):
    def __init__(self, tile, coordinates):
        self.Shape = tile.Shape
        self.Color = tile.Color
        self.Coordinates = coordinates

tile1 = Tile(1, Shape.Circle, Color.Purple)
tile2 = Tile(2, Shape.Square, Color.Yellow)
tile3 = Tile(3, Shape.EightPointStar, Color.Green)
tiles = [tile1, tile2]

tileOnBoard1 = TileOnBoard(tile1, Coordinates(0,0))
tileOnBoard2 = TileOnBoard(tile2, Coordinates(0,1))
tileOnBoard3 = TileOnBoard(tile3, Coordinates(1,0))
tilesOnBoard =  [tileOnBoard1, tileOnBoard2, tileOnBoard3]

class Board(object):
    def __init__(self, id, tilesOnBoard):
        self.Id = id
        self.Tiles = tilesOnBoard



board = Board(1, tilesOnBoard)

for tile in board.Tiles:
  print(f"{board.Id} {tile.Color} {tile.Shape} {tile.Coordinates.X} {tile.Coordinates.Y}")


