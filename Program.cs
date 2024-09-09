using CommandLine;
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
                var gh = new GitHubService();
                var cli = new CliPromptService();

                // Get owner
                var owner = GetOwner(options.Owner, gh, cli);
                if (owner is null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error: No owner! Please provide a valid owner.");
                    Environment.Exit(1);
                }

                // Get project data
                var project = GetProject(options.ProjectName, owner, gh, cli);
                if (project is null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error: No Project! Please provide a valid project name.");
                    Environment.Exit(1);
                }

                // Get issue to move
                var issues = GetIssues(options.IssueId, project, gh, cli);
                if (issues is null || issues.Any() == false)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error: No issue! Please provide a valid issue number.");
                    Environment.Exit(1);
                }

                // Get 'Status' field of project
                var projectStatusField = gh.GetProjectField(project.Number, "Status", owner);
                if (projectStatusField is null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error: Could not find 'Status' field of project!");
                    Environment.Exit(1);
                }

                // Get target column
                var column = GetTargetColumn(
                    options.ColumnName,
                    project.Number,
                    owner,
                    projectStatusField.Options,
                    gh,
                    cli
                );
                if (column is null)
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "Error: No column! Please provide a valid target column name."
                    );
                    Environment.Exit(1);
                }

                cli.PrintSelection(owner, project.Title, issues, column.Name);

                // Move items in project board
                var errors = new List<string>();
                foreach (var issue in issues)
                {
                    var isMoved = gh.MoveIssue(
                        issue.Id,
                        project.Id,
                        projectStatusField.Id,
                        column.Id
                    );

                    if (isMoved == false)
                    {
                        errors.Add($"Issue #{issue.Content.Number} could not be moved!");
                    }
                }

                // Print any errors
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error);
                    }
                    Environment.Exit(1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(
                        issues.Count > 1
                            ? "Issues moved successfully!"
                            : "Ussue moved successfully!"
                    );
                    Console.ResetColor();
                    Environment.Exit(0);
                }
            })
            .WithNotParsed(x => Environment.Exit(1));

    private static Option? GetTargetColumn(
        string? columnName,
        int projectNumber,
        string owner,
        Option[] options,
        GitHubService gh,
        CliPromptService cli
    )
    {
        Option? output = null;

        if (columnName is null)
        {
            columnName = cli.PrintSingleSelectOptions(
                "Which column would you like to move the issue(s) to?",
                options.Select(x => x.Name).ToArray()
            );
        }

        output = options.FirstOrDefault(x =>
            x.Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase)
        );

        return output;
    }

    private static string? GetOwner(string? input, GitHubService gh, CliPromptService cli)
    {
        var output = input;

        if (input is null)
        {
            var owners = gh.GetOwners();
            if (owners.Count() > 1)
            {
                output = cli.PrintSingleSelectOptions(
                    "Which owner would you like to use?",
                    owners.ToArray()
                );
            }
            else
            {
                output = owners.FirstOrDefault();
            }
        }

        return output;
    }

    private static Project? GetProject(
        string? projectName,
        string owner,
        GitHubService gh,
        CliPromptService cli
    )
    {
        Project? output = null;

        var projects = gh.GetProjects(owner);

        if (projectName is null)
        {
            projectName = cli.PrintSingleSelectOptions(
                "Which project would you like to use?",
                projects.OrderBy(x => x.Title).Select(x => x.Title).ToArray()
            );
        }

        output = projects.FirstOrDefault(x =>
            x.Title.Equals(projectName, StringComparison.InvariantCultureIgnoreCase)
        );

        return output;
    }

    private static List<ProjectItem> GetIssues(
        int? issueId,
        Project project,
        GitHubService gh,
        CliPromptService cli
    )
    {
        var output = new List<ProjectItem>();

        var issues = gh.GetIssues(project, project.Owner);

        if (issueId is null)
        {
            var selectedIssues = cli.PrintMultiSelectOptions(
                    "Which issue(s) would you like to update?",
                    issues
                        .OrderBy(x => x.Content.Number)
                        .Select(x => $"{x.Title} (#{x.Content.Number})")
                        .ToArray()
                )
                .ToList();

            output = issues
                .Where(x => selectedIssues.Contains($"{x.Title} (#{x.Content.Number})"))
                .ToList();
        }
        else
        {
            output = issues.Where(x => x.Content.Number == issueId).ToList();
        }

        return output;
    }
}
