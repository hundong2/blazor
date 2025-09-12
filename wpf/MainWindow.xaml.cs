using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using wpf.Infrastructure;
using wpf.Services;
using wpf.ViewModels;

namespace wpf
{
    public partial class MainWindow : Window
    {
        private TextHighlightService _highlightService;
        private TextMarkerService _textMarkerService;

        public MainWindow()
        {
            InitializeComponent();
            SetupEditor();
            SetupKeyBindings();
            SetupEventHandlers();
        }

        private void SetupEditor()
        {
            // Initialize highlight services
            _textMarkerService = new TextMarkerService(JsonEditor.Document);
            _textMarkerService.SetTextView(JsonEditor.TextArea.TextView);
            _highlightService = new TextHighlightService();
            _highlightService.SetTextMarkerService(_textMarkerService);

            // Add text marker service to editor
            JsonEditor.TextArea.TextView.LineTransformers.Add(_textMarkerService);

            // Bind editor content to ViewModel
            if (DataContext is MainViewModel viewModel)
            {
                // Set initial regex mode
                _highlightService.SetRegexMode(viewModel.IsRegexMode);
                
                // Initial content binding
                JsonEditor.Text = viewModel.Content;
                
                // Two-way binding setup
                JsonEditor.TextChanged += (s, e) => 
                {
                    viewModel.Content = JsonEditor.Text;
                    
                    // Re-highlight after text changes with current highlight term
                    if (!string.IsNullOrEmpty(viewModel.HighlightTerm))
                    {
                        _highlightService.HighlightText(viewModel.HighlightTerm, JsonEditor.Document);
                        UpdateMatchCount();
                    }
                };

                // Listen for content changes from ViewModel
                viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainViewModel.Content) && JsonEditor.Text != viewModel.Content)
                    {
                        JsonEditor.Text = viewModel.Content;
                        
                        // Re-apply highlights after content change
                        if (!string.IsNullOrEmpty(viewModel.HighlightTerm))
                        {
                            _highlightService.HighlightTextImmediate(viewModel.HighlightTerm, JsonEditor.Document);
                            UpdateMatchCount();
                        }
                    }
                };
            }
        }

        private void SetupKeyBindings()
        {
            // Ctrl+S: Format and Save
            var saveGesture = new KeyGesture(Key.S, ModifierKeys.Control);
            var saveBinding = new KeyBinding(new RelayCommand(() =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.FormatJsonCommand.Execute(null);
                    vm.SaveCommand.Execute(null);
                }
            }), saveGesture);

            // Ctrl+O: Open
            var openGesture = new KeyGesture(Key.O, ModifierKeys.Control);
            var openBinding = new KeyBinding(new RelayCommand(() =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.OpenCommand.Execute(null);
                }
            }), openGesture);

            // Ctrl+N: New
            var newGesture = new KeyGesture(Key.N, ModifierKeys.Control);
            var newBinding = new KeyBinding(new RelayCommand(() =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.NewCommand.Execute(null);
                }
            }), newGesture);

            // Ctrl+Shift+F: Format JSON
            var formatGesture = new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift);
            var formatBinding = new KeyBinding(new RelayCommand(() =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.FormatJsonCommand.Execute(null);
                }
            }), formatGesture);

            // Ctrl+R: Toggle Regex Mode
            var regexGesture = new KeyGesture(Key.R, ModifierKeys.Control);
            var regexBinding = new KeyBinding(new RelayCommand(() =>
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.ToggleRegexModeCommand.Execute(null);
                }
            }), regexGesture);

            InputBindings.Add(saveBinding);
            InputBindings.Add(openBinding);
            InputBindings.Add(newBinding);
            InputBindings.Add(formatBinding);
            InputBindings.Add(regexBinding);
        }

        private void SetupEventHandlers()
        {
            // Handle highlight term changes with immediate visual feedback
            HighlightBox.TextChanged += (s, e) =>
            {
                if (DataContext is MainViewModel vm)
                {
                    // Update ViewModel property which will trigger PropertyChanged
                    vm.HighlightTerm = HighlightBox.Text;
                }
            };

            // Clear highlights button
            ClearHighlightButton.Click += (s, e) =>
            {
                _highlightService.ClearHighlights();
                if (DataContext is MainViewModel vm)
                {
                    vm.HighlightTerm = string.Empty;
                    vm.MatchCount = 0;
                }
                HighlightBox.Clear();
            };

            // Regex mode toggle
            RegexModeToggle.Checked += (s, e) => UpdateRegexMode();
            RegexModeToggle.Unchecked += (s, e) => UpdateRegexMode();

            // Handle ViewModel property changes for highlighting
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.HighlightTerm))
            {
                if (sender is MainViewModel vm)
                {
                    // Use immediate highlighting for responsive UI
                    _highlightService.HighlightTextImmediate(vm.HighlightTerm, JsonEditor.Document);
                    UpdateMatchCount();
                }
            }
            else if (e.PropertyName == nameof(MainViewModel.IsRegexMode))
            {
                UpdateRegexMode();
            }
        }

        private void UpdateRegexMode()
        {
            if (DataContext is MainViewModel vm)
            {
                _highlightService.SetRegexMode(vm.IsRegexMode);
                
                // Re-highlight with new mode if there's a search term
                if (!string.IsNullOrEmpty(vm.HighlightTerm))
                {
                    _highlightService.HighlightTextImmediate(vm.HighlightTerm, JsonEditor.Document);
                    UpdateMatchCount();
                }
            }
        }

        private void UpdateMatchCount()
        {
            if (DataContext is MainViewModel vm)
            {
                vm.MatchCount = _highlightService.GetMatchCount();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Clean up resources
            _highlightService?.Dispose();
            base.OnClosed(e);
        }
    }
}