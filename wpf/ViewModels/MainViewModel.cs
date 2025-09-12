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
    private string _highlightTerm = string.Empty;

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

    public string HighlightTerm
    {
        get => _highlightTerm;
        set => SetProperty(ref _highlightTerm, value);
    }

    public string Title => string.IsNullOrEmpty(Document.FilePath) ? "Untitled*" : ($"{Path.GetFileName(Document.FilePath)}" + (Document.IsDirty ? "*" : string.Empty));

    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand FormatJsonCommand { get; }

    public MainViewModel()
    {
        NewCommand = new RelayCommand(NewFile);
        OpenCommand = new RelayCommand(OpenFile);
        SaveCommand = new RelayCommand(_ => Save(false));
        SaveAsCommand = new RelayCommand(_ => Save(true));
        FormatJsonCommand = new RelayCommand(_ => FormatJson());
    }

    private void NewFile()
    {
        Document = new DocumentModel { Content = "{}", IsDirty = false };
        OnPropertyChanged(nameof(Content));
        OnPropertyChanged(nameof(Title));
    }

    private void OpenFile()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };
        
        if (dlg.ShowDialog() != true) return;

        try
        {
            var content = File.ReadAllText(dlg.FileName);
            Document = new DocumentModel 
            { 
                FilePath = dlg.FileName, 
                Content = content, 
                IsDirty = false 
            };
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Title));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Open failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
