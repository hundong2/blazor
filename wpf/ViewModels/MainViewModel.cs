using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using wpf.Infrastructure;
using wpf.Models;

namespace wpf.ViewModels;

public class MainViewModel : ObservableObject
{
    private DocumentModel _document = new();

    public DocumentModel Document
    {
        get => _document;
        set => SetProperty(ref _document, value);
    }

    public string Content
    {
        get => Document.Content;
        set
        {
            if (Document.Content != value)
            {
                Document.Content = value;
                Document.IsDirty = true;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public string Title => string.IsNullOrEmpty(Document.FilePath) ? "Untitled*" : ($"{Path.GetFileName(Document.FilePath)}" + (Document.IsDirty ? "*" : string.Empty));

    public ICommand NewCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand FormatJsonCommand { get; }

    public MainViewModel()
    {
        NewCommand = new RelayCommand(NewFile);
        SaveCommand = new RelayCommand(_ => Save(false));
        SaveAsCommand = new RelayCommand(_ => Save(true));
        FormatJsonCommand = new RelayCommand(_ => FormatJson());
    }

    private void NewFile()
    {
        Document = new DocumentModel { Content = "{}", IsDirty = false };
        OnPropertyChanged(nameof(Title));
    }

    private void Save(bool saveAs)
    {
        string? path = Document.FilePath;
        if (saveAs || string.IsNullOrEmpty(path))
        {
            var dlg = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != true) return;
            path = dlg.FileName;
            Document.FilePath = path;
        }

        try
        {
            File.WriteAllText(path!, Content);
            Document.IsDirty = false;
            OnPropertyChanged(nameof(Title));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void FormatJson()
    {
        try
        {
            var node = JsonNode.Parse(Content);
            if (node is null) return;
            Content = node.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            Document.IsDirty = true;
            OnPropertyChanged(nameof(Content));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"JSON parse error: {ex.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
