using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
