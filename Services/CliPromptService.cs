using Extension.Models;

internal class CliPromptService
{
    internal string PrintSingleSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var pageSize = Console.WindowHeight - 3;
        var currentPage = 0;
        var filteredOptions = new List<string>(options);
        var isFilterEnabled = false;
        var filterValue = string.Empty;
        var selectedIndex = 0;

        while (true)
        {
            var totalPages = CalculateTotalPages(filteredOptions.Count, pageSize);
            var (startIndex, endIndex) = GetPageBounds(
                currentPage,
                pageSize,
                filteredOptions.Count
            );

            ClearAndResetConsole();
            DisplayMenuHeader(
                title,
                description ?? "Use arrows or j/k to move, type / to filter",
                filterValue,
                totalPages
            );
            DisplaySingleSelectFilterOptions(filteredOptions, selectedIndex, startIndex, endIndex);
            DisplayFooter(currentPage, totalPages);

            var key = Console.ReadKey(true);

            if (isFilterEnabled)
            {
                (isFilterEnabled, filterValue, filteredOptions) = HandleFilterMode(
                    key,
                    filterValue,
                    options,
                    filteredOptions
                );

                if (isFilterEnabled == false && filteredOptions.Count == 1)
                {
                    return filteredOptions.First();
                }
            }
            else
            {
                (selectedIndex, currentPage) = HandleNavigation(
                    key,
                    selectedIndex,
                    filteredOptions.Count,
                    currentPage,
                    totalPages,
                    pageSize
                );

                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
                {
                    return filteredOptions[selectedIndex];
                }
                else if (key.KeyChar == '/')
                {
                    isFilterEnabled = true;
                    filterValue = string.Empty;
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
        var pageSize = Console.WindowHeight - 3;
        var currentPage = 0;
        var filteredOptions = new List<string>(options);
        var isFilterEnabled = false;
        var filterValue = string.Empty;
        var selectedIndexes = new List<int>();
        var cursorIndex = 0;

        while (true)
        {
            var totalPages = CalculateTotalPages(filteredOptions.Count, pageSize);
            var (startIndex, endIndex) = GetPageBounds(
                currentPage,
                pageSize,
                filteredOptions.Count
            );

            ClearAndResetConsole();
            DisplayMenuHeader(
                title,
                description ?? "Use arrows or j/k to move and space to select, type / to filter",
                filterValue,
                totalPages
            );
            DisplayMultiSelectFilterOptions(
                filteredOptions,
                selectedIndexes,
                cursorIndex,
                startIndex,
                endIndex
            );
            DisplayFooter(currentPage, totalPages);

            var key = Console.ReadKey(true);

            if (isFilterEnabled)
            {
                (isFilterEnabled, filterValue, filteredOptions) = HandleFilterMode(
                    key,
                    filterValue,
                    options,
                    filteredOptions
                );

                if (isFilterEnabled == false)
                {
                    filterValue = string.Empty;
                    selectedIndexes.Clear();
                }
            }
            else
            {
                (cursorIndex, currentPage) = HandleNavigation(
                    key,
                    cursorIndex,
                    filteredOptions.Count,
                    currentPage,
                    totalPages,
                    pageSize
                );

                if (key.Key == ConsoleKey.Spacebar)
                {
                    ToggleSelection(selectedIndexes, cursorIndex);
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return selectedIndexes.Select(index => filteredOptions[index]).ToArray();
                }
                else if (key.KeyChar == '/')
                {
                    isFilterEnabled = true;
                    filterValue = string.Empty;
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

    private static void PrintSelection(string title, string? content)
    {
        if (string.IsNullOrWhiteSpace(content) == true)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("?");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" {title}:");
        Console.ResetColor();
        Console.WriteLine($" {content}");
    }

    private int CalculateTotalPages(int optionsCount, int pageSize) =>
        (int)Math.Ceiling((double)optionsCount / pageSize);

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

    private void ClearAndResetConsole()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }

    private void ToggleSelection(List<int> selectedIndexes, int index)
    {
        if (selectedIndexes.Contains(index))
        {
            selectedIndexes.Remove(index);
        }
        else
        {
            selectedIndexes.Add(index);
        }
    }

    private (
        bool isFilterEnabled,
        string filterValue,
        List<string> filteredOptions
    ) HandleFilterMode(
        ConsoleKeyInfo key,
        string filterValue,
        string[] options,
        List<string> filteredOptions
    )
    {
        var isFilterEnabled = true;

        if (key.Key == ConsoleKey.Enter)
        {
            isFilterEnabled = false;
        }
        else if (key.Key == ConsoleKey.Escape)
        {
            isFilterEnabled = false;
            filterValue = string.Empty;
            filteredOptions = new List<string>(options);
        }
        else if (key.Key == ConsoleKey.Backspace && filterValue.Length > 0)
        {
            filterValue = filterValue.Substring(0, filterValue.Length - 1);
            filteredOptions = options
                .Where(x => x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
        else if (key.KeyChar != '\u0000' && char.IsControl(key.KeyChar) == false)
        {
            filterValue += key.KeyChar;
            filteredOptions = options
                .Where(x => x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        return (isFilterEnabled, filterValue, filteredOptions);
    }

    private (int selectedIndex, int currentPage) HandleNavigation(
        ConsoleKeyInfo key,
        int selectedIndex,
        int optionsCount,
        int currentPage,
        int totalPages,
        int pageSize
    )
    {
        if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.K)
        {
            selectedIndex--;
            if (selectedIndex < 0)
            {
                selectedIndex = optionsCount - 1;
            }
        }
        else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.J)
        {
            selectedIndex++;
            if (selectedIndex >= optionsCount)
            {
                selectedIndex = 0;
            }
        }
        else if (
            key.Key == ConsoleKey.PageUp
            || (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.U)
        )
        {
            if (currentPage > 0)
            {
                currentPage--;
                selectedIndex = Math.Max(selectedIndex - pageSize, 0);
            }
        }
        else if (
            key.Key == ConsoleKey.PageDown
            || (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.D)
        )
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;
                selectedIndex = Math.Min(selectedIndex + pageSize, optionsCount - 1);
            }
        }

        return (selectedIndex, currentPage);
    }

    private void DisplayMenuHeader(
        string title,
        string description,
        string filterValue,
        int totalPages
    )
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{title}");
        Console.ResetColor();

        if (string.IsNullOrWhiteSpace(filterValue) == false)
        {
            Console.Write($" /{filterValue}");
            description = "Press ESC to reset filter and Enter to accept";
        }

        if (totalPages > 1 && string.IsNullOrWhiteSpace(filterValue) == true)
        {
            description += ", PageUp/Ctrl+U previous page, PageDown/Ctrl+D next page";
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write($" [{description}]");
        Console.WriteLine();
        Console.ResetColor();
    }

    private void DisplayFooter(int currentPage, int totalPages)
    {
        if (totalPages > 1)
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Page {currentPage + 1} of {totalPages}");
            Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"> {options[i]}".PadRight(Console.WindowWidth - 1));
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {options[i]}".PadRight(Console.WindowWidth - 1));
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
                Console.Write("> ");
            }
            else
            {
                Console.Write("  ");
            }

            if (selectedIndexes.Contains(i))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[x] {options[i]}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"[ ] {options[i]}");
            }
        }
    }
}
