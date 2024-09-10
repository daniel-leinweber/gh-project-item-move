namespace Extension.Interfaces;

public interface IConsole
{
    void Write(string? message = null, bool clearScreen = false, ConsoleColor? color = null);
    void WriteLine(string? message = null, bool clearScreen = false, ConsoleColor? color = null);
    ConsoleKeyInfo ReadKey(bool intercept = false);
}