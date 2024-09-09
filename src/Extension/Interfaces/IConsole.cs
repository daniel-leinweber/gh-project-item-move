namespace Extension.Interfaces;

public interface IConsole
{
    void Write(string? message, bool clearScreen = false, ConsoleColor? color = null);
    void WriteLine(string? message, bool clearScreen = false, ConsoleColor? color = null);
    ConsoleKeyInfo ReadKey(bool intercept = false);
}
