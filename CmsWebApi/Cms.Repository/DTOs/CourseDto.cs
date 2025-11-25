using Cms.Repository.Models;
using System.Text.Json.Serialization;

namespace Cms.Repository.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int CourseDuration { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public COURSE_TYPE CourseType { get; set; }
}
