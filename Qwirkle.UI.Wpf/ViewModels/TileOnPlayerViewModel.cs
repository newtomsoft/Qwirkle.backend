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
        public int Id { get; }
        public int RackPosition { get; }
        public string FullNameImage { get; }
        public BitmapImage Image { get; }

        public TileOnPlayerViewModel(int id, string fullNameImage)
        {
            Id = id;
            FullNameImage = fullNameImage;
            Image = new BitmapImage(new Uri(FullNameImage));
        }
    }
}
