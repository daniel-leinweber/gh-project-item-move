namespace Extension.Interfaces;

public interface IInputHandler
{
    (bool isFilterEnabled, string filterValue, List<string> filteredOptions) HandleFilterMode(
        ConsoleKeyInfo key,
        string filterValue,
        string[] options,
        List<string> filteredOptions
    );

    (int selectedIndex, int currentPage) HandleNavigation(
        ConsoleKeyInfo key,
        int selectedIndex,
        int optionsCount,
        int currentPage,
        int totalPages,
        int pageSize
    );

    List<int> ToggleSelection(int index, List<int> selectedIndexes);
}
