using Extension.Interfaces;

namespace Extension.Helper;

public class PromptRenderer
{
    private readonly IConsole _console;

    public PromptRenderer(IConsole console)
    {
        _console = console;
    }

    public void DisplayMenuHeader(
        string title,
        string description,
        string filterValue,
        int totalPages
    )
    {
        _console.Write(title, clearScreen: true, color: ConsoleColor.White);

        if (string.IsNullOrWhiteSpace(filterValue) == false)
        {
            _console.Write($" /{filterValue}");
            description = "Press ESC to reset filter and Enter to accept";
        }

        if (totalPages > 1 && string.IsNullOrWhiteSpace(filterValue) == true)
        {
            description += ", PageUp/Ctrl+U previous page, PageDown/Ctrl+D next page";
        }

        _console.WriteLine($" [{description}]", color: ConsoleColor.Blue);
    }

    public void DisplayFooter(int currentPage, int totalPages)
    {
        if (totalPages > 1)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            _console.Write($"Page {currentPage + 1} of {totalPages}", color: ConsoleColor.Blue);
        }
    }

    public void DisplaySingleSelectOptions(
        List<string> options,
        int selectedIndex,
        int startIndex,
        int endIndex
    )
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i == selectedIndex)
            {
                _console.WriteLine(
                    $"> {options[i]}".PadRight(Console.WindowWidth - 1),
                    color: ConsoleColor.Green
                );
            }
            else
            {
                _console.WriteLine($"  {options[i]}".PadRight(Console.WindowWidth - 1));
            }
        }
    }

    public void DisplayMultiSelectOptions(
        List<string> options,
        List<int> selectedIndexes,
        int cursorIndex,
        int startIndex,
        int endIndex
    )
    {
        for (int i = startIndex; i < endIndex; i++)
        {
            if (i == cursorIndex)
            {
                _console.Write("> ");
            }
            else
            {
                _console.Write("  ");
            }

            if (selectedIndexes.Contains(i))
            {
                _console.WriteLine($"[x] {options[i]}", color: ConsoleColor.Green);
            }
            else
            {
                _console.WriteLine($"[ ] {options[i]}");
            }
        }
    }
}
