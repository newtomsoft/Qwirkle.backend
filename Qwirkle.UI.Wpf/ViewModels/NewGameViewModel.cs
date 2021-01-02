using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class NewGameViewModel : ViewModelBase, IPageViewModel
    {
        public NewGameViewModel(Dispatcher uiDispatcher) : base(uiDispatcher)
        {
        }
    }
}
