namespace Extension.Exceptions;

public class OwnersNotFoundException(
    string message =
        "Could not determine possible owners, please make sure to authenticate with the gh CLI or provide the '--owner' option!"
) : Exception(message) { }
