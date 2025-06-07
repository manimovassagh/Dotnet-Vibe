using Microsoft.AspNetCore.Mvc;
using SqliteWebApi.Models.DTOs;
using SqliteWebApi.Services;

namespace SqliteWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApartmentsController : ControllerBase
{
    private readonly ApartmentService _apartmentService;

    public ApartmentsController(ApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ApartmentResponseDto>>> GetAllApartments()
    {
        try
        {
            var apartments = await _apartmentService.GetAllApartmentsAsync();
            return Ok(apartments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApartmentResponseDto>> CreateApartment([FromForm] CreateApartmentDto apartment, [FromForm] IFormFileCollection photos)
    {
        if (photos == null || !photos.Any())
        {
            return BadRequest("At least one photo is required");
        }

        try 
        {
            var createdApartment = await _apartmentService.CreateApartmentAsync(apartment, photos);
            return CreatedAtAction(nameof(CreateApartment), new { id = createdApartment.Id }, createdApartment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApartmentResponseDto>> GetApartment(int id)
    {
        try
        {
            var apartment = await _apartmentService.GetApartmentByIdAsync(id);
            if (apartment == null)
            {
                return NotFound($"Apartment with ID {id} not found");
            }
            return Ok(apartment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{apartmentId:int}/photos/{photoId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApartmentPhotoDto>> GetPhoto(int apartmentId, int photoId)
    {
        try
        {
            var photo = await _apartmentService.GetPhotoByIdAsync(apartmentId, photoId);
            if (photo == null)
            {
                return NotFound($"Photo with ID {photoId} not found for apartment {apartmentId}");
            }
            return Ok(photo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
