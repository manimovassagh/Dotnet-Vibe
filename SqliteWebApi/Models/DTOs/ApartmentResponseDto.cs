namespace SqliteWebApi.Models.DTOs;

public class ApartmentResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<ApartmentPhotoDto> Photos { get; set; } = new();
}

public class ApartmentPhotoDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public bool IsMainPhoto { get; set; }
}
