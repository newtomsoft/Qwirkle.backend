using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class ControlsViewModel : ViewModelBase, IPageViewModel
    {
        public string square0 = "0";
        public string square1 = "1";
        public string square2 = "2";
        public string square3 = "3";

        public ICommand Play { get; private set; }
        public ICommand Tips { get; private set; }

        public ControlsViewModel(Dispatcher uiDispatcher) : base(uiDispatcher)
        {
            Play = new RelayCommand(OnPlay);
            Tips = new RelayCommand(OnTips);
        }

        private void OnTips()
        {
            throw new NotImplementedException();
        }

        private void OnPlay()
        {
            throw new NotImplementedException();
        }
    }
}
