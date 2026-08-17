using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Web.Services;

namespace Web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;

        public ClientsController(GestionHoteliereDbContext context, IBusinessRulesService rules)
        {
            _context = context;
            _rules = rules;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View(new Client());
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Prenom,Nom,Email,Telephone,Adresse,DateNaissance,IdentifiantNational")] Client client)
        {
            if (ModelState.IsValid)
            {
                // Remplissage par défaut des champs de métadonnées
                client.CreatedAt = DateTimeOffset.UtcNow;
                client.CreatedById = null;
                client.IsDeleted = false;
                client.UpdatedAt = null;
                client.UpdatedById = null;
                client.DeletedAt = null;
                client.DeletedById = null;

                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Prenom,Nom,Email,Telephone,Adresse,DateNaissance,IdentifiantNational,Id")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingClient = await _context.Clients.FindAsync(id);
                    if (existingClient == null)
                    {
                        return NotFound();
                    }

                    existingClient.Prenom = client.Prenom;
                    existingClient.Nom = client.Nom;
                    existingClient.Email = client.Email;
                    existingClient.Telephone = client.Telephone;
                    existingClient.Adresse = client.Adresse;
                    existingClient.DateNaissance = client.DateNaissance;
                    existingClient.IdentifiantNational = client.IdentifiantNational;
                    existingClient.UpdatedAt = DateTimeOffset.UtcNow;
                    existingClient.UpdatedById = null;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                if (!await _rules.CanDeleteClientAsync(id))
                {
                    TempData["Toast.Type"] = "warning";
                    TempData["Toast.Message"] = "Ce client possede un historique et ne peut pas etre supprime.";
                    return RedirectToAction(nameof(Index));
                }

                client.IsDeleted = true;
                client.DeletedAt = DateTimeOffset.UtcNow;
                client.DeletedById = null;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
