using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class TileViewModel
    {
        public TileViewModel(string fullNameImage)
        {
            FullNameImage = fullNameImage;
            Image = new BitmapImage(new Uri(FullNameImage));
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FullNameImage { get; set; }
        public BitmapImage Image { get; set; }
    }
}
