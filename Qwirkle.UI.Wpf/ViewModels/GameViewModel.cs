using Microsoft.Extensions.Configuration;
using MvvmBindingPack;
using Qwirkle.Core.Ports;
using System.Collections.Generic;
using System.Windows;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class GameViewModel : NotifyChangesBase
    {
        private ICoreUseCase CommonUseCases { get; }

        public RackViewModel RackViewModel { get { return _rackViewModel; } private set { _rackViewModel = value; NotifyPropertyChanged(); } }
        private RackViewModel _rackViewModel;
        public BoardViewModel BoardViewModel { get; private set; }

        private IConfiguration _configuration;

        public bool GameInProgress { get { return _gameInProgress; } private set { _gameInProgress = value; NotifyPropertyChanged(); } }
        private bool _gameInProgress;
        public bool GameNotInProgress { get { return !_gameInProgress; } private set { _gameInProgress = !value; NotifyPropertyChanged(); } }

        public GameViewModel(bool newGame, ICoreUseCase commonUseCases, IConfiguration configuration, RackViewModel rack)
        {
            _configuration = configuration;

            GameInProgress = newGame;

            CommonUseCases = commonUseCases;

            RackViewModel = rack;
            BoardViewModel = new BoardViewModel(configuration);
        }

        private bool CanExecuteChangeTiles()
        {
            return RackViewModel != null && RackViewModel.SelectedCells.Count != 0;
        }

        public void ChangeTiles(object o)
        {
            if (RackViewModel.SelectedCells.Count == 0)
            {
                MessageBox.Show("aucune tuile à échanger");
                return;
            }
            List<int> tilesIds = new List<int>();
            foreach (var cell in RackViewModel.SelectedCells)
                tilesIds.Add(((TileOnPlayerViewModel)cell.Item).Tile.Id);

            var rack = CommonUseCases.SwapTiles(1, tilesIds); //todo playerId
            if (rack != null)
                RackViewModel = new RackViewModel(rack, _configuration);
            else
                MessageBox.Show("aucune tuile ne peut être échangée");
        }

        public void Tips(object o)
        {
            MessageBox.Show("Fonctionnalité Tips en cours de dev...");
        }

        public void Play(object o)
        {
            MessageBox.Show("Fonctionnalité Play en cours de dev...");
        }
    }
}