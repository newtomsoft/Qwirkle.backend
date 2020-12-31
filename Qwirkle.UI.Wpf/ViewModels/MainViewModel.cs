using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.PlayerContext.Ports;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IPageViewModel _currentViewModel;

        public IPageViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }

            private set
            {
                _currentViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public MainViewModel(IRequestCompliance requestCompliance, IRequestGame requestGameService, IRequestPlayer requestPlayerService, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            CurrentViewModel = new GameViewModel(requestCompliance, requestGameService, requestPlayerService, uiDispatcher);
        }
    }
}
