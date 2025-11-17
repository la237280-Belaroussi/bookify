using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookify.Models;
using Microsoft.AspNetCore.Authentication;
using Bookify.Services;
using BCrypt.Net;
using System.Runtime.CompilerServices;
using Bookify.Data;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly AuthorizationService _authService;
        public UsersController(AppDBContext context, AuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }

        //GET : api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        //GET: api/Users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        //PUT: api/Users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        //POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            // Check if the user already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            // Hash password of the user
            if (user.PasswordHash != null)
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash, salt);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUserById", new { id = user.Id }, user);
        }

        //DELETE: api/Users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("/login")]
        public async Task<ActionResult<User>> Login([FromForm] string username, [FromForm] string password)
        {
            var userExists = UserExists(username, password);
            if (userExists == null)
            {
                return Unauthorized("Identifiants invalides");
            }
            else
            {
                var token = _authService.CreateToken(userExists);
                return Ok(token);
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private User UserExists(string username, string password)
        {
            var user = _context.Users.First(u => u.Username == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
    }
}
