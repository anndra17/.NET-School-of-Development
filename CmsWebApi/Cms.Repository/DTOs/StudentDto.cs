using System.ComponentModel.DataAnnotations;

namespace Cms.Repository.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [MaxLength(50)]
    public string Address { get; set; }
}
