using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BE._iservices;
using BE.Context;
using BE.InterfaceController;
using BE.Model.Dto;
using BE.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    public class UserController : BaseApiController, IUserController
    {
        private readonly DataContext _context;
        private readonly EmailController _email;
        private readonly IContentService _contentService;
        private readonly ITokenService _tokenService;

        public UserController(DataContext context, EmailController email, IContentService contentService, ITokenService tokenService)
        {
            _context = context;
            _email = email;
            _contentService = contentService;
            _tokenService = tokenService;
        }

        private async Task<bool> UserExists(string Username)
        {
            return await _context.User.AnyAsync(x => x.Username == Username.ToLower());
        }
        private async Task<bool> EmailExists(string Email)
        {
            return await _context.User.AnyAsync(x => x.Email == Email.ToLower());
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterInputDto input)
        {
            if (await UserExists(input.Username))
            {
                return BadRequest(new {message = "Username already in use!"});
            }
            if (await EmailExists(input.Email))
            {
                return BadRequest(new {message = "Email already in use!"});
            }
            var activeCode = new Random().Next(1000, 9999);
            var rs = await _email.SendEmail(new EmailModel()
            {
                To = input.Email,
                Subject = "Active your YuGhiOh TCG account",
                Body = "<h2>Dear" + input.Username + ", click the button to active your account!</h2><a href='http://localhost:5233/api/User/ActiveUser" + "/" + input.Username + "/" + activeCode + "'><button style='width: 200px; height: 40px; background-color: #7400cc; color: white; border-radius: 6px; border: none;'>Click me!!!</button></a>",
            });
            if ((int)rs.GetType().GetProperty("StatusCode").GetValue(rs, null) == 200)
            {
                var newUser = new User()
                {
                    Username = input.Username.ToLower(),
                    Password = input.Password,
                    Email = input.Email,
                    Money = 0,
                    Actived = false,
                    ActiveCode = activeCode,
                    AvatarUrl = "https://res.cloudinary.com/dslzbnfu8/image/upload/v1699185130/samples/DuRiu.png",
                };
                _context.User.Add(newUser);
                await _context.SaveChangesAsync();
            }
            return rs;
        }

        [HttpGet("ActiveUser/{username}/{activeCode}")]
        public async Task<ActionResult> ActiveUser(string username, int activeCode)
        {
            string message;
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null) 
            {
                message = "URL not found!";
                return Content(await _contentService.ContentWrite(message), "text/html");
            }
            if (user.ActiveCode == activeCode)
            {
                user.Actived = true;
                user.ActiveCode = null;
                await _context.SaveChangesAsync();
                message = "Active account successfully!";
                return Content(await _contentService.ContentWrite(message), "text/html");
            }
            else 
            {
                message = "URL not found!";
                return Content(await _contentService.ContentWrite(message), "text/html");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginOutputDto>> Login([FromBody] UserLoginInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            if (input.Password != user.Password) return BadRequest(new {message = "Wrong password, please check again!"});
            if (user.Actived != true) return BadRequest(new {message = "Account is unactivated, please check your Email!"});
            else return Ok(new UserLoginOutputDto()
            {
                Username = user.Username,
                AvatarURL = user.AvatarUrl,
                Token = _tokenService.CreateToken(user),
            });
        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] UserForgetPasswordInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            if (user.Email != input.Email) return BadRequest(new {message = "Wrong Email!"});
            if (user.Actived == false) return BadRequest(new {message = "Account Account is unactivated!"});
            else
            {
                return await _email.SendEmail(new EmailModel()
                {
                    To = user.Email,
                    Subject = "Forgot your password!",
                    Body = "<h2>Dear" + user.Username + ", please don't share your password for anyone, even ADMIN!</h2><h3>Your password is:</h3>" + user.Password,
                });
            }
        }

        //[Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] UserChangePasswordInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            if (user.Password != input.CurrentPassword) return BadRequest(new {message = "Wrong current password!"});
            else
            {
                user.Password = input.NewPassword;
                await _context.SaveChangesAsync();
                return Ok(new {message = "Change password successfully!"});
            }
        }

        //[Authorize]
        [HttpPost("ChangeEmail")]
        public async Task<ActionResult> ChangeEmail([FromBody] UserChangeEmailInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            if (user.Password != input.CurrentPassword) return BadRequest(new {message = "Wrong current password!"});
            if (user.Email != input.CurrentEmail) return BadRequest(new {message = "Wrong current Email!"});
            if (await EmailExists(input.NewEmail))
            {
                return BadRequest(new {message = "Email already in use!"});
            }
            else
            {
                user.Email = input.NewEmail;
                await _context.SaveChangesAsync();
                return Ok(new {message = "Change Email successfully!"});
            }
        }
        
        [Authorize]
        [HttpPost("ChangeAvatarUrl")]
        public async Task<ActionResult> ChangeAvatarUrl([FromBody] UserChangeAvatarUrlInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == input.Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            user.AvatarUrl = input.NewAvatarUrl;
            await _context.SaveChangesAsync();
            return Ok(new {message = "Change Avatar successful!"});
        }

        //[Authorize]
        [HttpGet("GetMoney")]
        public async Task<ActionResult> GetMoney([FromQuery] string Username)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Username == Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            return Ok(new {money = user.Money});
        }
    }
}