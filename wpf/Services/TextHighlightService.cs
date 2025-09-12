using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace wpf.Services;

public class TextHighlightService
{
    private readonly List<ITextMarker> _markers = new();
    private ITextMarkerService? _textMarkerService;
    private DispatcherTimer? _debounceTimer;
    private string _pendingSearchText = string.Empty;
    private TextDocument? _pendingDocument;
    private bool _isRegexMode;

    public TextHighlightService()
    {
        // Debounce timer to avoid excessive highlighting during fast typing
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300) // 300ms delay
        };
        _debounceTimer.Tick += OnDebounceTimerTick;
    }

    public void SetTextMarkerService(ITextMarkerService textMarkerService)
    {
        _textMarkerService = textMarkerService;
    }

    public void SetRegexMode(bool isRegexMode)
    {
        _isRegexMode = isRegexMode;
    }

    public void HighlightText(string searchText, TextDocument document)
    {
        // Store pending request and restart debounce timer
        _pendingSearchText = searchText;
        _pendingDocument = document;
        
        _debounceTimer?.Stop();
        _debounceTimer?.Start();
    }

    private void OnDebounceTimerTick(object? sender, EventArgs e)
    {
        _debounceTimer?.Stop();
        
        if (_pendingDocument != null)
        {
            PerformHighlight(_pendingSearchText, _pendingDocument);
        }
    }

    private void PerformHighlight(string searchText, TextDocument document)
    {
        ClearHighlights();

        if (string.IsNullOrEmpty(searchText) || _textMarkerService == null) 
            return;

        try
        {
            Regex regex;
            
            if (_isRegexMode)
            {
                // Use raw regex pattern
                regex = new Regex(searchText, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            else
            {
                // Escape special characters for literal string search
                regex = new Regex(Regex.Escape(searchText), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            
            var matches = regex.Matches(document.Text);

            foreach (Match match in matches)
            {
                var marker = _textMarkerService.Create(match.Index, match.Length);
                marker.BackgroundColor = System.Windows.Media.Colors.Yellow;
                marker.ForegroundColor = System.Windows.Media.Colors.Black;
                _markers.Add(marker);
            }
        }
        catch (Exception)
        {
            // Ignore regex errors for invalid patterns
            // TODO: Add error callback for invalid regex patterns
        }
    }

    public void HighlightTextImmediate(string searchText, TextDocument document)
    {
        // For immediate highlighting without debouncing
        _debounceTimer?.Stop();
        PerformHighlight(searchText, document);
    }

    public int GetMatchCount()
    {
        return _markers.Count;
    }

    public void ClearHighlights()
    {
        if (_textMarkerService == null) return;

        foreach (var marker in _markers)
        {
            _textMarkerService.Remove(marker);
        }
        _markers.Clear();
    }

    public void Dispose()
    {
        _debounceTimer?.Stop();
        _debounceTimer = null;
        ClearHighlights();
    }
}

// Interface for text marker service
public interface ITextMarker
{
    int StartOffset { get; }
    int EndOffset { get; }
    int Length { get; }
    System.Windows.Media.Color? BackgroundColor { get; set; }
    System.Windows.Media.Color? ForegroundColor { get; set; }
    string ToolTip { get; set; }
}

public interface ITextMarkerService
{
    ITextMarker Create(int startOffset, int length);
    void Remove(ITextMarker marker);
    void RemoveAll(Predicate<ITextMarker> predicate);
    IEnumerable<ITextMarker> GetMarkersAtOffset(int offset);
}