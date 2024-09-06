internal class CliPromptService
{
    internal string PrintSingleSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var menuHeight = options.Length + 1;
        var menuStartRow = Console.CursorTop;

        var selectedIndex = 0;
        while (true)
        {
            menuStartRow = GetMenuStartRow(menuStartRow, menuHeight);
            description ??= "Use arrows or j/k to move";

            // Reset cursor to initial position and print the menu
            Console.SetCursorPosition(0, menuStartRow);
            Console.Write(title);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($" [{description}]");
            Console.ResetColor();

            // Display the options and highlight the selected one
            for (int i = 0; i < options.Count(); i++)
            {
                if (i == selectedIndex)
                {
                    // Highlight the currently selected option
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }

            // Capture key press without displaying it in the console
            var key = Console.ReadKey(true);

            // Handle navigation
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.K)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex = options.Length - 1;
                }
            }
            else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.J)
            {
                selectedIndex++;
                if (selectedIndex >= options.Length)
                {
                    selectedIndex = 0;
                }
            }
            // Handle selection
            else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                return options[selectedIndex];
            }
        }
    }

    internal string[] PrintMultiSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var menuHeight = options.Length + 1;
        var menuStartRow = Console.CursorTop;

        var selectedIndexes = new List<int>();
        var cursorIndex = 0;
        while (true)
        {
            menuStartRow = GetMenuStartRow(menuStartRow, menuHeight);
            description ??= "Use arrows or j/k to move and space to select"; // , type / to filter";

            // Reset cursor to initial position and print the menu
            Console.SetCursorPosition(0, menuStartRow);
            Console.Write(title);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($" [{description}]");
            Console.ResetColor();

            // Display the options and highlight the selected one
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

            // Capture key press without displaying it in the console
            var key = Console.ReadKey(true);

            // Handle navigation
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.K)
            {
                cursorIndex--;
                if (cursorIndex < 0)
                {
                    cursorIndex = options.Length - 1;
                }
            }
            else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.J)
            {
                cursorIndex++;
                if (cursorIndex >= options.Length)
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
                    output.Add(options[index]);
                }
                return output.ToArray();
            }
        }
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
}
