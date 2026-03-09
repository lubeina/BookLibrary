using BookApi.Data;
using BookApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:5002").AllowAnyMethod().AllowAnyHeader()
);

//authors endpoints
app.MapGet(
    "/api/authors",
    async (AppDbContext db) => await db.Authors.Include(author => author.Books).ToListAsync()
);

app.MapGet(
    "/api/authors/{id}",
    async (int id, AppDbContext db) =>
    {
        var author = await db
            .Authors.Include(author => author.Books)
            .FirstOrDefaultAsync(author => author.Id == id);
        return author is not null ? Results.Ok(author) : Results.NotFound();
    }
);

app.MapPost(
    "/api/authors",
    async (Author author, AppDbContext db) =>
    {
        if (string.IsNullOrWhiteSpace(author.Name))
            return Results.BadRequest("Author name is required.");

        db.Authors.Add(author);
        await db.SaveChangesAsync();
        return Results.Created($"/api/authors/{author.Id}", author);
    }
);

app.MapPut(
    "/api/authors/{id}",
    async (int id, Author input, AppDbContext db) =>
    {
        var author = await db.Authors.FindAsync(id);
        if (author is null)
            return Results.NotFound();

        if (string.IsNullOrWhiteSpace(input.Name))
            return Results.BadRequest("Author name is required.");

        author.Name = input.Name;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.MapDelete(
    "/api/authors/{id}",
    async (int id, AppDbContext db) =>
    {
        var author = await db.Authors.FindAsync(id);
        if (author is null)
            return Results.NotFound();

        db.Authors.Remove(author);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

//books endpoints
app.MapGet(
    "/api/books",
    async (AppDbContext db) =>
        await db
            .Books.Join(
                db.Authors,
                book => book.AuthorId,
                author => author.Id,
                (book, author) =>
                    new
                    {
                        book.Id,
                        book.Title,
                        book.AuthorId,
                        AuthorName = author.Name,
                    }
            )
            .ToListAsync()
);

app.MapGet(
    "/api/books/{id}",
    async (int id, AppDbContext db) =>
    {
        var book = await db
            .Books.Where(book => book.Id == id)
            .Join(
                db.Authors,
                book => book.AuthorId,
                author => author.Id,
                (book, author) =>
                    new
                    {
                        book.Id,
                        book.Title,
                        book.AuthorId,
                        AuthorName = author.Name,
                    }
            )
            .FirstOrDefaultAsync();
        return book is not null ? Results.Ok(book) : Results.NotFound();
    }
);

app.MapPost(
    "/api/books",
    async (Book book, AppDbContext db) =>
    {
        if (string.IsNullOrWhiteSpace(book.Title))
            return Results.BadRequest("Book title is required.");

        var authorExists = await db.Authors.AnyAsync(author => author.Id == book.AuthorId);
        if (!authorExists)
            return Results.BadRequest($"Author with Id {book.AuthorId} does not exist.");

        db.Books.Add(book);
        await db.SaveChangesAsync();
        return Results.Created($"/api/books/{book.Id}", book);
    }
);

app.MapPut(
    "/api/books/{id}",
    async (int id, Book input, AppDbContext db) =>
    {
        var book = await db.Books.FindAsync(id);
        if (book is null)
            return Results.NotFound();

        if (string.IsNullOrWhiteSpace(input.Title))
            return Results.BadRequest("Book title is required.");

        var authorExists = await db.Authors.AnyAsync(author => author.Id == input.AuthorId);
        if (!authorExists)
            return Results.BadRequest($"Author with Id {input.AuthorId} does not exist.");

        book.Title = input.Title;
        book.AuthorId = input.AuthorId;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.MapDelete(
    "/api/books/{id}",
    async (int id, AppDbContext db) =>
    {
        var book = await db.Books.FindAsync(id);
        if (book is null)
            return Results.NotFound();

        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.MapGet(
    "/api/health",
    async (AppDbContext db) =>
    {
        var canConnect = await db.Database.CanConnectAsync();
        return Results.Ok(new { status = canConnect ? "Connected!" : "Failed" });
    }
);

app.Run();
