namespace Blog.DTOs
{
    public class PostDto
    {
        public int id { get; set; }
        public required string imageurl {  get; set; }
        public string? description { get; set; }
        public int userid { get; set; }
        public int likesCount { get; set; }
    }
}
