namespace Extension.Models;

public class AppResult
{
    public ExitCode ExitCode { get; set; }
    public string Message { get; set; }

    public AppResult(string message, ExitCode exitCode = ExitCode.Success)
    {
        ExitCode = exitCode;
        Message = message;
    }
}
