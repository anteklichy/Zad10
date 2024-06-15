using System.ComponentModel.DataAnnotations;

namespace Zad10.Dtos;

public class DoctorInDto
{
    [Required] public string? FirstName { get; set; }
    [Required] public string? LastName { get; set; }
    [Required] [EmailAddress] public string? Email { get; set; }
}