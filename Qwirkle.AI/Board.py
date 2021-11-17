from TileOnBoard import TileOnBoard
class Board:
    def __init__(self, tilesOnBoard):
        self.Tiles = tilesOnBoard

    def xMin(self):
        return min([tile.Coordinates.X for tile in self.Tiles])
    def xMax(self):
        return max([tile.Coordinates.X for tile in self.Tiles])
    def yMin(self):
        return min([tile.Coordinates.Y for tile in self.Tiles])
    def yMax(self):
        return max([tile.Coordinates.Y for tile in self.Tiles])