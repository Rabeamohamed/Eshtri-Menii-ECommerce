

namespace ECom.Core.DTO.Auth
{
    public class EmailDto
    {
        public EmailDto(string to, string from, string subject, string content)
        {
            To = to;
            From = from;
            Subject = subject;
            Content = content;
        }

        public string To { get; init; }
        public string From { get; init; }
        public string Subject { get; init; }
        public string Content { get; init; }

    }
}
