using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/paiements")]
public sealed class PaiementsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PaiementsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaiementResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Paiements.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaiementResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Paiements.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<PaiementResponse>> Create(PaiementRequest request)
    {
        var entity = new Paiement
        {
            FactureId = request.FactureId,
            DatePaiement = request.DatePaiement,
            Montant = request.Montant,
            Mode = request.Mode,
            Statut = request.Statut,
            TransactionReference = request.TransactionReference,
            UserId = request.UserId
        };

        await _unitOfWork.Paiements.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PaiementRequest request)
    {
        var entity = await _unitOfWork.Paiements.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.FactureId = request.FactureId;
        entity.DatePaiement = request.DatePaiement;
        entity.Montant = request.Montant;
        entity.Mode = request.Mode;
        entity.Statut = request.Statut;
        entity.TransactionReference = request.TransactionReference;
        entity.UserId = request.UserId;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Paiements.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Paiements.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Paiements.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static PaiementResponse ToResponse(Paiement entity) =>
        new(
            entity.Id,
            entity.FactureId,
            entity.DatePaiement,
            entity.Montant,
            entity.Mode,
            entity.Statut,
            entity.TransactionReference,
            entity.UserId);
}
