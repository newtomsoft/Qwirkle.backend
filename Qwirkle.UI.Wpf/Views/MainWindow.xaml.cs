using Microsoft.Extensions.Configuration;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.UI.Wpf.ViewModels;
using System.Configuration;
using System.Windows;


namespace Qwirkle.UI.Wpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(IRequestCompliance requestCompliance, IConfiguration configuration)
        {
            InitializeComponent();
            DataContext = new MainViewModel(requestCompliance, configuration, Dispatcher);
        }
    }
}
