namespace wpf.Models;

public class DocumentModel
{
    public string? FilePath { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsDirty { get; set; }
}
