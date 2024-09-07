internal class CliPromptService
{
    internal string PrintSingleSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var menuStartRow = Console.CursorTop;

        var filteredOptions = new List<string>(options);
        var isFilterEnabled = false;
        var filterValue = string.Empty;
        var selectedIndex = 0;
        while (true)
        {
            // Reset cursor to initial position
            menuStartRow = ResetCursorPosition(menuStartRow, filteredOptions);

            // Display menu title, description and filter
            DisplayMenuHeader(
                title,
                description ??= "Use arrows or j/k to move, type / to filter",
                filterValue,
                menuStartRow
            );

            // Clear options
            ClearOptions(options, menuStartRow);

            // Display the options and highlight the selected one
            DisplaySingleSelectFilterOptions(filteredOptions, selectedIndex);

            // Capture key press without displaying it in the console
            var key = Console.ReadKey(true);

            // Handle filter mode
            if (isFilterEnabled == true)
            {
                selectedIndex = 0;

                // Exit filter mode
                if (key.Key == ConsoleKey.Enter)
                {
                    isFilterEnabled = false;
                    if (filteredOptions.Count() == 1)
                    {
                        return filteredOptions.First();
                    }
                }
                // Cancel filter mode and reset options
                else if (key.Key == ConsoleKey.Escape)
                {
                    isFilterEnabled = false;
                    filterValue = string.Empty;
                    filteredOptions = new List<string>(options);
                }
                // Handle backspace to remove the last character from the filter
                else if (key.Key == ConsoleKey.Backspace && filterValue.Length > 0)
                {
                    filterValue = filterValue.Substring(0, filterValue.Length - 1);
                    filteredOptions = options
                        .Where(x =>
                            x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase)
                        )
                        .ToList();
                }
                // Add typed character to the filter and update the filtered options
                else if (key.KeyChar != '\u0000' && char.IsControl(key.KeyChar) == false)
                {
                    filterValue += key.KeyChar;
                    filteredOptions = options
                        .Where(x =>
                            x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase)
                        )
                        .ToList();
                }
            }
            else
            {
                // Handle navigation
                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.K)
                {
                    selectedIndex--;
                    if (selectedIndex < 0)
                    {
                        selectedIndex = filteredOptions.Count() - 1;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.J)
                {
                    selectedIndex++;
                    if (selectedIndex >= filteredOptions.Count())
                    {
                        selectedIndex = 0;
                    }
                }
                // Handle selection
                else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
                {
                    return filteredOptions[selectedIndex];
                }
                // Enter filter mode
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
        var menuStartRow = Console.CursorTop;

        var filteredOptions = new List<string>(options);
        var isFilterEnabled = false;
        var filterValue = string.Empty;
        var selectedIndexes = new List<int>();
        var cursorIndex = 0;
        while (true)
        {
            // Reset cursor to initial position
            menuStartRow = ResetCursorPosition(menuStartRow, filteredOptions);

            // Display menu title, description and filter
            DisplayMenuHeader(
                title,
                description ??= "Use arrows or j/k to move and space to select, type / to filter",
                filterValue,
                menuStartRow
            );

            // Clear options
            ClearOptions(options, menuStartRow);

            // Display the options and highlight the selected one
            DisplayMultiSelectFilterOptions(filteredOptions, selectedIndexes, cursorIndex);

            // Capture key press without displaying it in the console
            var key = Console.ReadKey(true);

            // Handle filter mode
            if (isFilterEnabled == true)
            {
                cursorIndex = 0;

                // Exit filter mode
                if (key.Key == ConsoleKey.Enter)
                {
                    isFilterEnabled = false;
                    filterValue = string.Empty;
                    selectedIndexes.Clear();
                }
                // Cancel filter mode and reset options
                else if (key.Key == ConsoleKey.Escape)
                {
                    isFilterEnabled = false;
                    filterValue = string.Empty;
                    filteredOptions = new List<string>(options);
                    selectedIndexes.Clear();
                }
                // Handle backspace to remove the last character from the filter
                else if (key.Key == ConsoleKey.Backspace && filterValue.Length > 0)
                {
                    filterValue = filterValue.Substring(0, filterValue.Length - 1);
                    filteredOptions = options
                        .Where(x =>
                            x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase)
                        )
                        .ToList();
                }
                // Add typed character to the filter and update the filtered options
                else if (key.KeyChar != '\u0000' && char.IsControl(key.KeyChar) == false)
                {
                    filterValue += key.KeyChar;
                    filteredOptions = options
                        .Where(x =>
                            x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase)
                        )
                        .ToList();
                }
            }
            else
            {
                // Handle navigation
                if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.K)
                {
                    cursorIndex--;
                    if (cursorIndex < 0)
                    {
                        cursorIndex = filteredOptions.Count() - 1;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.J)
                {
                    cursorIndex++;
                    if (cursorIndex >= filteredOptions.Count())
                    {
                        cursorIndex = 0;
                    }
                }
                // Handle selection
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    if (selectedIndexes.Contains(cursorIndex) == true)
                    {
                        selectedIndexes.Remove(cursorIndex);
                    }
                    else
                    {
                        selectedIndexes.Add(cursorIndex);
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    var output = new List<string>();
                    foreach (var index in selectedIndexes)
                    {
                        output.Add(filteredOptions[index]);
                    }
                    return output.ToArray();
                }
                // Enter filter mode
                else if (key.KeyChar == '/')
                {
                    isFilterEnabled = true;
                    filterValue = string.Empty;
                }
            }
        }
    }

    private int ResetCursorPosition(int menuStartRow, List<string> filteredOptions)
    {
        var menuHeight = filteredOptions.Count() + 1;
        menuStartRow = GetMenuStartRow(menuStartRow, menuHeight);
        Console.SetCursorPosition(0, menuStartRow);
        return menuStartRow;
    }

    private int GetMenuStartRow(int menuStartRow, int menuHeight)
    {
        int currentRow = Console.CursorTop;

        // If the menu start position + height is beyond the window, scroll up by printing empty lines
        if (menuStartRow + menuHeight >= Console.WindowHeight)
        {
            // Calculate how many lines we need to move up
            int scrollLines = (menuStartRow + menuHeight) - Console.WindowHeight + 1;

            // Scroll up by printing blank lines
            for (int i = 0; i < scrollLines; i++)
            {
                Console.WriteLine();
            }

            // Adjust the menu start row accordingly
            menuStartRow -= scrollLines;
        }

        return menuStartRow;
    }

    private void DisplayMenuHeader(
        string title,
        string description,
        string filterValue,
        int menuStartRow
    )
    {
        Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
        Console.SetCursorPosition(0, menuStartRow);
        Console.Write(title);
        if (string.IsNullOrWhiteSpace(filterValue) == false)
        {
            Console.Write($" /{filterValue}");
            description = "Press ESC to reset filter and Enter to accept";
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($" [{description}]");
        Console.ResetColor();
    }

    private void ClearOptions(string[] options, int menuStartRow)
    {
        foreach (var option in options)
        {
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }
        Console.SetCursorPosition(0, menuStartRow + 1);
    }

    private void DisplaySingleSelectFilterOptions(List<string> options, int selectedIndex)
    {
        for (int i = 0; i < options.Count(); i++)
        {
            if (i == selectedIndex)
            {
                // Highlight the currently selected option
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
        int cursorIndex
    )
    {
        for (int i = 0; i < options.Count(); i++)
        {
            // Display indicator of current cursor position
            if (i == cursorIndex)
            {
                Console.Write("> ");
            }
            else
            {
                Console.Write("  ");
            }

            // Highlight the currently selected option
            if (selectedIndexes.Contains(i) == true)
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