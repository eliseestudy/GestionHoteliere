using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Users.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(UserRequest request)
    {
        var entity = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email,
            PasswordHash = request.PasswordHash,
            Role = request.Role,
            Prenom = request.Prenom,
            Nom = request.Nom,
            IsActive = request.IsActive
        };

        await _unitOfWork.Users.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UserRequest request)
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Username = request.Username.Trim();
        entity.Email = request.Email;
        entity.PasswordHash = request.PasswordHash;
        entity.Role = request.Role;
        entity.Prenom = request.Prenom;
        entity.Nom = request.Nom;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Users.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Users.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Users.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static UserResponse ToResponse(User entity) =>
        new(
            entity.Id,
            entity.Username,
            entity.Email,
            entity.Role,
            entity.Prenom,
            entity.Nom,
            entity.IsActive,
            entity.DerniereConnexion);
}
