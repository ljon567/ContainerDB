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
        public async Task<List<string>> GetUsername()
        {
            var users = (from m in _context.UserItem
                              select m.username).Distinct();

            var returned = await users.ToListAsync();

            return returned;
        }

        //Upload user 
        [HttpPost, Route("uploadUser")]
        public async Task<IActionResult> UploadUserItem([FromForm]UserUploadItem user)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }
            try
            {
                //Upload all variables
                UserItem userItem = new UserItem();
                userItem.username = user.username;
                userItem.password = user.password;

                _context.UserItem.Add(userItem);
                await _context.SaveChangesAsync();

                return Ok($"File: {user.username} has successfully uploaded");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error has occured. Details: {ex.Message}");
            }
        }
    }
}