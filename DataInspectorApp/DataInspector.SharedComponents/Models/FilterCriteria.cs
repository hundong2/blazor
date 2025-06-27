using System.Collections.Generic;
using System.Linq; // Required for .Any()

namespace DataInspector.SharedComponents.Models
{
    public class FilterCriteria
    {
        public IEnumerable<string> SelectedTags { get; set; } = new List<string>();
        public ItemStatus? StatusFilter { get; set; }
        public string? SearchTerm { get; set; }

        // Helper method to quickly check if any filters are active.
        public bool IsActive()
        {
            return (SelectedTags != null && SelectedTags.Any()) ||
                   StatusFilter.HasValue ||
                   !string.IsNullOrWhiteSpace(SearchTerm);
        }
    }
}
