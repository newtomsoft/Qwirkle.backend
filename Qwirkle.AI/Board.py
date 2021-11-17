from TileOnBoard import TileOnBoard
from Coordinates import Coordinates

class Board:
    
    def __init__(self, tilesOnBoard):
        self.Tiles = tilesOnBoard

    def __xMin(self):
        if len(self.Tiles) == 0:
            return 0
        return min([tile.Coordinates.X for tile in self.Tiles])
    def __xMax(self):
        if len(self.Tiles) == 0:
            return 0
        return max([tile.Coordinates.X for tile in self.Tiles])
    def __yMin(self):
        if len(self.Tiles) == 0:
            return 0
        return min([tile.Coordinates.Y for tile in self.Tiles])
    def __yMax(self):
        if len(self.Tiles) == 0:
            return 0
        return max([tile.Coordinates.Y for tile in self.Tiles])
    
    def searchCoordinates(self):
        coordinates = []
        for x in range(self.__xMin()-1, self.__xMax()+1+1):
            for y in range(self.__yMin()-1, self.__yMax()+1+1):
                coordinates.append(Coordinates(x, y))
        return coordinates