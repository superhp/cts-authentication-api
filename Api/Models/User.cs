namespace Api.Models
{
    public class User
    {
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public string SocialEmail { get; set; }
        public string CtsEmail { get; set; }
    }
}
