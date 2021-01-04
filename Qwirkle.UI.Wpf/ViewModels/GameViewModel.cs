using Microsoft.Extensions.Configuration;
using Qwirkle.Core.ComplianceContext.Ports;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class GameViewModel : ViewModelBase, IPageViewModel
    {
        private IRequestCompliance RequestCompliance { get; }

        public RackViewModel RackViewModel { get { return _rackViewModel; } private set { _rackViewModel = value; OnPropertyChanged(nameof(RackViewModel)); } }
        private RackViewModel _rackViewModel;
        public BoardViewModel BoardViewModel { get; private set; }

        public ICommand ChangeTiles { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }

        private readonly Dispatcher _uiDispatcher;
        private IConfiguration _configuration;

        public bool GameInProgress { get { return _gameInProgress; } private set { _gameInProgress = value; OnPropertyChanged(nameof(GameInProgress)); } }
        private bool _gameInProgress;
        public bool GameNotInProgress { get { return !_gameInProgress; } private set { _gameInProgress = !value; OnPropertyChanged(nameof(GameInProgress)); } }

        public GameViewModel(bool newGame, IRequestCompliance requestCompliance, IConfiguration configuration, RackViewModel rack, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            _configuration = configuration;
            _uiDispatcher = uiDispatcher;

            GameInProgress = newGame;

            RequestCompliance = requestCompliance;

            Play = new RelayCommand(OnPlay);
            Tips = new RelayCommand(OnTips);
            ChangeTiles = new RelayCommand(OnChangeTiles);

            RackViewModel = rack;
            BoardViewModel = new BoardViewModel(configuration, uiDispatcher);
        }

        private void OnChangeTiles()
        {
            if (RackViewModel.SelectedCells.Count == 0)
            {
                MessageBox.Show("aucune tuile à échanger");
                return;
            }
            List<int> tilesIds = new List<int>();
            foreach (var cell in RackViewModel.SelectedCells)
                tilesIds.Add(((TileOnPlayerViewModel)cell.Item).Tile.Id);

            var rack = RequestCompliance.SwapTiles(1, tilesIds); //todo playerId
            if (rack != null)
                RackViewModel = new RackViewModel(rack, _configuration, _uiDispatcher);
            else
                MessageBox.Show("aucune tuile ne peut être échangée");
        }

        private void OnTips()
        {
            MessageBox.Show("Fonctionnalité Tips en cours de dev...");
        }

        private void OnPlay()
        {
            MessageBox.Show("Fonctionnalité Play en cours de dev...");
        }
    }
}