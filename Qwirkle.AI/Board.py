from TileOnBoard import TileOnBoard
class Board:
    def __init__(self, tilesOnBoard):
        #for tile in tilesOnBoard:
        #    TileOnBoard(tile)       
        self.Tiles = [TileOnBoard(tile) for tile in tilesOnBoard]
        #self.Tiles = tilesOnBoard
    def xMin(self):
        toto = min((tile['coordinates'])['x'] for tile in self.Tiles)
        return toto