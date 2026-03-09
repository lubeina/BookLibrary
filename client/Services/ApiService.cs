using System.Net.Http.Json;
using BookClient.Models;

namespace BookClient.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    // Authors
    public async Task<List<Author>> GetAuthors()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Author>>("/api/authors") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<bool> CreateAuthor(Author author)
    {
        var response = await _http.PostAsJsonAsync("/api/authors", author);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var response = await _http.DeleteAsync($"/api/authors/{id}");
        return response.IsSuccessStatusCode;
    }

    // Books
    public async Task<List<Book>> GetBooks()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Book>>("/api/books") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<bool> CreateBook(Book book)
    {
        var response = await _http.PostAsJsonAsync("/api/books", book);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBook(int id)
    {
        var response = await _http.DeleteAsync($"/api/books/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAuthor(Author author)
    {
        var response = await _http.PutAsJsonAsync($"/api/authors/{author.Id}", author);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateBook(Book book)
    {
        var response = await _http.PutAsJsonAsync($"/api/books/{book.Id}", book);
        return response.IsSuccessStatusCode;
    }
}
