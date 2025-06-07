using Microsoft.EntityFrameworkCore;
using SqliteWebApi.Data;
using SqliteWebApi.Models;
using SqliteWebApi.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace SqliteWebApi.Services;

public class ApartmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ApartmentService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<List<ApartmentResponseDto>> GetAllApartmentsAsync()
    {
        return await _context.Apartments
            .Include(a => a.Photos)
            .Select(a => new ApartmentResponseDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Address = a.Address,
                Price = a.Price,
                Photos = a.Photos.Select(p => new ApartmentPhotoDto
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    FilePath = $"/Images/{p.FileName}",
                    IsMainPhoto = p.IsMainPhoto
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<ApartmentResponseDto> CreateApartmentAsync(CreateApartmentDto dto, IFormFileCollection photos)
    {
        var apartment = new Apartment
        {
            Title = dto.Title,
            Description = dto.Description,
            Address = dto.Address,
            Price = dto.Price
        };

        _context.Apartments.Add(apartment);
        await _context.SaveChangesAsync();

        // Process and save photos
        var imagesPath = Path.Combine(_environment.ContentRootPath, "Images");
        Directory.CreateDirectory(imagesPath); // Ensure directory exists

        foreach (var photo in photos)
        {
            if (photo.Length > 0)
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine("Images", fileName);
                var fullPath = Path.Combine(_environment.ContentRootPath, filePath);

                // Save the file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Create photo record
                var apartmentPhoto = new ApartmentPhoto
                {
                    FileName = fileName,
                    FilePath = filePath,
                    IsMainPhoto = !apartment.Photos.Any(), // First photo is the main photo
                    ApartmentId = apartment.Id
                };

                apartment.Photos.Add(apartmentPhoto);
            }
        }

        await _context.SaveChangesAsync();
        
        // Map to response DTO
        return await _context.Apartments
            .Include(a => a.Photos)
            .Where(a => a.Id == apartment.Id)
            .Select(a => new ApartmentResponseDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Address = a.Address,
                Price = a.Price,
                Photos = a.Photos.Select(p => new ApartmentPhotoDto
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    FilePath = p.FilePath,
                    IsMainPhoto = p.IsMainPhoto
                }).ToList()
            })
            .FirstAsync();
    }

    public async Task<ApartmentResponseDto?> GetApartmentByIdAsync(int id)
    {
        return await _context.Apartments
            .Include(a => a.Photos)
            .Where(a => a.Id == id)
            .Select(a => new ApartmentResponseDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Address = a.Address,
                Price = a.Price,
                Photos = a.Photos.Select(p => new ApartmentPhotoDto
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    FilePath = $"/Images/{p.FileName}",
                    IsMainPhoto = p.IsMainPhoto
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ApartmentPhotoDto?> GetPhotoByIdAsync(int apartmentId, int photoId)
    {
        return await _context.ApartmentPhotos
            .Where(p => p.ApartmentId == apartmentId && p.Id == photoId)
            .Select(p => new ApartmentPhotoDto
            {
                Id = p.Id,
                FileName = p.FileName,
                FilePath = $"/Images/{p.FileName}",
                IsMainPhoto = p.IsMainPhoto
            })
            .FirstOrDefaultAsync();
    }
}
