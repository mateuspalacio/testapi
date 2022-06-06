namespace Api.Exceptions
{
    public class InvalidTokenException : Exception
    {
        override public string Message { get; } = "Token is invalid or doesn't exist.";
    }
}
