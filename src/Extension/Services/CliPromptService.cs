using Extension.Helper;
using Extension.Interfaces;

namespace Extension.Services;

internal class CliPromptService
{
    private readonly IConsole _console;
    private readonly IInputHandler _inputHandler;
    private readonly PromptRenderer _renderer;

    public CliPromptService(IConsole console, IInputHandler inputHandler)
    {
        _console = console;
        _inputHandler = inputHandler;
        _renderer = new PromptRenderer(_console);
    }

    internal string PrintSingleSelectMenu(
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

            _renderer.DisplayMenuHeader(
                title,
                description ?? "Use arrows or j/k to move, type / to filter",
                state.FilterValue,
                state.TotalPages
            );

            _renderer.DisplaySingleSelectOptions(
                state.FilteredOptions,
                state.SelectedIndex,
                startIndex,
                endIndex
            );

            _renderer.DisplayFooter(state.CurrentPage, state.TotalPages);

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

    internal string[] PrintMultiSelectMenu(
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

            _renderer.DisplayMenuHeader(
                title,
                description ?? "Use arrows or j/k to move and space to select, type / to filter",
                state.FilterValue,
                state.TotalPages
            );

            _renderer.DisplayMultiSelectOptions(
                state.FilteredOptions,
                state.SelectedIndexes,
                state.CursorIndex,
                startIndex,
                endIndex
            );

            _renderer.DisplayFooter(state.CurrentPage, state.TotalPages);

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
}
