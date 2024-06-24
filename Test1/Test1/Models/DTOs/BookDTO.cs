namespace Test1.Models.DTOs;

public class BookDTO
{
    
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new List<string>();
}


