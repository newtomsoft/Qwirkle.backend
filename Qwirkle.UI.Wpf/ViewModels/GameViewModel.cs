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

        public RackViewModel RackViewModel { get; private set; }
        public BoardViewModel BoardViewModel { get; private set; }

        public ICommand ChangeTiles { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }


        public GameViewModel(IRequestCompliance requestCompliance, RackViewModel rack, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            RequestCompliance = requestCompliance;

            Play = new RelayCommand(OnPlay);
            Tips = new RelayCommand(OnTips);
            ChangeTiles = new RelayCommand(OnChangeTiles);

            RackViewModel = rack;
            BoardViewModel = new BoardViewModel(uiDispatcher);
        }

        private void OnChangeTiles()
        {
            if (RackViewModel.SelectedCells.Count == 0) return;
            List<int> tilesIds = new List<int>();
            foreach (var cell in RackViewModel.SelectedCells)
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