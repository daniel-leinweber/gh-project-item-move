using CommandLine;
using Extension.Helper;
using Extension.Interfaces;
using Extension.Models;
using Extension.Services;

namespace Extension;

class Program
{
    static void Main(string[] args) =>
        Parser
            .Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(options =>
            {
                IConsole console = new AnsiConsole();
                IGitHubService gh = new GitHubService();
                IInputHandler inputHandler = new ConsoleInputHandler();
                ICliPromptService cli = new CliPromptService(console, inputHandler);

                var appService = new AppService(gh, cli, console);
                var result = appService.Run(options);

                if (result.ExitCode == ExitCode.Error)
                {
                    console.WriteLine(result.Message, color: ConsoleColor.Red);
                }
                else
                {
                    console.WriteLine(result.Message, color: ConsoleColor.Blue);
                }

                Environment.Exit((int)result.ExitCode);
            })
            .WithNotParsed(x => Environment.Exit(1));
}
