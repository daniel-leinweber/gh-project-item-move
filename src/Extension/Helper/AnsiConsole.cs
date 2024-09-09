using System.Runtime.InteropServices;
using Extension.Interfaces;

namespace Extension.Helper;

public class AnsiConsole : IConsole
{
    #region DllImports for ANSI sequences on Windows

    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    #endregion DllImports for ANSI sequences on Windows

    public AnsiConsole()
    {
        EnableAnsiSupport();
    }

    private void EnableAnsiSupport()
    {
        // Check if the current OS is Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (GetConsoleMode(handle, out uint mode))
            {
                SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }
        }
        // On Linux/macOS, ANSI support is typically enabled by default, so no action is needed.
    }

    public void Write(string? message = null, bool clearScreen = false, ConsoleColor? color = null)
    {
        if (clearScreen)
        {
            ClearScreen();
        }

        if (color.HasValue)
        {
            Write(message, color.Value);
        }
        else
        {
            Console.Write(message);
        }
    }

    private void Write(string? message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }

    public void WriteLine(
        string? message = null,
        bool clearScreen = false,
        ConsoleColor? color = null
    )
    {
        if (clearScreen)
        {
            ClearScreen();
        }

        if (color.HasValue)
        {
            WriteLine(message, color.Value);
        }
        else
        {
            Console.WriteLine(message);
        }
    }

    private void WriteLine(string? message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        return System.Console.ReadKey(intercept);
    }

    private void ClearScreen()
    {
        // Move cursor to the top-left corner and clear the screen
        Console.Write("\u001b[H\u001b[J");
    }
}
