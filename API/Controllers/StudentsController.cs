using API.DTOs;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class StudentsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IJsonTranslationService _translator;

    public StudentsController(IUnitOfWork unitOfWork, IMapper mapper,
    UserManager<AppUser> userManager, IJsonTranslationService translator)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _translator = translator;
    }


    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StudentDto>>> GetStudents([FromQuery] StudentSpecParams specParams)
    {
        var spec = new StudentSpecification(specParams);

        return await CreatePagedResult<StudentUser, StudentDto>(_unitOfWork.Repository<StudentUser>(),
            spec, specParams.PageIndex, specParams.PageSize, _mapper);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetStudentById(int id)
    {
        var spec = new StudentSpecification(id);

        var student = await _unitOfWork.Repository<StudentUser>().GetEntityWithSpec(spec);

        if (student is null) return NotFound();

        return Ok(_mapper.Map<StudentDto>(student));

        // add more features ...Later
    }

    [Authorize(Policy = "RequireStudentRole")]
    [HttpGet("my-profile")]
    public async Task<ActionResult<StudentDto>> GetMyProfile()
    {
        var spec = new StudentSpecification(User.GetUserId(), true);

        var student = await _unitOfWork.Repository<StudentUser>().GetEntityWithSpec(spec);

        if (student is null) return NotFound();

        return Ok(_mapper.Map<StudentDto>(student));
    }


    [Authorize(Policy = "RequireStudentRole")]
    [HttpGet("contacts")]
    public async Task<ActionResult> GetMyContacts()
    {
        var currentStudentId = User.GetUserId();

        var spec = new StudentSubjectSpecification(currentStudentId);

        var subjectsForCurrentStudent = await _unitOfWork.Repository<SubjectStudents>().ListAsync(spec);

        if (subjectsForCurrentStudent is null) return BadRequest("No subjects");

        var studentSubjects = subjectsForCurrentStudent.Select(x => x.SubjectId).ToArray();

        // Getting students with shared courses

        var studentsSpec = new StudentSubjectSpecification(studentSubjects);

        var sharedStudents = await _unitOfWork.Repository<SubjectStudents>().ListAsync(studentsSpec);

        if (sharedStudents is null) return BadRequest("No shared students");

        var contacts = sharedStudents
            .Select(x => x.Student)
            .Where(x => x.Id != currentStudentId)
            .DistinctBy(x => x.Id)
            .ToList();

        var unreadMessagesSpec = new MessageSpecification(currentStudentId, true);

        var unreadMessages = await _unitOfWork.Repository<Message>().ListAsync(unreadMessagesSpec);

        var unreadMessagesCount = unreadMessages
            .GroupBy(x => x.SenderId)
            .ToDictionary(x => x.Key, x => x.Count());

        var contactDtos = contacts.Select(x => new ContactDto 
        {
            ContactId = x.Id,
            PhotoUrl = x.AppUser.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            UserName = x.AppUser.UserName!,
            UnreadMessages = unreadMessagesCount.TryGetValue(x.Id, out int value) ? value : 0
        });

        return Ok(contactDtos);            
    }


    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent(CreateStudentDto createStudentDto)
    {
        if (await UserExists(createStudentDto.Email))
        {
            var message = await _translator.Translate("StudentsController", "CreateStudent", "EmailUsed");

            return BadRequest(message);
        }    

        var user = _mapper.Map<AppUser>(createStudentDto);

        user.UserName = createStudentDto.UserName.ToLower();

        user.CreatedAt = DateTime.Now;

        var result = await _userManager.CreateAsync(user, "Pa$$w0rd");

        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Student");

        if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

        var student = new StudentUser
        {
            Id = user.Id,
            AppUserId = user.Id
        };

        _unitOfWork.Repository<StudentUser>().Add(student);

        if (await _unitOfWork.Complete())
        {
            return CreatedAtAction("GetStudentById", new {id = user.Id}, _mapper.Map<StudentDto>(student));
        }

        return BadRequest("Failed creating student");
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut]
    public async Task<ActionResult> UpdateStudent(UpdateStudentDto updateStudentDto)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == updateStudentDto.UpdateStudentId);

        if (user is null) return NotFound();

        _mapper.Map(updateStudentDto, user);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("No new data");
    }


    [Authorize(Policy = "RequireStudentRole")]
    [HttpPut("my-profile")]
    public async Task<ActionResult> UpdateMyProfile(UpdateStudentDto updateStudentDto)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == updateStudentDto.UpdateStudentId);

        if (user is null) return NotFound();

        var currentUserId = User.GetUserId();

        if (user.Id != currentUserId) return Unauthorized("You are not allowed to do this");

        if (user.Email != updateStudentDto.Email) 
        {
            if (await UserExists(updateStudentDto.Email)) 
            {
                return BadRequest("This Email is used by another user");
            }
        }

        _mapper.Map(updateStudentDto, user);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("No new data");
    }


    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteStudent(int id)
    {
        var student = await _unitOfWork.Repository<StudentUser>().GetByIdAsync(id);

        if (student is null) return NotFound();

        _unitOfWork.Repository<StudentUser>().Remove(student);

        if (!await _unitOfWork.Complete()) return BadRequest("Failed delete this student");

        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == student.Id);

        if (user is null) return NotFound();

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }

    private async Task<bool> UserExists(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email);
    }
}
