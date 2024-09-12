namespace Extension.Exceptions;

public class ProjectsNotFoundException(string owner, string message)
    : Exception($"Could not find projects for owner \"{owner}\".\n{message}") { }
