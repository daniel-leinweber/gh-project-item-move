namespace Extension.Interfaces;

public interface ICliPromptService
{
    string[] PrintMultiSelectMenu(string title, string[] options, string? description = null);
    string PrintSingleSelectMenu(string title, string[] options, string? description = null);
}
