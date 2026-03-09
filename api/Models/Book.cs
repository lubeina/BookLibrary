using System.ComponentModel.DataAnnotations;

namespace BookApi.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Book title is required")]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    public int AuthorId { get; set; }
}
