using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.PlayerContext.Ports;
using Qwirkle.UI.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Qwirkle.UI.Wpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(IRequestCompliance requestCompliance, IRequestGame requestGameService, IRequestPlayer requestPlayerService)
        {
            InitializeComponent();
            DataContext = new MainViewModel(requestCompliance, requestGameService, requestPlayerService, Dispatcher);
        }
    }
}
