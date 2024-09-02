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

                // Get project data
                var project = gh.GetProject(options.ProjectName, options.Owner);
                if (project == null)
                {
                    Console.WriteLine(
                        $"Could not find a project with the name \"{options.ProjectName}\""
                    );
                    Environment.Exit(1);
                }

                // Get issue to move
                var issue = gh.GetIssue(project, options.IssueId, options.Owner);
                if (issue == null)
                {
                    Console.WriteLine(
                        $"Could not find issue with number {options.IssueId} in project \"{options.ProjectName}\""
                    );
                    Environment.Exit(1);
                }

                // Get 'Status' field of project
                var projectStatusField = gh.GetProjectField(
                    project.Number,
                    "Status",
                    options.Owner
                );

                if (projectStatusField == null)
                {
                    Console.WriteLine(
                        $"Could not find 'Status' field in project \"{options.ProjectName}\""
                    );
                    Environment.Exit(1);
                }

                // Get option of new status
                var option = projectStatusField?.Options.FirstOrDefault(x =>
                    x.Name.Equals(options.ColumnName, StringComparison.InvariantCultureIgnoreCase)
                );

                if (option == null)
                {
                    Console.WriteLine(
                        $"Could not find option with name \"{options.ColumnName}\" for \"Status\" field of project \"{options.ProjectName}\""
                    );
                    Environment.Exit(1);
                }

                // Move item in project board
                var isMoved = gh.MoveIssue(
                    issue.Id,
                    project.Id,
                    projectStatusField!.Id,
                    option!.Id
                );

                if (isMoved == false)
                {
                    Console.WriteLine($"Issue could not be moved!");
                    Environment.Exit(1);
                }

                Console.WriteLine("Issue was moved successfully!");
                Environment.Exit(0);
            })
            .WithNotParsed(x => Environment.Exit(1));
}
