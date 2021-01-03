using Qwirkle.Core.ComplianceContext.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{

    public class RackViewModel : ViewModelBase, IPageViewModel
    {
        private const string PATH_IMAGE = @"D:\Boulot\projets info\Qwirkle\Qwirkle.Ressources\Images\Tiles\";

        private IList<DataGridCellInfo> _selectedCells;
        public IList<DataGridCellInfo> SelectedCells { get => _selectedCells; set { _selectedCells = value; OnPropertyChanged(nameof(SelectedCells)); } }

        public List<TileViewModel> TilesViewModel { get; set; }

        public TileViewModel SelectedTileViewModel { get; set; }


        public RackViewModel(Rack rack, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            SelectedCells = new List<DataGridCellInfo>();

            var tilesViewModel = new List<TileViewModel>();
            foreach (var tile in rack.Tiles)
            {
                string fullName = GetFullNameImage(tile);
                tilesViewModel.Add(new TileViewModel(fullName));
            }
            TilesViewModel = tilesViewModel;
        }

        private static string GetFullNameImage(TileOnPlayer tile)
        {
            return Path.Combine(PATH_IMAGE, tile.GetNameImage()); ;
        }
    }
}
