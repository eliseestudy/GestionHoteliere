using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/sejours")]
public sealed class SejoursController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SejoursController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SejourResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Sejours.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SejourResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Sejours.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<SejourResponse>> Create(SejourRequest request)
    {
        var entity = new Sejour
        {
            ReservationId = request.ReservationId,
            ClientId = request.ClientId,
            ChambreId = request.ChambreId,
            DateEntree = request.DateEntree,
            DateSortie = request.DateSortie,
            NbNuits = request.NbNuits,
            TarifApplique = request.TarifApplique,
            Remise = request.Remise,
            Statut = request.Statut
        };

        await _unitOfWork.Sejours.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SejourRequest request)
    {
        var entity = await _unitOfWork.Sejours.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.ReservationId = request.ReservationId;
        entity.ClientId = request.ClientId;
        entity.ChambreId = request.ChambreId;
        entity.DateEntree = request.DateEntree;
        entity.DateSortie = request.DateSortie;
        entity.NbNuits = request.NbNuits;
        entity.TarifApplique = request.TarifApplique;
        entity.Remise = request.Remise;
        entity.Statut = request.Statut;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Sejours.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Sejours.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Sejours.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static SejourResponse ToResponse(Sejour entity) =>
        new(
            entity.Id,
            entity.ReservationId,
            entity.ClientId,
            entity.ChambreId,
            entity.DateEntree,
            entity.DateSortie,
            entity.NbNuits,
            entity.TarifApplique,
            entity.Remise,
            entity.Statut);
}
