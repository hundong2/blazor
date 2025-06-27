using static System.Net.Mime.MediaTypeNames;

namespace DataInspector.MAUI
{
    public partial class App : Microsoft.Maui.Controls.Application // Corrected base class
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
