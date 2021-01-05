using Microsoft.Extensions.Configuration;
using Qwirkle.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{

    public class RackViewModel : ViewModelBase, IPageViewModel
    {
        private IList<DataGridCellInfo> _selectedCells;
        public IList<DataGridCellInfo> SelectedCells { get => _selectedCells; set { _selectedCells = value; OnPropertyChanged(nameof(SelectedCells)); } }

        public List<TileOnPlayerViewModel> TilesViewModel { get; set; }

        public TileOnPlayerViewModel SelectedTileViewModel { get; set; }

        private readonly IConfiguration _configuration;

        public RackViewModel(Rack rack, IConfiguration configuration, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            _configuration = configuration;
            SelectedCells = new List<DataGridCellInfo>();

            var tilesViewModel = new List<TileOnPlayerViewModel>();
            foreach (var tile in rack.Tiles)
            {
                string fullName = GetFullNameImage(tile);
                tilesViewModel.Add(new TileOnPlayerViewModel(tile, fullName));
            }
            tilesViewModel = tilesViewModel.OrderBy(t => t.Tile.RackPosition).ToList();
            TilesViewModel = tilesViewModel;
        }

        private string GetFullNameImage(TileOnPlayer tile) => Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetSection("ImagesPath:Tiles").Value, tile.GetNameImage());
    }
}
