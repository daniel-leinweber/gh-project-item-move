using System.Diagnostics;
using System.Text;
using Extension.Models;
using Newtonsoft.Json;

namespace Extension.Services;

internal class GitHubService
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
        catch (Exception)
        {
            throw;
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

            throw new Exception(
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
            Console.WriteLine(
                "Could not determine current user, please make sure to authenticate with the gh CLI or provide the '--owner' option!"
            );
            Environment.Exit(1);
        }

        return CurrentUser;
    }

    internal Project? GetProject(string projectName, string? owner = null)
    {
        Project? output = null;

        try
        {
            owner ??= GetCurrentUser();

            var shellOutput = RunShellCommand(
                $$"""
                project list --owner {{owner}} --format json --jq .[] | select(.title == "{{projectName}}") | {id: .id, number: .number, title: .title}
                """
            );
            shellOutput = shellOutput.Substring(0, shellOutput.LastIndexOf("]") + 1);

            var projects = JsonConvert.DeserializeObject<List<Project>>(shellOutput);
            output = projects?.FirstOrDefault(x =>
                x.Title.Equals(projectName, StringComparison.InvariantCultureIgnoreCase)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not find project with name \"{projectName}\".");
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return output;
    }

    internal ProjectItem? GetIssue(Project project, int issueNumber, string? owner = null)
    {
        ProjectItem? output = null;

        try
        {
            owner ??= GetCurrentUser();

            var shellOutput = RunShellCommand(
                $$"""
                project item-list {{project.Number}} --owner {{owner}} --format json --jq .[]
                """
            );
            shellOutput = shellOutput.Substring(0, shellOutput.LastIndexOf("]") + 1);

            var projectItems = JsonConvert.DeserializeObject<List<ProjectItem>>(shellOutput);
            output = projectItems?.FirstOrDefault(x => x.Content.Number == issueNumber);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not find issue with number \"{issueNumber}\"");
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return output;
    }

    internal ProjectField? GetProjectField(
        int projectNumber,
        string fieldName,
        string? owner = null
    )
    {
        ProjectField? output = null;

        try
        {
            owner ??= GetCurrentUser();

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
            Console.WriteLine($"Could not find project field with name \"{fieldName}\"");
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return output;
    }

    internal bool MoveIssue(string issueId, string projectId, string fieldId, string optionId)
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            output = false;
        }

        return output;
    }
}
