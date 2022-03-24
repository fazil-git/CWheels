using AuthenticationPlugin;
using ImageUploader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private CWheelsDbContext _cWheelsDbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public AccountsController(CWheelsDbContext cWheelsDbContext, IConfiguration configuration)
        {
            _cWheelsDbContext = cWheelsDbContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail = _cWheelsDbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
            if (userWithSameEmail != null)
            {
                return BadRequest("User with same email is already present");
            }
            else
            {
                string hashedPassword = SecurePasswordHasherHelper.Hash(user.Password);
                User userObj = new User()
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = hashedPassword
                };
                _cWheelsDbContext.Users.Add(userObj);
                _cWheelsDbContext.SaveChanges();
                return Created("User has been registered successfully", userObj);
            }
        }

        [HttpPost]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IActionResult Login([FromBody] User user)
        {
            var userEmail = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (userEmail == null)
            {
                return NotFound("Username not found!");
            }
            if (SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
            {
                var claims = new[]
                {
                   //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                   new Claim(ClaimTypes.Email, user.Email),
                };
                var token = _auth.GenerateAccessToken(claims);
                return new ObjectResult(new
                {
                    access_token = token.AccessToken,
                    expires_in = token.ExpiresIn,
                    token_type = token.TokenType,
                    creation_Time = token.ValidFrom,
                    expiration_Time = token.ValidTo,
                    user_id = userEmail.Id,
                    token_tostring = token.ToString()
                });
            }
            else
            {
                return Unauthorized("Entered password is incorrect!");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult changePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            //This 'User' class is not from Models
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);

            bool isOldPasswordMatches = SecurePasswordHasherHelper.Verify(changePasswordModel.OldPassword, user.Password);
            if (isOldPasswordMatches)
            {
                if (changePasswordModel.ConfirmPasswordSuccess)
                {
                    var newPassword = SecurePasswordHasherHelper.Hash(changePasswordModel.NewPassword);
                    user.Password = newPassword;
                    _cWheelsDbContext.SaveChanges();
                    return Ok("Your password has been successfully updated");
                }
                else
                {
                    return BadRequest("New password and confirm password doesn't match");
                }
            }
            else
            {
                return Unauthorized("Please enter the correct old password");
            }
        }

        //Please revise file handling which is important - checkout asp.net udemy lecture 52, 53
        [HttpPost]
        [Authorize]
        public IActionResult EditUserProfile([FromBody] byte[] ImageArray)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var user = _cWheelsDbContext.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var stream = new MemoryStream(ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "Images";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest();
            }
            else
            {
                user.ImageUrl = file;
                _cWheelsDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }
    }
}
