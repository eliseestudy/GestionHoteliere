using Api.Contracts;
using Application.UnitOfWork;
using GestionHoteliere.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/factures")]
public sealed class FacturesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public FacturesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FactureResponse>>> GetAll()
    {
        var entities = await _unitOfWork.Factures.GetAllAsync();
        return Ok(entities.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FactureResponse>> GetById(int id)
    {
        var entity = await _unitOfWork.Factures.GetByIdAsync(id);

        return entity is null
            ? NotFound()
            : Ok(ToResponse(entity));
    }

    [HttpPost]
    public async Task<ActionResult<FactureResponse>> Create(FactureRequest request)
    {
        var entity = new Facture
        {
            NumeroFacture = request.NumeroFacture.Trim(),
            SejourId = request.SejourId,
            ClientId = request.ClientId,
            DateEmission = request.DateEmission,
            DateEcheance = request.DateEcheance,
            MontantHT = request.MontantHT,
            Taxe = request.Taxe,
            MontantTTC = request.MontantTTC,
            MontantPaye = request.MontantPaye,
            Statut = request.Statut
        };

        await _unitOfWork.Factures.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToResponse(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FactureRequest request)
    {
        var entity = await _unitOfWork.Factures.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.NumeroFacture = request.NumeroFacture.Trim();
        entity.SejourId = request.SejourId;
        entity.ClientId = request.ClientId;
        entity.DateEmission = request.DateEmission;
        entity.DateEcheance = request.DateEcheance;
        entity.MontantHT = request.MontantHT;
        entity.Taxe = request.Taxe;
        entity.MontantTTC = request.MontantTTC;
        entity.MontantPaye = request.MontantPaye;
        entity.Statut = request.Statut;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Factures.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _unitOfWork.Factures.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTimeOffset.UtcNow;

        _unitOfWork.Factures.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private static FactureResponse ToResponse(Facture entity) =>
        new(
            entity.Id,
            entity.NumeroFacture,
            entity.SejourId,
            entity.ClientId,
            entity.DateEmission,
            entity.DateEcheance,
            entity.MontantHT,
            entity.Taxe,
            entity.MontantTTC,
            entity.MontantPaye,
            entity.Statut);
}
