using Extension.Helper;
using Extension.Interfaces;
using Extension.Models;

namespace Extension.Services;

internal class CliPromptService
{
    private readonly IConsole _console;
    private readonly IInputHandler _inputHandler;

    public CliPromptService(IConsole console, IInputHandler inputHandler)
    {
        _console = console;
        _inputHandler = inputHandler;
    }

    internal string PrintSingleSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var state = new PromptState(options, Console.WindowHeight - 3);

        while (true)
        {
            var (startIndex, endIndex) = GetPageBounds(
                state.CurrentPage,
                state.PageSize,
                state.FilteredOptions.Count
            );

            DisplayMenuHeader(
                title,
                description ?? "Use arrows or j/k to move, type / to filter",
                state.FilterValue,
                state.TotalPages
            );

            DisplaySingleSelectFilterOptions(
                state.FilteredOptions,
                state.SelectedIndex,
                startIndex,
                endIndex
            );

            DisplayFooter(state.CurrentPage, state.TotalPages);

            var key = _console.ReadKey(true);

            if (state.IsFilterEnabled)
            {
                (state.IsFilterEnabled, state.FilterValue, state.FilteredOptions) =
                    _inputHandler.HandleFilterMode(
                        key,
                        state.FilterValue,
                        options,
                        state.FilteredOptions
                    );

                if (state.IsFilterEnabled == false && state.FilteredOptions.Count == 1)
                {
                    return state.FilteredOptions.First();
                }
            }
            else
            {
                (state.SelectedIndex, state.CurrentPage) = _inputHandler.HandleNavigation(
                    key,
                    state.SelectedIndex,
                    state.FilteredOptions.Count,
                    state.CurrentPage,
                    state.TotalPages,
                    state.PageSize
                );

                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
                {
                    return state.FilteredOptions[state.SelectedIndex];
                }
                else if (key.KeyChar == '/')
                {
                    state.IsFilterEnabled = true;
                    state.FilterValue = string.Empty;
                }
            }
        }
    }

    internal string[] PrintMultiSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var state = new PromptState(options, Console.WindowHeight - 3);

        while (true)
        {
            var (startIndex, endIndex) = GetPageBounds(
                state.CurrentPage,
                state.PageSize,
                state.FilteredOptions.Count
            );

            DisplayMenuHeader(
                title,
                description ?? "Use arrows or j/k to move and space to select, type / to filter",
                state.FilterValue,
                state.TotalPages
            );

            DisplayMultiSelectFilterOptions(
                state.FilteredOptions,
                state.SelectedIndexes,
                state.CursorIndex,
                startIndex,
                endIndex
            );

            DisplayFooter(state.CurrentPage, state.TotalPages);

            var key = _console.ReadKey(true);

            if (state.IsFilterEnabled)
            {
                (state.IsFilterEnabled, state.FilterValue, state.FilteredOptions) =
                    _inputHandler.HandleFilterMode(
                        key,
                        state.FilterValue,
                        options,
                        state.FilteredOptions
                    );

                if (state.IsFilterEnabled == false)
                {
                    state.FilterValue = string.Empty;
                    state.SelectedIndexes.Clear();
                }
            }
            else
            {
                (state.CursorIndex, state.CurrentPage) = _inputHandler.HandleNavigation(
                    key,
                    state.CursorIndex,
                    state.FilteredOptions.Count,
                    state.CurrentPage,
                    state.TotalPages,
                    state.PageSize
                );

                if (key.Key == ConsoleKey.Spacebar)
                {
                    _inputHandler.ToggleSelection(state.CursorIndex, state.SelectedIndexes);
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return state
                        .SelectedIndexes.Select(index => state.FilteredOptions[index])
                        .ToArray();
                }
                else if (key.KeyChar == '/')
                {
                    state.IsFilterEnabled = true;
                    state.FilterValue = string.Empty;
                }
            }
        }
    }

    internal void PrintSelection(
        string owner,
        string project,
        List<ProjectItem> projectItems,
        string targetColumn
    )
    {
        var issues = string.Join(
            " | ",
            projectItems.Select(x => $"{x.Title} (#{x.Content.Number})")
        );

        Console.Clear();
        PrintSelection("Owner", owner);
        PrintSelection("Project", project);
        PrintSelection(projectItems.Count > 1 ? "Issues" : "Issue", issues);
        PrintSelection("Column", targetColumn);
        Console.WriteLine();
    }

    private void PrintSelection(string title, string? content)
    {
        if (string.IsNullOrWhiteSpace(content) == true)
        {
            return;
        }

        _console.Write("?", color: ConsoleColor.Green);
        _console.Write($" {title}:", color: ConsoleColor.White);
        _console.WriteLine($" {content}");
    }

    private (int startIndex, int endIndex) GetPageBounds(
        int currentPage,
        int pageSize,
        int optionsCount
    )
    {
        var startIndex = currentPage * pageSize;
        var endIndex = Math.Min(startIndex + pageSize, optionsCount);

        return (startIndex, endIndex);
    }

    private void DisplayMenuHeader(
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

    private void DisplayFooter(int currentPage, int totalPages)
    {
        if (totalPages > 1)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            _console.Write($"Page {currentPage + 1} of {totalPages}", color: ConsoleColor.Blue);
        }
    }

    private void DisplaySingleSelectFilterOptions(
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

    private void DisplayMultiSelectFilterOptions(
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
