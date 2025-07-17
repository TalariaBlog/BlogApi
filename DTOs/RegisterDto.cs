namespace Blog.DTOs
{
    public class RegisterDto
    {
        public required string name { get; set; }
        public required string email { get; set; }
        public required string passwordhash { get; set; }
    }
}
