from Tile import Tile
class TileOnBoard(Tile):
    #def __init__(self, tile, coordinates):
    #    self.Shape = tile.Shape
    #    self.Color = tile.Color
    #    self.Coordinates = coordinates
    def __init__(self, tile):
        self.Shape = tile['shape']
        self.Color = tile['color']
        #self.Coordinates = coordinates