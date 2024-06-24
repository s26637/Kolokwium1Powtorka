using Test1.Models.DTOs;

namespace Test1.Repositories;

public interface IBooksRepository
{
    public Task<bool> DoesBookExist(int id);
    
    public Task<bool> DoesGenresExist(int id);

    public Task<BookDTO> GetBook(int id);

    Task<int> AddBook(NewBookDTO book);
    
    Task AddBookGenres(int bookId, int genreId);
    
    
}