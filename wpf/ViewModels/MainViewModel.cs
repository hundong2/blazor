using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
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
    private bool _isRegexMode = false;
    private int _matchCount = 0;

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

    public bool IsRegexMode
    {
        get => _isRegexMode;
        set => SetProperty(ref _isRegexMode, value);
    }

    public int MatchCount
    {
        get => _matchCount;
        set => SetProperty(ref _matchCount, value);
    }

    public string Title => string.IsNullOrEmpty(Document.FilePath) ? "Untitled*" : ($"{Path.GetFileName(Document.FilePath)}" + (Document.IsDirty ? "*" : string.Empty));

    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand FormatJsonCommand { get; }
    public ICommand ToggleRegexModeCommand { get; }
    public ICommand LoadSampleDataCommand { get; }

    public MainViewModel()
    {
        NewCommand = new RelayCommand(NewFile);
        OpenCommand = new RelayCommand(OpenFile);
        SaveCommand = new RelayCommand(_ => Save(false));
        SaveAsCommand = new RelayCommand(_ => Save(true));
        FormatJsonCommand = new RelayCommand(_ => FormatJson());
        ToggleRegexModeCommand = new RelayCommand(ToggleRegexMode);
        LoadSampleDataCommand = new RelayCommand(LoadSampleData);
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
            
            // 한글 및 다국어 문자를 제대로 표시하기 위한 JsonSerializerOptions 설정
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                // 한글, 일본어, 중국어 등 모든 유니코드 문자를 이스케이프하지 않고 그대로 출력
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            
            Content = node.ToJsonString(options);
            Document.IsDirty = true;
            OnPropertyChanged(nameof(Content));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"JSON parse error: {ex.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ToggleRegexMode()
    {
        IsRegexMode = !IsRegexMode;
    }

    private void LoadSampleData()
    {
        try
        {
            var samplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "sample.json");
            if (File.Exists(samplePath))
            {
                var content = File.ReadAllText(samplePath);
                Document = new DocumentModel 
                { 
                    FilePath = samplePath, 
                    Content = content, 
                    IsDirty = false 
                };
                OnPropertyChanged(nameof(Content));
                OnPropertyChanged(nameof(Title));
            }
            else
            {
                MessageBox.Show($"Sample file not found at: {samplePath}", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load sample data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
