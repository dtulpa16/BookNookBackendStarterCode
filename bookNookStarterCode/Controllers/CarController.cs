using bookNookStarterCode.Data;
using bookNookStarterCode.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bookNookStarterCode.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/car
        [HttpGet]
        public IActionResult GetAllCars()
        {
            try
            {
                var cars = _context.Cars.Include(u => u.Owner);
                return StatusCode(200, cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("my-cars"), Authorize]
        public IActionResult GetUsersCars()
        {
            try
            {
                string userId = User.FindFirstValue("id");
                var cars = _context.Cars.Include(u => u.Owner).Where(c => c.OwnerId.Equals(userId));
                return StatusCode(200, cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // GET api/car/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var car = _context.Cars.Include(c => c.Owner).FirstOrDefault(c => c.Id == id);
                if (car == null)
                {
                    return NotFound();
                }
                return StatusCode(200, car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/car
        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Car data)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                data.OwnerId = userId;
                data.Owner = _context.Users.Find(userId);
                _context.Cars.Add(data);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();
                return StatusCode(201, data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/car/5
        [HttpPut("{id}"), Authorize]
        public IActionResult Put(int id, [FromBody] Car data)
        {
            try
            {
                Car car = _context.Cars.Include(c => c.Owner).FirstOrDefault(c => c.Id == id);

                if (car == null)
                {
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the car
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || car.OwnerId != userId)
                {
                    return Unauthorized();
                }

                // Update the car properties
                car.OwnerId = userId;
                car.Owner = _context.Users.Find(userId);
                car.Make = data.Make;
                car.Model = data.Model;
                car.Year = data.Year;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();

                return StatusCode(201, car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/<CarController>/5
        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                Car car = _context.Cars.FirstOrDefault(c => c.Id == id);
                if (car == null)
                {
                    return NotFound();
                }
                // Check if the authenticated user is the owner of the car
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || car.OwnerId != userId)
                {
                    return Unauthorized();
                }

                if (car == null)
                {
                    return NotFound();
                }
                else
                {
                    _context.Cars.Remove(car);
                    _context.SaveChanges();
                    return StatusCode(204);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
