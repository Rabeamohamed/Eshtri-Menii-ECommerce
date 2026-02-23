

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

        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

    }
}
