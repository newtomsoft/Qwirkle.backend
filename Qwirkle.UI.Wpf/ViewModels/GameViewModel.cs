using Qwirkle.Core.ComplianceContext;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Enums;
using Qwirkle.Core.ComplianceContext.Ports;
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
        private IRequestPlayer RequestPlayer { get; }

        public RackViewModel Rack { get; private set; }
        public BoardViewModel Board { get; private set; }

        public ICommand ChangeTiles { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }


        public GameViewModel(IRequestCompliance requestCompliance, IRequestPlayer requestPlayerService, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            var tile0 = new TileOnPlayer(0, 1, TileColor.Blue, TileForm.Ring);
            var tile1 = new TileOnPlayer(1, 2, TileColor.Red, TileForm.Square);
            var tile2 = new TileOnPlayer(2, 3, TileColor.Purple, TileForm.Square);
            var tile3 = new TileOnPlayer(3, 4, TileColor.Purple, TileForm.Trefail);
            var tile4 = new TileOnPlayer(4, 5, TileColor.Orange, TileForm.Trefail);
            var tile5 = new TileOnPlayer(5, 6, TileColor.Orange, TileForm.Ring);
            Rack rack = new Rack(new List<TileOnPlayer> { tile0, tile1, tile2, tile3, tile4, tile5 });

            RequestCompliance = requestCompliance;
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