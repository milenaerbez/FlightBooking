using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Data;
using FlightBooking.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlightBooking.Controllers
{
    
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        [Authorize(Roles ="Agent,Admin")]
        public async Task<IActionResult> Index()
        {
              return _context.Flight != null ? 
                          View(await _context.Flight.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Flight'  is null.");
        }

        // GET: Flights/Details/5
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Flight == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        [Authorize(Roles = "Agent")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Create([Bind("Id,FlightNumber,DepartureLocation,ArrivalLocation,DepartureTime,Seats,Transfer")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        // GET: Flights/Edit/5
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Flight == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FlightNumber,DepartureLocation,ArrivalLocation,DepartureTime,Seats,Transfer")] Flight flight)
        {
            if (id != flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Id))
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
            return View(flight);
        }

        // GET: Flights/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Flight == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Flight == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Flight'  is null.");
            }
            var flight = await _context.Flight.FindAsync(id);
            if (flight != null)
            {
                _context.Flight.Remove(flight);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
          return (_context.Flight?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Search(string departureLocation, string arrivalLocation, bool includeTransfers)
        {
            var flights = _context.Flight.AsQueryable();

            if (!string.IsNullOrEmpty(departureLocation))
            {
                flights = flights.Where(f => f.DepartureLocation.Contains(departureLocation));
            }

            if (!string.IsNullOrEmpty(arrivalLocation))
            {
                flights = flights.Where(f => f.ArrivalLocation.Contains(arrivalLocation));
            }
            if (!includeTransfers)
            {
                flights = flights.Where(f => f.Transfer == 0);
            }




            //if (hasTransfers)
            //{
            //    flights = flights.Where(f => f.Transfer > 0);
            //}
            //else
            //{
            //    flights = flights.Where(f => f.Transfer == 0);
            //}
            flights = flights.Where(f => f.Seats > 0);

            var searchResults = await flights.ToListAsync();

            return View(searchResults);
        }

        [AllowAnonymous]
        public IActionResult CreateReservation(int flightId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var flight = _context.Flight.Find(flightId);

                if (flight == null)
                {
                    return NotFound();
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    var currentDate = DateTime.Now;
                    var departDate = flight.DepartureTime;

                    var timeDif = departDate - currentDate;
                    if (timeDif.Days < 3)
                    {
                        return View("~/Views/Reservations/ErrorView.cshtml");
                    }

                    var reservationModel = new Reservation
                    {
                        FlightId = flightId,
                        CustomerId = userId,
                        Flight = flight


                    };

                    ViewBag.FlightId = flightId;
                    ViewBag.CustomerId = userId;



                    //Console.WriteLine("Flight Id:" + flightId);
                    return View("~/Views/Reservations/Create.cshtml", reservationModel);
                }
                else
                {

                    return View("ErrorView");
                }
            }
            else
            {

                return RedirectToAction("Login");
            }
        }
    }
}
