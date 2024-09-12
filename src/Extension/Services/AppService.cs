using System.Text;
using Extension.Interfaces;
using Extension.Models;

namespace Extension.Services;

public class AppService
{
    private readonly IGitHubService _gh;
    private readonly ICliPromptService _cli;
    private readonly IConsole _console;

    public AppService(IGitHubService gh, ICliPromptService cli, IConsole console)
    {
        _gh = gh;
        _cli = cli;
        _console = console;
    }

    public AppResult Run(CommandLineOptions options)
    {
        try
        {
            // Get owner
            var owner = GetOwner(options.Owner);
            if (owner is null)
            {
                return new AppResult(
                    "Error: No owner! Please provide a valid owner.",
                    ExitCode.Error
                );
            }

            // Get project data
            var project = GetProject(options.ProjectName, owner);
            if (project is null)
            {
                return new AppResult(
                    "Error: No Project! Please provide a valid project name.",
                    ExitCode.Error
                );
            }

            // Get issue to move
            var issues = GetIssues(options.IssueId, project);
            if (issues is null || issues.Any() == false)
            {
                return new AppResult(
                    "Error: No issue! Please provide a valid issue number.",
                    ExitCode.Error
                );
            }

            // Get 'Status' field of project
            var projectStatusField = _gh.GetProjectField(project.Number, "Status", owner);
            if (projectStatusField is null)
            {
                return new AppResult(
                    "Error: Could not find 'Status' field of project!",
                    ExitCode.Error
                );
            }

            // Get target column
            var column = GetTargetColumn(
                options.ColumnName,
                project.Number,
                owner,
                projectStatusField.Options
            );
            if (column is null)
            {
                return new AppResult(
                    "Error: No column! Please provide a valid target column name.",
                    ExitCode.Error
                );
            }

            PrintSelection(owner, project.Title, issues, column.Name);

            // Move items in project board
            var errors = new List<string>();
            foreach (var issue in issues)
            {
                var isMoved = _gh.MoveIssue(issue.Id, project.Id, projectStatusField.Id, column.Id);

                if (isMoved == false)
                {
                    errors.Add($"Issue #{issue.Content.Number} could not be moved!");
                }
            }

            // Print any errors
            if (errors.Any())
            {
                var message = new StringBuilder();
                foreach (var error in errors)
                {
                    message.AppendLine(error);
                }

                return new AppResult(message.ToString(), ExitCode.Error);
            }
            else
            {
                return new AppResult(
                    issues.Count > 1 ? "Issues moved successfully!" : "Issue moved successfully!",
                    ExitCode.Success
                );
            }
        }
        catch (Exception ex)
        {
            return new AppResult(ex.Message, ExitCode.Error);
        }
    }

    private Option? GetTargetColumn(
        string? columnName,
        int projectNumber,
        string owner,
        Option[] options
    )
    {
        Option? output = null;

        columnName ??= _cli.PrintSingleSelectMenu(
            "Which column would you like to move the issue(s) to?",
            options.Select(x => x.Name).ToArray()
        );

        output = options.FirstOrDefault(x =>
            x.Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase)
        );

        return output;
    }

    private string? GetOwner(string? input)
    {
        var output = input;

        if (input is null)
        {
            var owners = _gh.GetOwners();
            output =
                owners.Count() > 1
                    ? _cli.PrintSingleSelectMenu(
                        "Which owner would you like to use?",
                        owners.ToArray()
                    )
                    : owners.FirstOrDefault();
        }

        return output;
    }

    private Project? GetProject(string? projectName, string owner)
    {
        Project? output = null;

        var projects = _gh.GetProjects(owner);

        projectName ??= _cli.PrintSingleSelectMenu(
            "Which project would you like to use?",
            projects.OrderBy(x => x.Title).Select(x => x.Title).ToArray()
        );

        output = projects.FirstOrDefault(x =>
            x.Title.Equals(projectName, StringComparison.InvariantCultureIgnoreCase)
        );

        return output;
    }

    private List<ProjectItem> GetIssues(int? issueId, Project project)
    {
        var output = new List<ProjectItem>();

        var issues = _gh.GetIssues(project, project.Owner);

        if (issueId is null)
        {
            var selectedIssues = _cli.PrintMultiSelectMenu(
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

    private void PrintSelection(
        string owner,
        string project,
        List<ProjectItem> projectItems,
        string targetColumn
    )
    {
        var issues = string.Join(
            " | ",
            projectItems.Select(x => $"{x.Title} (#{x.Content.Number})")
        );

        Console.Clear();
        PrintSelection("Owner", owner);
        PrintSelection("Project", project);
        PrintSelection(projectItems.Count > 1 ? "Issues" : "Issue", issues);
        PrintSelection("Column", targetColumn);
        Console.WriteLine();
    }

    private void PrintSelection(string title, string? content)
    {
        if (string.IsNullOrWhiteSpace(content) == true)
        {
            return;
        }

        _console.Write("?", color: ConsoleColor.Green);
        _console.Write($" {title}:", color: ConsoleColor.White);
        _console.WriteLine($" {content}");
    }
}
