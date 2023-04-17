using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("linkpreviews")]
[Index(nameof(Url), IsUnique = true)]
public class LinkPreview: BaseEntity
{
    [Required]
    [Column(TypeName = "longtext")]
    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}
