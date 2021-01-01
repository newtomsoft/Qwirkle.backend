using Qwirkle.Core.CommonContext;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.PlayerContext.Entities;
using Qwirkle.Core.PlayerContext.Ports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class GameViewModel : ViewModelBase, IPageViewModel
    {
        private IRequestCompliance RequestCompliance { get; }
        private IRequestGame RequestGame { get; }
        private IRequestPlayer RequestPlayer { get; }

        public RackViewModel Rack { get; private set; }
        public BoardViewModel Board { get; private set; }

        public ICommand ChangeTiles { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }


        public GameViewModel(IRequestCompliance requestCompliance, IRequestGame requestGameService, IRequestPlayer requestPlayerService, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            Tile tile0 = new Tile(1, TileColor.Blue, TileForm.Ring, 0);
            Tile tile1 = new Tile(2, TileColor.Red, TileForm.Square, 1);
            Tile tile2 = new Tile(3, TileColor.Purple, TileForm.Square, 2);
            Tile tile3 = new Tile(4, TileColor.Purple, TileForm.Trefail, 3);
            Tile tile4 = new Tile(5, TileColor.Orange, TileForm.Trefail, 4);
            Tile tile5 = new Tile(6, TileColor.Orange, TileForm.Ring, 5);
            Rack rack = new Rack(new Tile[] {tile0, tile1, tile2, tile3, tile4, tile5 });

            RequestCompliance = requestCompliance;
            RequestGame = requestGameService;
            RequestPlayer = requestPlayerService;

            Play = new RelayCommand(OnPlay);
            Tips = new RelayCommand(OnTips);
            ChangeTiles = new RelayCommand(OnChangeTiles);

            Rack = new RackViewModel(rack, uiDispatcher);
            Board = new BoardViewModel(uiDispatcher);
        }

        private void OnChangeTiles()
        {
            if (Rack.SelectedCells.Count == 0) return;
            List<int> tilesIds = new List<int>();
            foreach (var cell in Rack.SelectedCells)
            {
                tilesIds.Add(((TileViewModel)cell.Item).Id);
            }
            RequestCompliance.SwapTiles(1, tilesIds);
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