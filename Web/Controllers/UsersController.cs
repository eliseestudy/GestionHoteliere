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
    public class UsersController : Controller
    {
        private readonly GestionHoteliereDbContext _context;
        private readonly IBusinessRulesService _rules;

        public UsersController(GestionHoteliereDbContext context, IBusinessRulesService rules)
        {
            _context = context;
            _rules = rules;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View(new User());
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,PasswordHash,Role,Prenom,Nom,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTimeOffset.UtcNow;
                user.CreatedById = null;
                user.IsDeleted = false;
                user.UpdatedAt = null;
                user.UpdatedById = null;
                user.DeletedAt = null;
                user.DeletedById = null;
                user.DerniereConnexion = null;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Username,Email,PasswordHash,Role,Prenom,Nom,IsActive,Id")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    existingUser.PasswordHash = user.PasswordHash;
                    existingUser.Role = user.Role;
                    existingUser.Prenom = user.Prenom;
                    existingUser.Nom = user.Nom;
                    existingUser.IsActive = user.IsActive;
                    existingUser.UpdatedAt = DateTimeOffset.UtcNow;
                    existingUser.UpdatedById = null;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (!await _rules.CanDeleteUserAsync(id))
                {
                    user.IsActive = false;
                    user.UpdatedAt = DateTimeOffset.UtcNow;
                    TempData["Toast.Type"] = "warning";
                    TempData["Toast.Message"] = "Utilisateur lie a des paiements : il a ete desactive au lieu d'etre supprime.";
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                user.IsDeleted = true;
                user.DeletedAt = DateTimeOffset.UtcNow;
                user.DeletedById = null;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Toast.Type"] = "success";
            TempData["Toast.Message"] = user.IsActive ? "Utilisateur active." : "Utilisateur desactive.";
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
