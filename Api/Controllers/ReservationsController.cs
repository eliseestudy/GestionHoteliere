using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reservations")]
public sealed class ReservationsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ReservationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Reservations.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Reservations.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create(ReservationRequest request)
    {
        var entity = new Reservation
        {
            NumeroReservation = request.NumeroReservation.Trim(),
            ClientId = request.ClientId,
            ChambreId = request.ChambreId,
            TypeChambreId = request.TypeChambreId,
            DateCreation = DateTimeOffset.UtcNow,
            DateArrivee = request.DateArrivee,
            DateDepart = request.DateDepart,
            Statut = request.Statut,
            NombrePersonnes = request.NombrePersonnes,
            MontantEstime = request.MontantEstime
        };

        await _unitOfWork.Reservations.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ReservationRequest request)
    {
        var entity = await _unitOfWork.Reservations.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.NumeroReservation = request.NumeroReservation.Trim();
        entity.ClientId = request.ClientId;
        entity.ChambreId = request.ChambreId;
        entity.TypeChambreId = request.TypeChambreId;
        entity.DateArrivee = request.DateArrivee;
        entity.DateDepart = request.DateDepart;
        entity.Statut = request.Statut;
        entity.NombrePersonnes = request.NombrePersonnes;
        entity.MontantEstime = request.MontantEstime;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Reservations.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Reservations.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Reservations.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static ReservationResponse ToResponse(Reservation entity) =>
        new(
            entity.Id,
            entity.NumeroReservation,
            entity.ClientId,
            entity.ChambreId,
            entity.TypeChambreId,
            entity.DateCreation,
            entity.DateArrivee,
            entity.DateDepart,
            entity.Statut,
            entity.NombrePersonnes,
            entity.MontantEstime);
}
