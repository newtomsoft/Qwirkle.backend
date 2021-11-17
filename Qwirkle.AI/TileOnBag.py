from Tile import Tile
from Shape import Shape
from Color import Color
class TileOnBag(Tile):
    def __init__(self, tile):
        self.Shape = Shape(tile.shape)
        self.Color = Color(tile.color)