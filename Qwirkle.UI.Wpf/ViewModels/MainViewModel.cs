using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.Configuration;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IRequestCompliance RequestCompliance { get; }
        private Dispatcher _uiDispatcher { get; }
        public GameViewModel GameViewModel {get { return _gameViewModel; } private set { _gameViewModel = value; OnPropertyChanged(); } }
        private GameViewModel _gameViewModel;
        private IConfiguration _configuration;
        public ICommand NewGame { get; private set; }


        public MainViewModel(IRequestCompliance requestCompliance, IConfiguration configuration, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            _configuration = configuration;
            _uiDispatcher = uiDispatcher;
            RequestCompliance = requestCompliance;
            GameViewModel = new GameViewModel(false, requestCompliance, configuration, null, uiDispatcher) ;
            NewGame = new RelayCommand(OnNewGame);
        }

        private void OnNewGame()
        {
            int playerId = 1; //todo
            int secondPlayerId = 2; //todo

            var playersInGame = RequestCompliance.CreateGame(new List<int> { playerId, secondPlayerId }); //todo playerIds
            var player = playersInGame.Where(p => p.Id == playerId).First();

            Rack rack = new Rack(player.Rack.Tiles); 
            var rackViewModel = new RackViewModel(rack, _configuration, _uiDispatcher);
            GameViewModel = new GameViewModel(true, RequestCompliance, _configuration, rackViewModel, _uiDispatcher);
        }
    }
}
