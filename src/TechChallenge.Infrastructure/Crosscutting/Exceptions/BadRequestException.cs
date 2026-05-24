namespace TechChallenge.Infrastructure.Crosscutting.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }
}