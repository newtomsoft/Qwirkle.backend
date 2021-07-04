using Qwirkle.Core.Entities;
using System;
using System.Windows.Media.Imaging;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class TileOnPlayerViewModel : NotifyPropertyChangedBase
    {
        public TileOnPlayer Tile { get; }
        public string FullNameImage { get; }
        public BitmapImage Image { get; }

        public TileOnPlayerViewModel(TileOnPlayer tile, string fullNameImage)
        {
            Tile = tile;
            FullNameImage = fullNameImage;
            Image = new BitmapImage(new Uri(FullNameImage));
        }
    }
}
