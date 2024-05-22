namespace View.Exceptions;

/// <summary>
/// Class <see cref="ClientException"/> should be thrown when any invalid situation occured on the client.
/// </summary>
public abstract class ClientException : Exception
{
    protected ClientException(string? message) : base(message)
    {
    }
}


public class UnspecifiedException : ClientException
{
    public UnspecifiedException() : base("Unspecified exception.")
    {
    }
}

public class SpecifiedException : ClientException
{
    public SpecifiedException(string message) : base($"Specific exception: {message ??= ""}")
    {
    }
}