using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SqliteWebApi.Models;

public class Apartment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public List<ApartmentPhoto> Photos { get; set; } = new();
}

public class ApartmentPhoto
{
    public int Id { get; set; }

    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string FilePath { get; set; } = string.Empty;

    public bool IsMainPhoto { get; set; }

    public int ApartmentId { get; set; }
    
    [JsonIgnore]
    public Apartment Apartment { get; set; } = null!;
}
