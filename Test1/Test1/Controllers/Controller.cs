
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Test1.Models.DTOs;
using Test1.Repositories;

namespace Test1.Controllers;

[Route("api/books")]
[ApiController]
public class Controller : ControllerBase
{
    
    private readonly IBooksRepository _booksRepository;
    
    public Controller(IBooksRepository booksRepository)
    {
        _booksRepository = booksRepository;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        if (!await _booksRepository.DoesBookExist(id))
        {
            return NotFound($"Book not found");
        }

        var book = await _booksRepository.GetBook(id);

        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(NewBookDTO newBook)
    {
        // if (!await _booksRepository.DoesGenresExist(newAnimalWithProcedures.OwnerId))
        //     return NotFound($"Owner with given ID - {newAnimalWithProcedures.OwnerId} doesnis't ext");
        var id = 0;
        foreach (var genre in newBook.Genres)
        {
            if (!await _booksRepository.DoesGenresExist(genre))
                return NotFound($"Genre with given ID - {genre} doesn't exist");
        }

        
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            id = await _booksRepository.AddBook(new NewBookDTO()
            {
                Title = newBook.Title,
            });

            foreach (var genre in newBook.Genres)
            {
                await _booksRepository.AddBookGenres(id, genre);
            }

            scope.Complete();
        }

        return Created(Request.Path.Value ?? "api/books", new
        {
            id = id,
            title = newBook.Title,
            genres = newBook.Genres
        });
    }



}