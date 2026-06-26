namespace Utils.Models
{
    public class EmailMessageDo
    {
        public string Subject { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Body { get; set; }
        public List<EmailAttachment> Attachments { get; set; }

        public class EmailAttachment
        {
            public string Name { get; set; } = string.Empty;
            public byte[] FileBytes { get; set; } = Array.Empty<byte>();
            public string ContentType { get; set; } = "application/octet-stream";
        }
    }




}
