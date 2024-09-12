using Extension.Interfaces;

namespace Extension.Helper;

public class ConsoleInputHandler : IInputHandler
{
    public (
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
        if (key.Key == ConsoleKey.Enter)
        {
            return (false, filterValue, filteredOptions);
        }
        else if (key.Key == ConsoleKey.Escape)
        {
            return (false, string.Empty, new List<string>(options));
        }
        else if (key.Key == ConsoleKey.Backspace && filterValue.Length > 0)
        {
            filterValue = filterValue.Substring(0, filterValue.Length - 1);
        }
        else if (key.KeyChar != '\u0000' && char.IsControl(key.KeyChar) == false)
        {
            filterValue += key.KeyChar;
        }

        filteredOptions = options
            .Where(x => x.Contains(filterValue, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        return (true, filterValue, filteredOptions);
    }

    public (int selectedIndex, int currentPage) HandleNavigation(
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

    public List<int> ToggleSelection(int index, List<int> selectedIndexes)
    {
        if (selectedIndexes.Contains(index))
        {
            selectedIndexes.Remove(index);
        }
        else
        {
            selectedIndexes.Add(index);
        }

        return selectedIndexes;
    }
}
