using Qwirkle.Core.ComplianceContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class TileOnPlayerViewModel
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
