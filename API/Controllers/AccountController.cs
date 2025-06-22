using API.DTOs;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(ITokenService tokenService, IMapper mapper, 
        IUnitOfWork unitOfWork, UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, IPhotoService photoService)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _photoService = photoService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Email)) return BadRequest("This email is used");

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.UserName.ToLower();

        user.CreatedAt = DateTime.UtcNow;

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Student");

        if (!roleResult.Succeeded) return BadRequest(result.Errors);

        var student = new StudentUser
        {
            Id = user.Id,
            AppUserId = user.Id
        };

        _unitOfWork.Repository<StudentUser>().Add(student);

        await _unitOfWork.Complete();

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user is null) return BadRequest("Invalid Email");

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return BadRequest("Invalid Password");

        return new UserDto
        {
            UserName = user.UserName!,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    [Authorize]
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userManager.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Id == User.GetUserId());

        if (user is null) return NotFound("User not found");

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error is not null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0)
            photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<PhotoDto>(photo));

        return BadRequest("Failed uploading image");
    }

    [Authorize]
    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _userManager.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Id == User.GetUserId());

        if (user is null) return NotFound("User not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo is null) return NotFound("Photo not found");

        if (photo.IsMain) return BadRequest("This is already your main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        if (currentMain is null) return NotFound("You don't have a main photo");

        currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to set main photo");
    }

    [Authorize]
    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _userManager.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.Id == User.GetUserId());

        if (user is null) return NotFound("User not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo is null) return NotFound("Photo not found");

        if (photo.IsMain) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId is not null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error is not null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed delete this photo");
    }

    private async Task<bool> UserExists(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.Email == email);
    }
}
