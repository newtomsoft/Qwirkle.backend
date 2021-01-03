using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IRequestCompliance RequestCompliance { get; }
        private Dispatcher UiDispatcher { get; }
        public GameViewModel GameViewModel {get { return _gameViewModel; } private set { _gameViewModel = value; OnPropertyChanged(); } }
        private GameViewModel _gameViewModel;

        public ICommand NewGame { get; private set; }



        public MainViewModel(IRequestCompliance requestCompliance, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            UiDispatcher = uiDispatcher;
            RequestCompliance = requestCompliance;
            GameViewModel = new GameViewModel(requestCompliance, null, uiDispatcher) ;
            NewGame = new RelayCommand(OnNewGame);
        }

        private void OnNewGame()
        {
            var players = RequestCompliance.CreateGame(new List<int> { 1, 2 }); //todo playerIds

            Rack rack = new Rack(players[0].Rack.Tiles); //todo player[0] ? prendre bon index
            var rackViewModel = new RackViewModel(rack, UiDispatcher);
            GameViewModel = new GameViewModel(RequestCompliance, rackViewModel, UiDispatcher);
        }
    }
}
