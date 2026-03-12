using api.DTOs.Subject;
using api.DTOs.Subjects;
using api.Models;

namespace api.Mappers;

public static class SubjectMappers
{
    public static SubjectDto ToSubjectDto(this Subject subjectModel)
    {
        return new SubjectDto
        {
            Id = subjectModel.Id,
            Name = subjectModel.Name
        };
    }

    public static Subject ToSubjectFromCreateDto(this CreateSubjectDto subjectDto)
    {
        return new Subject
        {
            Name = subjectDto.Name
        };
    }
}