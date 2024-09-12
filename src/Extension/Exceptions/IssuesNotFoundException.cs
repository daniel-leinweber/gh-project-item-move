namespace Extension.Exceptions;

public class IssuesNotFoundException(string projectTitle, string owner, string message)
    : Exception(
        $"Could not find issues for project \"{projectTitle}\" of owner \"{owner}\"\n{message}"
    ) { }
