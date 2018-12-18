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
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly ContainerDBContext _context;

        private IConfiguration _configuration;

        public UserController(ContainerDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/User
        [HttpGet]
        public IEnumerable<UserItem> GetAllUserItems()
        {
            return _context.UserItem;
        }

        // GET: api/Container/class
        /*[HttpGet("{Class}")]
        public IEnumerable<UserItem> GetUserItemWithClass([FromRoute] string Class)
        {
            return _context.UserItem.Where(m => m.Class == Class);
        }*/

        // PUT: api/User/id (Update user #id)
        [HttpPut("{UserID}")]
        public async Task<IActionResult> PutUserItem([FromRoute] int UserID, [FromBody] UserItem userItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserID != userItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(userItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserItemExists(UserID))
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

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> PostUserItem([FromBody] UserItem userItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.UserItem.Add(userItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserItem", new { id = userItem.Id }, userItem);
        }

        // DELETE: api/User/id (Delete user #id)
        [HttpDelete("{UserID}")]
        public async Task<IActionResult> DeleteUserItem([FromRoute] int UserID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userItem = await _context.UserItem.SingleOrDefaultAsync(m => m.Id == UserID);
            if (userItem == null)
            {
                return NotFound();
            }

            _context.UserItem.Remove(userItem);
            await _context.SaveChangesAsync();

            return Ok(userItem);
        }

        private bool UserItemExists(int id)
        {
            return _context.UserItem.Any(e => e.Id == id);
        }

        // GET: api/User/username (Get all usernames)
        [Route("username")]
        [HttpGet]
        public async Task<List<string>> GetClass()
        {
            var users = (from m in _context.UserItem
                              select m.username).Distinct();

            var returned = await users.ToListAsync();

            return returned;
        }

        //Upload user 
        [HttpPost, Route("uploadUser")]
        public async Task<IActionResult> UploadFile([FromForm]UserUploadItem user)
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
                UserItem userItem = new UserItem();
                userItem.username = user.username;
                userItem.password = user.password;

                //Upload url of image
                // System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                // userItem.Url = cloudBlock.SnapshotQualifiedUri.AbsoluteUri;
                // userItem.Uploaded = DateTime.Now.ToString();

                _context.UserItem.Add(userItem);
                await _context.SaveChangesAsync();

                return Ok($"File: {user.username} has successfully uploaded");
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