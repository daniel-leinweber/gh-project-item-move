using Extension.Models;

namespace Extension.Interfaces;

public interface IGitHubService
{
    string? CurrentUser { get; }

    IEnumerable<string> GetOwners();
    List<Project> GetProjects(string owner);
    ProjectField? GetProjectField(int projectNumber, string fieldName, string owner);
    bool MoveIssue(string issueId, string projectId, string fieldId, string optionId);
    List<ProjectItem> GetIssues(Project project, string owner);
}
