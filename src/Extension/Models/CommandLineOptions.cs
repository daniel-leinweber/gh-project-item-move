using CommandLine;
using CommandLine.Text;

namespace Extension.Models;

public class CommandLineOptions
{
    [Option(
        shortName: 'o',
        longName: "owner",
        Required = false,
        HelpText = "Owner of the project (optional)"
    )]
    public string? Owner { get; set; }

    [Option(
        shortName: 'p',
        longName: "project",
        Required = false,
        HelpText = "Name of the Project (optional)"
    )]
    public string? ProjectName { get; set; }

    [Option(
        shortName: 'i',
        longName: "issue",
        Required = false,
        HelpText = "ID of the issue to move (optional)"
    )]
    public int? IssueId { get; set; }

    [Option(
        shortName: 'c',
        longName: "column",
        Required = false,
        HelpText = "Column Name to move the issue to (optional)"
    )]
    public string? ColumnName { get; set; }

    [Usage(ApplicationAlias = "gh project-item-move")]
    public static IEnumerable<Example> Examples =>
        new List<Example>
        {
            new Example(
                "Choose owner, project, issue(s) and target column interactively",
                new CommandLineOptions()
            ),
            new Example(
                "Move issue with ID '123' to the 'Done' column of the project 'MyProject'",
                new CommandLineOptions
                {
                    Owner = "daniel-leinweber",
                    ProjectName = "MyProject",
                    IssueId = 123,
                    ColumnName = "Done",
                }
            ),
        };
}
