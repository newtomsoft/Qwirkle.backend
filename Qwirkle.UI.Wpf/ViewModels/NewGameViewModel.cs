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
