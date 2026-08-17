using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/types-chambres")]
public sealed class TypeChambresController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TypeChambresController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TypeChambreResponse>>> GetAll()
    {
        var entities = await _unitOfWork.TypesChambres.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TypeChambreResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.TypesChambres.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<TypeChambreResponse>> Create(
        TypeChambreRequest request)
    {
        var entity = new TypeChambre
        {
            Libelle = request.Libelle.Trim(),
            Description = request.Description,
            Capacite = request.Capacite,
            PrixParNuit = request.PrixParNuit
        };

        await _unitOfWork.TypesChambres.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = entity.Id },
            ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        TypeChambreRequest request)
    {
        var entity = await _unitOfWork.TypesChambres.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Libelle = request.Libelle.Trim();
        entity.Description = request.Description;
        entity.Capacite = request.Capacite;
        entity.PrixParNuit = request.PrixParNuit;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TypesChambres.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.TypesChambres.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.TypesChambres.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static TypeChambreResponse ToResponse(TypeChambre entity) =>
        new(
            entity.Id,
            entity.Libelle,
            entity.Description,
            entity.Capacite,
            entity.PrixParNuit);
}


