namespace Test1.Models.DTOs;

public class NewBookDTO
{
    public string Title { get; set; } = string.Empty;

    public List<int> Genres { get; set; } = new List<int>();
}