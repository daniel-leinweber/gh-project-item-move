using System.Diagnostics;
using System.Text;
using Extension.Exceptions;
using Extension.Interfaces;
using Extension.Models;
using Newtonsoft.Json;

namespace Extension.Services;

internal class GitHubService : IGitHubService
{
    public string? CurrentUser { get; private set; }

    private string RunShellCommand(string? arguments = null)
    {
        using var process = new Process();
        process.StartInfo.FileName = "gh";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;

        if (string.IsNullOrEmpty(arguments) == false)
        {
            process.StartInfo.Arguments = arguments;
        }

        var standardOutput = new StringBuilder();
        process.OutputDataReceived += (sender, args) => standardOutput.AppendLine(args.Data);

        string? standardError = null;
        try
        {
            process.Start();
            process.BeginOutputReadLine();
            standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            throw new GitHubCliException(ex.Message);
        }

        if (
            process.ExitCode == 0
            || standardError.Contains(
                "cannot iterate over",
                StringComparison.InvariantCultureIgnoreCase
            )
        )
        {
            return standardOutput.ToString();
        }
        else
        {
            var message = new StringBuilder();

            if (string.IsNullOrEmpty(standardError) == false)
            {
                message.AppendLine(standardError);
            }

            if (string.IsNullOrWhiteSpace(standardOutput.ToString()) == false)
            {
                message.AppendLine("Standard output:");
                message.AppendLine(standardOutput.ToString());
            }

            throw new GitHubCliException(
                $"{Format(arguments)} finished with exit code = {process.ExitCode}: {message}"
            );
        }
    }

    private string Format(string? arguments) =>
        string.IsNullOrWhiteSpace(arguments) ? $"'gh'" : $"'gh {arguments}'";

    private string? GetCurrentUser()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(CurrentUser))
            {
                var user = JsonConvert.DeserializeObject<User>(RunShellCommand("api user"));
                CurrentUser = user?.Login;
            }
        }
        catch (Exception)
        {
            throw new UserNotFoundException();
        }

        return CurrentUser;
    }

    public IEnumerable<string> GetOwners()
    {
        var output = new List<string>();

        try
        {
            var currentUser = GetCurrentUser();
            if (currentUser is not null)
            {
                output.Add(currentUser);
            }

            var organizations = GetOrganizations();
            foreach (var organization in organizations)
            {
                output.Add(organization);
            }
        }
        catch (UserNotFoundException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new OwnersNotFoundException();
        }

        return output;
    }

    private IEnumerable<string> GetOrganizations()
    {
        var output = new List<string>();

        var shellOutput = RunShellCommand("org list");
        if (string.IsNullOrWhiteSpace(shellOutput) == false)
        {
            var lines = shellOutput.Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );

            foreach (var line in lines)
            {
                output.Add(line);
            }
        }

        return output;
    }

    public List<Project> GetProjects(string owner)
    {
        var output = new List<Project>();

        try
        {
            var shellOutput = RunShellCommand(
                $$"""
                project list --owner {{owner}} --limit 1000 --format json
                """
            );
            shellOutput = shellOutput.Substring(shellOutput.IndexOf("["));
            shellOutput = shellOutput.Substring(0, shellOutput.LastIndexOf("]") + 1);

            var projects = JsonConvert.DeserializeObject<List<Project>>(shellOutput);

            if (projects is not null && projects.Any())
            {
                foreach (var project in projects)
                {
                    project.Owner = owner!;
                }

                output = projects;
            }
        }
        catch (Exception ex)
        {
            throw new ProjectsNotFoundException(owner, ex.Message);
        }

        return output;
    }

    public List<ProjectItem> GetIssues(Project project, string owner)
    {
        var output = new List<ProjectItem>();

        try
        {
            var shellOutput = RunShellCommand(
                $$"""
                project item-list {{project.Number}} --owner {{owner}} --limit 1000 --format json --jq .[]
                """
            );
            shellOutput = shellOutput.Substring(0, shellOutput.LastIndexOf("]") + 1);

            output = JsonConvert.DeserializeObject<List<ProjectItem>>(shellOutput);
        }
        catch (Exception ex)
        {
            throw new IssuesNotFoundException(project.Title, owner, ex.Message);
        }

        return output ?? new();
    }

    public ProjectField? GetProjectField(int projectNumber, string fieldName, string owner)
    {
        ProjectField? output = null;

        try
        {
            var shellOutput = RunShellCommand(
                $$"""
                project field-list {{projectNumber}} --owner {{owner}} --format json --jq .[]
                """
            );
            shellOutput = shellOutput.Substring(0, shellOutput.LastIndexOf("]") + 1);

            var projectFields = JsonConvert.DeserializeObject<List<ProjectField>>(shellOutput);
            output = projectFields?.FirstOrDefault(x =>
                x.Name.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)
            );
        }
        catch (Exception ex)
        {
            throw new ProjectFieldNotFoundException(fieldName, ex.Message);
        }

        return output;
    }

    public bool MoveIssue(string issueId, string projectId, string fieldId, string optionId)
    {
        var output = true;

        try
        {
            var shellOutput = RunShellCommand(
                $$"""
                project item-edit --project-id {{projectId}} --id {{issueId}} --field-id {{fieldId}} --single-select-option-id {{optionId}}
                """
            );
        }
        catch (Exception)
        {
            output = false;
        }

        return output;
    }
}
