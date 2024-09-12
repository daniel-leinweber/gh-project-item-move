namespace Extension.Exceptions;

public class ProjectFieldNotFoundException(string fieldName, string message)
    : Exception($"Could not find project field with name \"{fieldName}\"\n{message}") { }
