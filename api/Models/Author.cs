using System.ComponentModel.DataAnnotations;

namespace BookApi.Models;

public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Author name is required")]
    [StringLength(100)]
    public string Name { get; set; } = "";

    public List<Book> Books { get; set; } = new();
}
