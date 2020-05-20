using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var foundObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);
            if (foundObject == null)
            {
                return NotFound();
            }

            var satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == id).ToList();

            foundObject.Satellites = satellites;

            return Ok(foundObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var objects = _context.CelestialObjects.Where(co => co.Name == name).ToList();
            if (objects.Count == 0)
            {
                return NotFound();
            }

            foreach (var obj in objects)
            {
                var satellites = _context.CelestialObjects.Where(co => co.OrbitedObjectId == obj.Id)
                                         .ToList();

                obj.Satellites = satellites;
            }

            return Ok(objects);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var objects = _context.CelestialObjects;

            foreach (var obj in objects)
            {

                var satellites = objects.Where(co => co.OrbitedObjectId == obj.Id).ToList();
                obj.Satellites = satellites;
            }

            return Ok(objects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject celestialObject)
        {
            var foundObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);
            if (foundObject == null)
            {
                return NotFound();
            }

            foundObject.Name = celestialObject.Name;
            foundObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            foundObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(foundObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenamedObject(int id,string name)
        {
            var foundObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);
            if (foundObject == null)
            {
                return NotFound();
            }

            foundObject.Name = name;

            _context.CelestialObjects.Update(foundObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objects = _context.CelestialObjects.Where(co => co.Id == id || co.OrbitedObjectId == id)
                                  .ToList();
            if (objects.Count == 0)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(objects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
