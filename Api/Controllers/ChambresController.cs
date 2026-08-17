using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/chambres")]
public sealed class ChambresController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ChambresController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChambreResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Chambres.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChambreResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Chambres.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ChambreResponse>> Create(ChambreRequest request)
    {
        var entity = new Chambre
        {
            Numero = request.Numero.Trim(),
            Etage = request.Etage,
            TypeChambreId = request.TypeChambreId,
            Statut = request.Statut,
            NbLits = request.NbLits,
            Observations = request.Observations
        };

        await _unitOfWork.Chambres.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ChambreRequest request)
    {
        var entity = await _unitOfWork.Chambres.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.Numero = request.Numero.Trim();
        entity.Etage = request.Etage;
        entity.TypeChambreId = request.TypeChambreId;
        entity.Statut = request.Statut;
        entity.NbLits = request.NbLits;
        entity.Observations = request.Observations;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Chambres.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Chambres.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Chambres.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static ChambreResponse ToResponse(Chambre entity) =>
        new(
            entity.Id,
            entity.Numero,
            entity.Etage,
            entity.TypeChambreId,
            entity.Statut,
            entity.NbLits,
            entity.Observations);
}
