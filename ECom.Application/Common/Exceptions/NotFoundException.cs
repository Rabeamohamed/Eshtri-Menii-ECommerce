namespace ECom.Application.Common.Exceptions
{
    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message)
            : base(message, 404)
        {
        }
    }
}
