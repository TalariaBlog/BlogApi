namespace Blog.DTOs
{
    public class LoginDto
    {
        public required string email { get; set; }
        public required string passwordhash { get; set; }
    }
}
