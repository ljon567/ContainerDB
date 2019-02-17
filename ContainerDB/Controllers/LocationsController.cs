using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContainerDB.Models;
using ContainerDB.Helpers;
using Microsoft.Extensions.Configuration;

namespace ContainerDB.Controllers
{
    [Produces("application/json")]
    [Route("api/Locations")]
    public class LocationsController : Controller
    {
        private readonly ContainerDBContext _context;

        private IConfiguration _configuration;

        public LocationsController(ContainerDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Locations
        [HttpGet]
        public IEnumerable<LocationsItem> GetAllLocationsItems()
        {
            return _context.LocationsItem;
        }

        // GET: api/Locations/ContainerID
        [HttpGet("{ContainerID}")]
        public IEnumerable<LocationsItem> GetLocationsItemWithContainerID([FromRoute] string ContainerID)
        {
            return _context.LocationsItem.Where(m => m.ContainerID == ContainerID);
        }

        // PUT: api/Locations/containerID (Update locations at container #id)
        [HttpPut("{ContainerID}")]
        public async Task<IActionResult> PutLocationsItem([FromRoute] string ContainerID, [FromBody] LocationsItem locationsItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ContainerID != locationsItem.ContainerID)
            {
                return BadRequest();
            }

            _context.Entry(locationsItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Locations
        [HttpPost]
        public async Task<IActionResult> PostLocationsItem([FromBody] LocationsItem locationsItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LocationsItem.Add(locationsItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocationsItem", new { id = locationsItem.Id }, locationsItem);
        }

        // DELETE: api/Locations/id (Delete locations #id)
        [HttpDelete("{LocationsID}")]
        public async Task<IActionResult> DeleteLocationsItem([FromRoute] int LocationsID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationsItem = await _context.LocationsItem.SingleOrDefaultAsync(m => m.Id == LocationsID);
            if (locationsItem == null)
            {
                return NotFound();
            }

            _context.LocationsItem.Remove(locationsItem);
            await _context.SaveChangesAsync();

            return Ok(locationsItem);
        }

        private bool LocationsItemExists(int id)
        {
            return _context.LocationsItem.Any(e => e.Id == id);
        }

        // GET: api/Locations/ContainerID (Get all container IDs)
        [Route("ContainerID")]
        [HttpGet]
        public async Task<List<string>> GetContainerID()
        {
            var locationss = (from m in _context.LocationsItem
                         select m.ContainerID).Distinct();

            var returned = await locationss.ToListAsync();

            return returned;
        }

        //Upload container locations
        [HttpPost, Route("uploadLocations")]
        public async Task<IActionResult> UploadLocationsItem([FromForm]LocationsUploadItem locations)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                //Upload all variables
                LocationsItem locationsItem = new LocationsItem();
                locationsItem.ContainerID = locations.ContainerID;
                locationsItem.Tauranga = locations.Tauranga;
                locationsItem.Lyttleton = locations.Lyttleton;
                locationsItem.Timaru = locations.Timaru;
                locationsItem.Otago = locations.Otago;
                locationsItem.Kiwirail = locations.Kiwirail;
                locationsItem.Auckland = locations.Auckland;

                _context.LocationsItem.Add(locationsItem);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {locations.ContainerID} has successfully uploaded");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }
        }
    }
}