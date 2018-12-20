using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContainerDB.Models;
using ContainerDB.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
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

        // GET: api/Locations/class
        /*[HttpGet("{Class}")]
        public IEnumerable<LocationsItem> GetLocationsItemWithClass([FromRoute] string Class)
        {
            return _context.LocationsItem.Where(m => m.Class == Class);
        }*/

        // PUT: api/Locations/id (Update locations #id)
        [HttpPut("{LocationsID}")]
        public async Task<IActionResult> PutLocationsItem([FromRoute] int LocationsID, [FromBody] LocationsItem locationsItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (LocationsID != locationsItem.Id)
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
                if (!LocationsItemExists(LocationsID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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
        public async Task<List<string>> GetClass()
        {
            var locationss = (from m in _context.LocationsItem
                         select m.ContainerID).Distinct();

            var returned = await locationss.ToListAsync();

            return returned;
        }

        //Upload locations image to blob storage
        [HttpPost, Route("uploadLocations")]
        public async Task<IActionResult> UploadFile([FromForm]LocationsUploadItem locations)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                /*using (var stream = locations.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(locations.Image.FileName, null, stream);

                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }*/

                //Upload all variables
                LocationsItem locationsItem = new LocationsItem();
                locationsItem.ContainerID = locations.ContainerID;
                locationsItem.Tauranga = locations.Tauranga;
                locationsItem.Lyttleton = locations.Lyttleton;
                locationsItem.Timaru = locations.Timaru;
                locationsItem.Otago = locations.Otago;
                locationsItem.Kiwirail = locations.Kiwirail;
                locationsItem.PortConnect = locations.PortConnect;

                //Upload url of image
                // System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                // locationsItem.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                // locationsItem.Uploaded = DateTime.Now.ToString();

                _context.LocationsItem.Add(locationsItem);
                    await _context.SaveChangesAsync();

                    return Ok($"File: {locations.ContainerID} has successfully uploaded");
                //}
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }


        }

        //Send image file to blob storage on Azure
        /*private async Task<CloudBlockBlob> UploadToBlob(string filename, byte[] imageBuffer = null, System.IO.Stream stream = null)
        {

            var accountName = _configuration["AzureBlob:name"];
            var accountKey = _configuration["AzureBlob:key"]; ;
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("locationsimages");

            string storageConnectionString = _configuration["AzureBlob:connectionString"];

            //Check whether the connection string can be parsed
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    //Generate a new filename for every new blob
                    var fileName = Guid.NewGuid().ToString();
                    fileName += GetFileExtention(filename);

                    //Get a reference to the blob address, then upload the file to the blob
                    CloudBlockBlob cloudBlockBlob = imagesContainer.GetBlockBlobReference(fileName);

                    if (stream != null)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return new CloudBlockBlob(new Uri(""));
                    }

                    return cloudBlockBlob;
                }
                catch (StorageException ex)
                {
                    return new CloudBlockBlob(new Uri(""));
                }
            }
            else
            {
                return new CloudBlockBlob(new Uri(""));
            }

        }*/

        private string GetFileExtention(string fileName)
        {
            //No extension
            if (!fileName.Contains("."))
                return ""; 
            else
            {
                //Assumes last item is the extension 
                var extentionList = fileName.Split('.');
                return "." + extentionList.Last();             }
        }
    }
}