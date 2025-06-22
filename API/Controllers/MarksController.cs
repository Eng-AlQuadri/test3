using API.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MarksController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    public MarksController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [Authorize(Policy = "RequireStudentRole")]
    [HttpPost]
    public async Task<ActionResult> SetMark(SetMarkDto setMarkDto)
    {
        var spec = new SubjectSpecification2(setMarkDto.SubjectName);

        var subject = await _unitOfWork.Repository<Subject>().GetEntityWithSpec(spec);

        if (subject is null) return NotFound("subject not found");

        var studentSpec = new StudentSpecification(setMarkDto.StudentId);

        var student = await _unitOfWork.Repository<StudentUser>().GetEntityWithSpec(studentSpec);

        if (student is null) return NotFound("Student not found");

        if (!student.SubjectStudents.Any(x => x.SubjectId == subject.Id)) 
            return BadRequest("Student doesn't register this subject");

        var markSpec = new MarkSpecification(subject.Id, student.Id);

        var mark2 = await _unitOfWork.Repository<Mark>().GetEntityWithSpec(markSpec);

        if (mark2 is not null) return BadRequest("Mark is recorded");

        var mark = new Mark
        {
            SubjectId = subject.Id,
            StudentId = student.Id,
            GainedMark = setMarkDto.GainedMark,
            Subject = subject,
            Student = student
        };

        _unitOfWork.Repository<Mark>().Add(mark);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to take this mark");
    }
}
