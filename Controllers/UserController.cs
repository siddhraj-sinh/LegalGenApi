using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegalGenApi.Data;
using LegalGenApi.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LegalGenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LegalGenContext _context;

        public UserController(LegalGenContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
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

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
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

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {

                //get user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // User not found
                    return NotFound();
                }

                //generate reset token
                var token = GenerateToken();

                //store reset token to db
                SaveResetToken(user, token);

                await SendPasswordResetEmail(email, token);

                
                return Ok();
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate error response
                
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }

    
        private string GenerateToken()
        {
            Guid tokenGuid = Guid.NewGuid();
            string tokenString = tokenGuid.ToString();

            return tokenString;
        }
        private async Task SaveResetToken(User user, string token)
        {
            user.ResetToken = token;
           // user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1); // Set the token expiration time as needed

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        private async Task SendPasswordResetEmail(string email, string token)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("contact.developer.siddharaj@gmail.com", "cgvynezscrnqgpkm");

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("contact.developer.siddharaj@gmail.com");
                    message.To.Add(email);
                    message.Subject = "Forgot Password";
                    message.Body = $"http://localhost:4200/user/reset-password/{token}";
                    message.IsBodyHtml = true;

                    await smtpClient.SendMailAsync(message);
                }
            }
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordModel model)
        {
            try
            {

                // get the token and password
               

                // get the user based on the token
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token);

                if (user == null)
                {
                    // Token is invalid or expired
                    return BadRequest("Invalid or expired reset token.");
                }

                // Update the user's password
                user.Password = model.Password; // Hash the password using a suitable hashing algorithm
                user.ResetToken = null;
                

                // Save the changes to the database
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate error response
                return StatusCode(500, "An error occurred while updating the password.");
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] SignInModel model)
        {
            // Authenticate user credentials
            var authenticatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (authenticatedUser == null)
                return Unauthorized("Invalid email or password");

            // Generate an access token
            var accessToken = GenerateToken();
            authenticatedUser.AccessToken = accessToken;
            await _context.SaveChangesAsync();
            return Ok(new { AccessToken = accessToken });
        }

        [HttpPost("signout")]
        public async Task<IActionResult> Signout()
        {
            // Get the current user's token from the request headers
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Retrieve the user associated with the token from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccessToken == token);

            if (user == null)
            {
                // User not found or already logged out
                return NotFound();
            }

            // Clear the user's token
            user.AccessToken = null;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
