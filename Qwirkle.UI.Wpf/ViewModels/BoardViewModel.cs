using Microsoft.Extensions.Configuration;

namespace Qwirkle.UI.Wpf.ViewModels
{
    public class BoardViewModel : NotifyPropertyChangedBase
    {
        public string square0 = "0";
        public string square1 = "1";
        public string square2 = "2";
        public string square3 = "3";

        public BoardViewModel(IConfiguration configuration)
        {
        }
    }
}
