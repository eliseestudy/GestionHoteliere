using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Clients.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Clients.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create(ClientRequest request)
    {
        var entity = new Client
        {
            Prenom = request.Prenom.Trim(),
            Nom = request.Nom.Trim(),
            Email = request.Email,
            Telephone = request.Telephone,
            Adresse = request.Adresse,
            DateNaissance = request.DateNaissance,
            IdentifiantNational = request.IdentifiantNational
        };

        await _unitOfWork.Clients.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ClientRequest request)
    {
        var entity = await _unitOfWork.Clients.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Prenom = request.Prenom.Trim();
        entity.Nom = request.Nom.Trim();
        entity.Email = request.Email;
        entity.Telephone = request.Telephone;
        entity.Adresse = request.Adresse;
        entity.DateNaissance = request.DateNaissance;
        entity.IdentifiantNational = request.IdentifiantNational;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Clients.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Clients.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Clients.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static ClientResponse ToResponse(Client entity) =>
        new(
            entity.Id,
            entity.Prenom,
            entity.Nom,
            entity.Email,
            entity.Telephone,
            entity.Adresse,
            entity.DateNaissance,
            entity.IdentifiantNational);
}
