using System.Windows;
using System.Windows.Input;
using wpf.Infrastructure;
using wpf.ViewModels;

namespace wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var saveGesture = new KeyGesture(Key.S, ModifierKeys.Control);
            var saveBinding = new KeyBinding(new RelayCommand(() =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.FormatJsonCommand.Execute(null);
                    vm.SaveCommand.Execute(null);
                }
            }), saveGesture);
            InputBindings.Add(saveBinding);
        }
    }
}