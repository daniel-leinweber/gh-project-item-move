internal class CliPromptService
{
    internal string PrintSingleSelectOptions(
        string title,
        string[] options,
        string? description = null
    )
    {
        var cursorPosition = Console.CursorTop;
        var selectedIndex = 0;
        while (true)
        {
            description ??= "Use arrows or j/k to move";

            // Reset cursor to initial position and print the menu
            Console.SetCursorPosition(0, cursorPosition);
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
        var cursorPosition = Console.CursorTop;
        var selectedIndexes = new List<int>();
        var cursorIndex = 0;
        while (true)
        {
            description ??= "Use arrows or j/k to move and space to select"; // , type / to filter";

            // Reset cursor to initial position and print the menu
            Console.SetCursorPosition(0, cursorPosition);
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
}
