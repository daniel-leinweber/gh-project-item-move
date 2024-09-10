namespace Extension.Helper;

public class PromptState
{
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<string> FilteredOptions { get; set; }
    public bool IsFilterEnabled { get; set; }
    public string FilterValue { get; set; }
    public int SelectedIndex { get; set; }
    public List<int> SelectedIndexes { get; set; }
    public int CursorIndex { get; set; }

    public PromptState(string[] options, int pageSize)
    {
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)options.Length / pageSize);
        CurrentPage = 0;
        FilteredOptions = new List<string>(options);
        IsFilterEnabled = false;
        FilterValue = string.Empty;
        SelectedIndex = 0;
        SelectedIndexes = new List<int>();
        CursorIndex = 0;
    }
}
