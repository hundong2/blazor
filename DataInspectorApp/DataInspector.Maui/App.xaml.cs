namespace DataInspector.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage(); // Ensure MainPage is set as the starting page
	}
}
