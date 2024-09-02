using CommandLine;
using CommandLine.Text;

namespace Extension.Models;

internal class CommandLineOptions
{
    [Option(
        shortName: 'o',
        longName: "owner",
        Required = false,
        HelpText = "Owner of the project (optional)"
    )]
    public string? Owner { get; set; }

    [Option(shortName: 'p', longName: "project", Required = true, HelpText = "Name of the Project")]
    public required string ProjectName { get; set; }

    [Option(
        shortName: 'i',
        longName: "issue",
        Required = true,
        HelpText = "ID of the issue to move"
    )]
    public int IssueId { get; set; }

    [Option(
        shortName: 'c',
        longName: "column",
        Required = true,
        HelpText = "Column Name to move the issue to"
    )]
    public required string ColumnName { get; set; }

    [Usage(ApplicationAlias = "gh project-item-move")]
    public static IEnumerable<Example> Examples =>
        new List<Example>
        {
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