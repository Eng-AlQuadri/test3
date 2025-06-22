using API.DTOs;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class SubjectsController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public SubjectsController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }


    [Authorize(Policy = "RequireStudentRole")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<string>>> GetSubjects()
    {
        var spec = new SubjectSpecification();

        return Ok(await _unitOfWork.Repository<Subject>().ListAsync(spec));
    }


    [Authorize(Policy = "RequireStudentRole")]
    [HttpGet("student-subjects")]
    public async Task<ActionResult<IReadOnlyList<StudentSubjectDto>>> GetSubjectsForStudent()
    {
        var student = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == User.GetUserId());

        if (student is null) return NotFound("Student not found");

        var subjectSpec = new StudentSubjectSpecification(student.Id);

        var subjects = await _unitOfWork.Repository<SubjectStudents>().ListAsync(subjectSpec);

        if (subjects is null) return BadRequest("No subjects");

        var markSpec = new MarkSpecification(student.Id);

        var marks = await _unitOfWork.Repository<Mark>().ListAsync(markSpec);

        if (marks is null) return BadRequest("No marks");

        var query = from subject in subjects
            where subject.StudentId == student.Id
            join mark in marks on
            subject.SubjectId equals mark.SubjectId into subjectMarks
            from mark in subjectMarks.DefaultIfEmpty() // Left Join
            select new StudentSubjectDto {
                SubjectId = subject.Subject.Id,
                SubjectName = subject.Subject.Name,
                MinMark = subject.Subject.MinMark,
                GainedMark = mark?.GainedMark
            };
        
        return Ok(query);
    }


    [Authorize(Policy = "RequireStudentRole")]
    [HttpPost]
    public async Task<ActionResult<SubjectDto>> CreateSubject(CreateSubjectDto createSubjectDto)
    {
        var spec = new SubjectSpecification(createSubjectDto.Name);

        var subject = await _unitOfWork.Repository<Subject>().GetEntityWithSpec(spec);

        if (subject is not null) return BadRequest("This subject exists!");

        var newSubject = _mapper.Map<Subject>(createSubjectDto);

        _unitOfWork.Repository<Subject>().Add(newSubject);

        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<SubjectDto>(newSubject));

        return BadRequest("Failed create this subject");
    }


    [Authorize(Policy = "RequireStudentRole")]
    [HttpPost("assign/{subjectName}/student/{studentId:int}")]
    public async Task<ActionResult> AssignSubjectToStudent(string subjectName, int studentId)
    {
        var student = await _unitOfWork.Repository<StudentUser>().GetByIdAsync(studentId);

        if (student is null) return NotFound("Student not found");

        var spec = new SubjectSpecification2(subjectName);

        var subject = await _unitOfWork.Repository<Subject>().GetEntityWithSpec(spec);

        if (subject is null) return NotFound("Subject not found");

        var spec2 = new SubjectStudentSpecification(studentId, subject.Id);

        var subjectStudent2 = await _unitOfWork.Repository<SubjectStudents>().GetEntityWithSpec(spec2);

        if (subjectStudent2 is not null) return BadRequest("This student is assigned to this subject");

        var subjectStudent = new SubjectStudents
        {
            StudentId = studentId,
            SubjectId = subject.Id,
            Student = student,
            Subject = subject
        };

        _unitOfWork.Repository<SubjectStudents>().Add(subjectStudent);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed assign this subject");
    }
}
