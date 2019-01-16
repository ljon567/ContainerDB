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
    [Route("api/Container")]
    public class ContainerController : Controller
    {
        private readonly ContainerDBContext _context;

        private IConfiguration _configuration;

        public ContainerController(ContainerDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Container
        [HttpGet]
        public IEnumerable<ContainerItem> GetAllContainerItems()
        {
            return _context.ContainerItem;
        }

        // GET: api/Container/ContainerID
        [HttpGet("{ContainerID}")]
        public IEnumerable<ContainerItem> GetContainerItemWithContainerID([FromRoute] string ContainerID)
        {
            return _context.ContainerItem.Where(m => m.ContainerID == ContainerID);
        }

        // GET: api/Container/class
        /*[HttpGet("{Class}")]
        public IEnumerable<ContainerItem> GetContainerItemWithClass([FromRoute] string Class)
        {
            return _context.ContainerItem.Where(m => m.Class == Class);
        }*/

        // PUT: api/Container/id (Update container #id)
        [HttpPut("{ContainerID}")]
        public async Task<IActionResult> PutContainerItem([FromRoute] int ContainerID, [FromBody] ContainerItem containerItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ContainerID != containerItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(containerItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContainerItemExists(ContainerID))
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

        // POST: api/Container
        [HttpPost]
        public async Task<IActionResult> PostContainerItem([FromBody] ContainerItem containerItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContainerItem.Add(containerItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContainerItem", new { id = containerItem.Id }, containerItem);
        }

        // DELETE: api/Container/id (Delete container #id)
        [HttpDelete("{ContainerID}")]
        public async Task<IActionResult> DeleteContainerItem([FromRoute] int ContainerID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var containerItem = await _context.ContainerItem.SingleOrDefaultAsync(m => m.Id == ContainerID);
            if (containerItem == null)
            {
                return NotFound();
            }

            _context.ContainerItem.Remove(containerItem);
            await _context.SaveChangesAsync();

            return Ok(containerItem);
        }

        private bool ContainerItemExists(int id)
        {
            return _context.ContainerItem.Any(e => e.Id == id);
        }

        // GET: api/Container/ContainerID (Get all container IDs)
        [Route("ContainerID")]
        [HttpGet]
        public async Task<List<string>> GetClass()
        {
            var containers = (from m in _context.ContainerItem
                              select m.ContainerID).Distinct();

            var returned = await containers.ToListAsync();

            return returned;
        }

        //Upload container image to blob storage
        [HttpPost, Route("uploadContainer")]
        public async Task<IActionResult> UploadFile([FromForm]ContainerUploadItem container)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                /*using (var stream = container.Image.OpenReadStream())
                {
                    var cloudBlock = await UploadToBlob(container.Image.FileName, null, stream);

                    if (string.IsNullOrEmpty(cloudBlock.StorageUri.ToString()))
                    {
                        return BadRequest("An error has occured while uploading your file. Please try again.");
                    }*/

                //Upload all variables
                ContainerItem containerItem = new ContainerItem();
                containerItem.ContainerID = container.ContainerID;
                containerItem.shipco = container.shipco;
                containerItem.ISO = container.ISO;
                containerItem.grade = container.grade;
                containerItem.location = container.location;
                containerItem.status = container.status;
                containerItem.time = container.time;
                containerItem.booking = container.booking;
                containerItem.vessel = container.vessel;
                containerItem.loadPort = container.loadPort;
                containerItem.weight = container.weight;
                containerItem.category = container.category;
                containerItem.seal = container.seal;
                containerItem.commodity = container.commodity;

                //Upload url of image
                // System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                // containerItem.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                // containerItem.Uploaded = DateTime.Now.ToString();

                _context.ContainerItem.Add(containerItem);
                await _context.SaveChangesAsync();

                return Ok($"File: {container.ContainerID} has successfully uploaded");
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

            CloudBlobContainer imagesContainer = blobClient.GetContainerReference("containerimages");

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
                return "." + extentionList.Last();
            }
        }
    }
}