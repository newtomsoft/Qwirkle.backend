using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class BoardViewModel : ViewModelBase, IPageViewModel
    {
        public string square0 = "0";
        public string square1 = "1";
        public string square2 = "2";
        public string square3 = "3";

        public BoardViewModel(IConfiguration configuration, Dispatcher uiDispatcher) : base(uiDispatcher)
        {
        }
    }
}
