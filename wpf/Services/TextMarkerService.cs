using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using wpf.Services;

namespace wpf.Services;

public class TextMarkerService : DocumentColorizingTransformer, ITextMarkerService
{
    private readonly TextDocument _document;
    private readonly List<TextMarker> _markers = new();
    private TextView? _textView;

    public TextMarkerService(TextDocument document)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
    }

    public void SetTextView(TextView textView)
    {
        _textView = textView;
    }

    public ITextMarker Create(int startOffset, int length)
    {
        if (startOffset < 0 || startOffset > _document.TextLength)
            throw new ArgumentOutOfRangeException(nameof(startOffset));
        if (length < 0 || startOffset + length > _document.TextLength)
            throw new ArgumentOutOfRangeException(nameof(length));

        var marker = new TextMarker(this, startOffset, length);
        _markers.Add(marker);
        
        // Trigger immediate redraw
        RequestRedraw();
        return marker;
    }

    public void Remove(ITextMarker marker)
    {
        if (marker is TextMarker textMarker && _markers.Contains(textMarker))
        {
            _markers.Remove(textMarker);
            RequestRedraw();
        }
    }

    public void RemoveAll(Predicate<ITextMarker> predicate)
    {
        var removed = _markers.RemoveAll(m => predicate(m));
        if (removed > 0)
        {
            RequestRedraw();
        }
    }

    public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
    {
        return _markers.Where(m => m.StartOffset <= offset && offset <= m.EndOffset).Cast<ITextMarker>();
    }

    private void RequestRedraw()
    {
        if (_textView != null)
        {
            // Force immediate redraw of the text view
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _textView.Redraw();
            }), System.Windows.Threading.DispatcherPriority.Render);
        }
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        var markersInLine = _markers.Where(m => 
            (m.StartOffset <= line.EndOffset && m.EndOffset >= line.Offset)).ToList();

        foreach (var marker in markersInLine)
        {
            var startOffset = Math.Max(marker.StartOffset, line.Offset);
            var endOffset = Math.Min(marker.EndOffset, line.EndOffset);
            
            if (startOffset < endOffset)
            {
                ChangeLinePart(startOffset, endOffset, element =>
                {
                    if (marker.BackgroundColor.HasValue)
                        element.BackgroundBrush = new SolidColorBrush(marker.BackgroundColor.Value);
                    if (marker.ForegroundColor.HasValue)
                        element.TextRunProperties.SetForegroundBrush(new SolidColorBrush(marker.ForegroundColor.Value));
                });
            }
        }
    }

    private class TextMarker : ITextMarker
    {
        private readonly TextMarkerService _service;

        public TextMarker(TextMarkerService service, int startOffset, int length)
        {
            _service = service;
            StartOffset = startOffset;
            Length = length;
        }

        public int StartOffset { get; }
        public int EndOffset => StartOffset + Length;
        public int Length { get; }
        public Color? BackgroundColor { get; set; }
        public Color? ForegroundColor { get; set; }
        public string ToolTip { get; set; } = string.Empty;
    }
}