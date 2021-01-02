using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.PlayerContext.Ports;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IRequestCompliance RequestCompliance { get; }
        private IRequestGame RequestGame { get; }
        private IRequestPlayer RequestPlayer { get; }

        public GameViewModel GameViewModel { get; private set; }


        private IPageViewModel _currentViewModel;
        public ICommand NewGame { get; private set; }

        public IPageViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }

            private set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }




        public MainViewModel(IRequestCompliance requestCompliance, IRequestGame requestGameService, IRequestPlayer requestPlayerService, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            GameViewModel = new GameViewModel(requestCompliance, requestGameService, requestPlayerService, uiDispatcher);
            NewGame = new RelayCommand(OnNewGame);

            RequestCompliance = requestCompliance;
            RequestGame = requestGameService;
            RequestPlayer = requestPlayerService;


        }

        private void OnNewGame()
        {
            var players = RequestCompliance.CreateGame(new List<int> { 1, 2 });
            var tiles = players[0].Rack;

        }
    }
}
