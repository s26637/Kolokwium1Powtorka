using Microsoft.Data.SqlClient;
using Test1.Models.DTOs;

namespace Test1.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;
    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM [Books] WHERE [PK] = @ID";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<BookDTO> GetBook(int id)
    {
	    var query = @"SELECT 
							[Books].PK AS BookPK,
							[Books].Title AS BookTitle,
							[Genres].Name as GenresName
						FROM [Books]
						JOIN [Books_genres] ON [Books_genres].FK_book = [Books].PK
						JOIN [Genres] ON [Genres].PK = [Books_genres].FK_genre
						WHERE [Books].PK = @ID";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);

	    await connection.OpenAsync();

	    var reader = await command.ExecuteReaderAsync();

	    var bookId = reader.GetOrdinal("BookPK");
	    var bookTitle = reader.GetOrdinal("BookTitle");
	    var genresNam = reader.GetOrdinal("GenresName");

	    BookDTO bookDto = null;
	    while (await reader.ReadAsync())
	    {
		    if (bookDto is not null)
		    {
			    bookDto.Genres.Add(reader.GetString(genresNam));

		    }
		    else
		    {
			    bookDto = new BookDTO()
			    {
				    Id = reader.GetInt32(bookId),
				    Title = reader.GetString(bookTitle),
				    Genres = new List<string>()
				    {
					    new string(reader.GetString(genresNam))
				    }
			    };
		    }

		   
	    }
	    if (bookDto is null) throw new Exception();

	    return bookDto;
    }

    public async Task<int> AddBook(NewBookDTO book)
    {
	    var insert = @"INSERT INTO Books VALUES(@Title);
					   SELECT @@IDENTITY AS ID;";
	    
	        
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@Title", book.Title);
	    
	    await connection.OpenAsync();
	    
	    var id = await command.ExecuteScalarAsync();

	    if (id is null) throw new Exception();
	    
	    return Convert.ToInt32(id);

    }
    
    
    public async Task AddBookGenres(int animalId, int genreId)
    {
	    var query = $"INSERT INTO Books_genres VALUES(@BookPK, @GenrePK)";

	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@BookPK", animalId);
	    command.Parameters.AddWithValue("@GenrePK", genreId);
	    
	    await connection.OpenAsync();

	    await command.ExecuteNonQueryAsync();
    }
    
    
    
    public async Task<bool> DoesGenresExist(int id)
    {
	    var query = "SELECT 1 FROM [Genres] WHERE [PK] = @ID";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);
        
	    await connection.OpenAsync();

	    var res = await command.ExecuteScalarAsync();

	    return res is not null;
    }

    

    

}