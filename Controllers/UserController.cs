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

        [HttpGet("user/by-token")]
        public async Task<IActionResult> GetUserByToken()
        {
            try
            {
                // Get the token from the request headers
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Retrieve the user associated with the token from the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.AccessToken == token);

                if (user == null)
                {
                    // User not found or token is invalid
                    return NotFound("User not found");
                }

                // Return the user details
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate error response
                return StatusCode(500, "An error occurred while retrieving the user.");
            }
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
        private string GenerateResetPasswordEmailBody(string token)
        {
            return $@"<!doctype html>
        <html lang=""en-US"">
        <head>
            <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"" />
            <title>Reset Password Email Template</title>
            <meta name=""description"" content=""Reset Password Email Template."">
            <style type=""text/css"">
                a:hover {{text-decoration: underline !important;}}
            </style>
        </head>
        <body marginheight=""0"" topmargin=""0"" marginwidth=""0"" style=""margin: 0px; background-color: #f2f3f8;"" leftmargin=""0"">
            <!--100% body table-->
            <table cellspacing=""0"" border=""0"" cellpadding=""0"" width=""100%"" bgcolor=""#f2f3f8""
                style=""@import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); font-family: 'Open Sans', sans-serif;"">
                <tr>
                    <td>
                        <table style=""background-color: #f2f3f8; max-width:670px;  margin:0 auto;"" width=""100%"" border=""0""
                            align=""center"" cellpadding=""0"" cellspacing=""0"">
                            <tr>
                                <td style=""height:80px;"">&nbsp;</td>
                            </tr>
                            <tr>
                                <td style=""text-align:center;"">
                                  <h1>LegalGen.Ai</h1>
                                </td>
                            </tr>
                            <tr>
                                <td style=""height:20px;"">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <table width=""95%"" border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0""
                                        style=""max-width:670px;background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);"">
                                        <tr>
                                            <td style=""height:40px;"">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style=""padding:0 35px;"">
                                                <h1 style=""color:#1e1e2d; font-weight:500; margin:0;font-size:32px;font-family:'Rubik',sans-serif;"">You have
                                                    requested to reset your password</h1>
                                                <span
                                                    style=""display:inline-block; vertical-align:middle; margin:29px 0 26px; border-bottom:1px solid #cecece; width:100px;""></span>
                                                <p style=""color:#455056; font-size:15px;line-height:24px; margin:0;"">
                                                    We cannot simply send you your old password. A unique link to reset your
                                                    password has been generated for you. To reset your password, click the
                                                    following link and follow the instructions.
                                                </p>
                                                <a href=""http://localhost:4200/user/reset-password/{token}""
                                                    style=""background:#20e277;text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff;text-transform:uppercase; font-size:14px;padding:10px 24px;display:inline-block;border-radius:50px;"">Reset
                                                    Password</a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style=""height:40px;"">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                            <tr>
                                <td style=""height:20px;"">&nbsp;</td>
                            </tr>
                            <tr>
                                <td style=""text-align:center;"">
                                    <p style=""font-size:14px; color:rgba(69, 80, 86, 0.7411764705882353); line-height:18px; margin:0 0 0;"">&copy; <strong>www.LegalGenAi.com</strong></p>
                                </td>
                            </tr>
                            <tr>
                                <td style=""height:80px;"">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <!--/100% body table-->
        </body>
        </html>";
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
                    message.Body = GenerateResetPasswordEmailBody(token);
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

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordModel model)
        {
            try
            {

                // get the token and password


                // get the user based on the token
                var user = await _context.Users.FirstOrDefaultAsync(u => u.AccessToken == model.Token && u.Password == model.CurrentPassword);

                if (user == null)
                {
                    // Token is invalid or expired
                    return BadRequest("Invalid or expired reset token.");
                }

                // Update the user's password
                user.Password = model.NewPassword; // Hash the password using a suitable hashing algorithm
               


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
