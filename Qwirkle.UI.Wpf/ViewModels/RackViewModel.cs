using Qwirkle.Core.PlayerContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class RackViewModel : ViewModelBase, IPageViewModel
    {
        public string[] ImagePath { get; private set; }

        public RackViewModel(Rack rack, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            ImagePath = new string[6];
            //for (int i = 0; i < ImagePath.Length; i++)
            //{
            //    ImagePath[i] = rack.Tiles[i].
            //}
            ImagePath[0]= @"D:\Boulot\projets info\Qwirkle\Qwirkle.Infra.Persistence\Images\Tiles\Png\BlueCircle.png";
            ImagePath[1]= @"D:\Boulot\projets info\Qwirkle\Qwirkle.Infra.Persistence\Images\Tiles\Png\BlueClover.png";
            ImagePath[2]= @"D:\Boulot\projets info\Qwirkle\Qwirkle.Infra.Persistence\Images\Tiles\Png\GreenFourPointStar.png";
            ImagePath[3]= @"D:\Boulot\projets info\Qwirkle\Qwirkle.Infra.Persistence\Images\Tiles\Png\OrangeDiamond.png";
            ImagePath[4]= @"D:\Boulot\projets info\Qwirkle\Qwirkle.Infra.Persistence\Images\Tiles\Png\OrangeSquare.png";
            ImagePath[5]= @"D:\Boulot\projets info\Qwirkle\Qwirkle.Infra.Persistence\Images\Tiles\Png\RedEightPointStar.png";
        }
    }
}
