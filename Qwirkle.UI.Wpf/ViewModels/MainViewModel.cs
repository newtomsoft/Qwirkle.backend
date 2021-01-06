using Microsoft.Extensions.Configuration;
using MvvmBindingPack;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Ports;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class MainViewModel : NotifyChangesBase
    {
        private ICoreUseCase RequestCompliance { get; }

        public GameViewModel GameViewModel { get { return _gameViewModel; } private set { _gameViewModel = value; NotifyPropertyChanged(); } }
        private GameViewModel _gameViewModel;
        private IConfiguration _configuration;


        public MainViewModel(ICoreUseCase commonUseCase, IConfiguration configuration)
        {
            IsNewGameEnable = true;
            _configuration = configuration;
            RequestCompliance = commonUseCase;
            GameViewModel = new GameViewModel(false, commonUseCase, configuration, null);
        }

        public bool IsNewGameEnable
        {
            get => _isNewGameEnable;
            set { _isNewGameEnable = value; NotifyPropertyChanged(); }
        }
        private bool _isNewGameEnable;

        public void NewGame(object o)
        {
            int playerId = 1; //todo
            int secondPlayerId = 2; //todo

            var playersInGame = RequestCompliance.CreateGame(new List<int> { playerId, secondPlayerId }); //todo playerIds
            var player = playersInGame.Where(p => p.Id == playerId).First();

            Rack rack = new Rack(player.Rack.Tiles);
            var rackViewModel = new RackViewModel(rack, _configuration);
            GameViewModel = new GameViewModel(true, RequestCompliance, _configuration, rackViewModel);
        }
    }
}
