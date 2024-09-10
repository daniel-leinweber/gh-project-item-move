namespace Extension.Exceptions;

public class UserNotFoundException(
    string message =
        "Could not determine current user, please make sure to authenticate with the gh CLI or provide the '--owner' option!"
) : Exception(message) { }