using GalaSoft.MvvmLight.Command;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.PlayerContext.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class GameViewModel : ViewModelBase, IPageViewModel
    {
        //private readonly IEnumerable<Product> _availableProducts;
        //private readonly User _user;
        //private readonly BuyingService _buyingService = new BuyingService();
        private string _messageInfo;
        private bool _messageInfoIsVisible;

        public RackViewModel PlayerRack { get; private set; }
        public BoardViewModel Board { get; private set; }
        public ControlsViewModel Controls { get; private set; }

        public ICommand PlayTiles { get; private set; }
        public ICommand ViewTips { get; private set; }
        public ICommand DismissMessageInfo { get; private set; }

        public string MessageInfo
        {
            get
            {
                return _messageInfo;
            }

            set
            {
                _messageInfo = value;
                NotifyPropertyChanged();
            }
        }
        public bool MessageInfoIsVisible
        {
            get
            {
                return _messageInfoIsVisible;
            }

            set
            {
                _messageInfoIsVisible = value;
                NotifyPropertyChanged();
            }
        }

        //public IEnumerable<ProductViewModel> ProductsAvailable { get; private set; }
        //public ProductViewModel SelectedProduct { get; set; }
        //public ObservableCollection<ProductViewModel> ProductsInChart { get; private set; }

        public GameViewModel(Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            PlayTiles = new RelayCommand(OnPlayTiles);
            ViewTips = new RelayCommand(OnViewTips);

            Tile tile0 = new Tile(1, TileColor.Blue, TileForm.Ring, 0);
            Tile tile1 = new Tile(2, TileColor.Red, TileForm.Square, 1);
            Tile tile2 = new Tile(3, TileColor.Purple, TileForm.Square, 2);
            Tile tile3 = new Tile(4, TileColor.Purple, TileForm.Trefail, 3);
            Tile tile4 = new Tile(5, TileColor.Orange, TileForm.Trefail, 4);
            Tile tile5 = new Tile(6, TileColor.Orange, TileForm.Ring, 5);
            Rack rack = new Rack(new Tile[] {tile0, tile1, tile2, tile3, tile4, tile5 });

            PlayerRack = new RackViewModel(rack, uiDispatcher);
            Board = new BoardViewModel(uiDispatcher);
            Controls = new ControlsViewModel(uiDispatcher);

            DismissMessageInfo = new RelayCommand(OnDismissMessageInfo);
        }

        private void OnViewTips()
        {
            throw new NotImplementedException();
        }

        private void OnPlayTiles()
        {
            throw new NotImplementedException();
        }

        private void OnDismissMessageInfo()
        {
            MessageInfoIsVisible = false;
            MessageInfo = string.Empty;
        }

        private void DisplayMessage(string message)
        {
            MessageInfoIsVisible = true;
            MessageInfo = message;
        }
    }
}