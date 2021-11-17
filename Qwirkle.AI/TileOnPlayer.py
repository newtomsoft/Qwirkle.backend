from Tile import Tile
from Tile import Tile
from Shape import Shape
from Color import Color

class TileOnPlayer(Tile):
    def __init__(self, tile):
        self.Id = int(tile.id)
        self.Shape = Shape(tile.shape)
        self.Color = Color(tile.color)
        self.RackPosition = tile.rackPosition