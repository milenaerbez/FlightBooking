using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FlightBooking.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {

            if (User.IsInRole("Agent"))
            {
                var reservations = await _context.Reservation
            .Include(r => r.Customer)
            .Include(r => r.Flight)
            .ToListAsync();

                return View(reservations);
            }
            else
            {

                string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var applicationDbContext = _context.Reservation
         .Where(r => r.CustomerId.Equals(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value))
         .Include(r => r.Customer)
         .Include(r => r.Flight);
                return View(await applicationDbContext.ToListAsync());
            }
          

        }



        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }
            //var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservation = await _context.Reservation
                .Include(r => r.Customer)
                .Include(r => r.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.AppUser, "Id", "Id");
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,FlightId,Status,Seats")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.AppUser, "Id", "Id", reservation.CustomerId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", reservation.FlightId);
            return View(reservation);
        }
        [Authorize(Roles ="Agent")]
        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.AppUser, "Id", "Id", reservation.CustomerId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", reservation.FlightId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,FlightId,Status,Seats")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.AppUser, "Id", "Id", reservation.CustomerId);
            ViewData["FlightId"] = new SelectList(_context.Flight, "Id", "Id", reservation.FlightId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Customer)
                .Include(r => r.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Agent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Reservation?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
