using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.PlayerContext.Ports;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class ControlsViewModel : ViewModelBase, IPageViewModel
    {
        private IRequestCompliance RequestCompliance { get; }
        private IRequestGame RequestGame { get; }
        private IRequestPlayer RequestPlayer { get; }

        public string square0 = "0";
        public string square1 = "1";
        public string square2 = "2";
        public string square3 = "3";
        public ICommand ChangeTiles { get; private set; }
        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }

        public ControlsViewModel(IRequestCompliance requestCompliance, IRequestGame requestGameService, IRequestPlayer requestPlayerService, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            RequestCompliance = requestCompliance;
            RequestGame = requestGameService;
            RequestPlayer = requestPlayerService;

            Play = new RelayCommand(OnPlay);
            Tips = new RelayCommand(OnTips);
            ChangeTiles = new RelayCommand(OnChangeTiles);
        }

        private void OnChangeTiles()
        {
            MessageBox.Show("Fonctionnalité échange des tuiles en cours de dev...");
            RequestCompliance.CreateGame(new List<int> { 1, 2, 3 });
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