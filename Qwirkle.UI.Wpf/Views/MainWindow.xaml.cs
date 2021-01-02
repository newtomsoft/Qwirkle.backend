using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.UI.Wpf.ViewModels;
using System.Windows;


namespace Qwirkle.UI.Wpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(IRequestCompliance requestCompliance, IRequestPlayer requestPlayerService)
        {
            InitializeComponent();
            DataContext = new MainViewModel(requestCompliance, requestPlayerService, Dispatcher);
        }
    }
}
