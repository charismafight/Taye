namespace Taye.Models
{
    public class User : BaseModel
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Signature { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public int NotifyCount { get; set; }
        public int UnreadCount { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}