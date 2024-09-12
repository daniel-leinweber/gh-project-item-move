using Extension.Interfaces;
using Extension.Services;
using Moq;

namespace Extension.Tests.Services;

[TestClass]
public class CliPromptServiceTests
{
    private Mock<IConsole>? _mockConsole;
    private Mock<IInputHandler>? _mockInputHandler;

    private CliPromptService? _cliPromptService;

    [TestInitialize]
    public void Setup()
    {
        _mockConsole = new Mock<IConsole>();
        _mockInputHandler = new Mock<IInputHandler>();
        _cliPromptService = new CliPromptService(_mockConsole.Object, _mockInputHandler.Object);
    }

    [TestMethod]
    public void PrintSingleSelectMenu_ShouldReturnSelectedOption_WhenUserPressesEnter()
    {
        // Arrange
        var title = "Select an option";
        var options = new[] { "Option 1", "Option 2", "Option 3" };
        var expectedOption = "Option 2";

        // Simulate user pressing the Down Arrow key and then Enter
        _mockConsole!
            .SetupSequence(c => c.ReadKey(It.IsAny<bool>()))
            .Returns(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)) // Move to next option
            .Returns(new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false)); // Select the current option

        // Simulate navigation to the second option (index 1)
        _mockInputHandler!
            .SetupSequence(h =>
                h.HandleNavigation(
                    It.IsAny<ConsoleKeyInfo>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .Returns((1, 0)) // First call - Down arrow moves to index 1
            .Returns((1, 0)); // Second call - Enter key pressed

        // Act
        var result = _cliPromptService!.PrintSingleSelectMenu(title, options);

        // Assert
        Assert.AreEqual(expectedOption, result);
    }

    [TestMethod]
    public void PrintMultiSelectMenu_ShouldReturnSelectedOptions_WhenUserSelectsMultipleAndPressesEnter()
    {
        // Arrange
        var title = "Select multiple options";
        var options = new[] { "Option 1", "Option 2", "Option 3" };
        var expectedOptions = new[] { "Option 1", "Option 3" };

        // Simulate user input: Spacebar to select, DownArrow to navigate, Enter to confirm.
        _mockConsole!
            .SetupSequence(c => c.ReadKey(It.IsAny<bool>()))
            .Returns(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false)) // Select first option
            .Returns(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)) // Move down to index 1
            .Returns(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)) // Move down to index 2
            .Returns(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false)) // Select third option
            .Returns(new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false)); // Press Enter to confirm

        // Simulate navigation behavior to the correct indexes.
        _mockInputHandler!
            .SetupSequence(h =>
                h.HandleNavigation(
                    It.IsAny<ConsoleKeyInfo>(),
                    It.IsAny<int>(), // Selected index
                    It.IsAny<int>(), // Options count
                    It.IsAny<int>(), // Current page
                    It.IsAny<int>(), // Total pages
                    It.IsAny<int>() // Page size
                )
            )
            .Returns((0, 0)) // First call - Spacebar
            .Returns((0, 0)) // Second call - Down arrow
            .Returns((0, 0)) // Third call - Down arrow
            .Returns((2, 0)) // Fourth call - Spacebar
            .Returns((2, 0)); // Fifth call - Enter

        // Simulate selecting items
        _mockInputHandler!
            .Setup(h => h.ToggleSelection(It.IsAny<int>(), It.IsAny<List<int>>()))
            .Callback<int, List<int>>((index, list) => list.Add(index));

        // Act
        var result = _cliPromptService!.PrintMultiSelectMenu(title, options);

        // Assert the selected options are as expected.
        CollectionAssert.AreEqual(expectedOptions, result);
    }
}
