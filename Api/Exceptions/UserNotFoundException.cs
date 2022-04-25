namespace Api.Exceptions
{
    public class UserNotFoundException : Exception
    {
        override public string Message { get; } = "Couldn't find user, or user not specified.";
    }
}
