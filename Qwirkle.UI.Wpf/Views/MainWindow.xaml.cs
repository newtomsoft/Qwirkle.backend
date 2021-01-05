using Microsoft.Extensions.Configuration;
using Qwirkle.Core.Ports;
using Qwirkle.UI.Wpf.ViewModels;
using System.Windows;


namespace Qwirkle.UI.Wpf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(ICommonUseCasePort requestCompliance, IConfiguration configuration)
        {
            InitializeComponent();
            DataContext = new MainViewModel(requestCompliance, configuration, Dispatcher);
        }
    }
}
