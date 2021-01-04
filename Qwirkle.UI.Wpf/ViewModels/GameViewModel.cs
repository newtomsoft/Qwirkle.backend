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

        public RackViewModel RackViewModel { get; private set; }
        public BoardViewModel BoardViewModel { get; private set; }

        public ICommand ChangeTiles { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }

        private readonly Dispatcher _uiDispatcher;

        public bool GameInProgress { get { return _gameInProgress; } private set { _gameInProgress = value; OnPropertyChanged(nameof(GameInProgress)); } }
        private bool _gameInProgress;
        public bool GameNotInProgress { get { return !_gameInProgress; } private set { _gameInProgress = !value; OnPropertyChanged(nameof(GameInProgress)); } }

        public GameViewModel(bool newGame, IRequestCompliance requestCompliance, RackViewModel rack, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            GameInProgress = newGame;

            RequestCompliance = requestCompliance;

            Play = new RelayCommand(OnPlay);
            Tips = new RelayCommand(OnTips);
            ChangeTiles = new RelayCommand(OnChangeTiles);

            RackViewModel = rack;
            BoardViewModel = new BoardViewModel(uiDispatcher);

            _uiDispatcher = uiDispatcher;
        }

        private void OnChangeTiles()
        {
            if (RackViewModel.SelectedCells.Count == 0) return;
            List<int> tilesIds = new List<int>();
            foreach (var cell in RackViewModel.SelectedCells)
                tilesIds.Add(((TileOnPlayerViewModel)cell.Item).Id);

            var rack = RequestCompliance.SwapTiles(1, tilesIds); //todo playerId
            if (rack != null)
                RackViewModel = new RackViewModel(rack, _uiDispatcher);
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