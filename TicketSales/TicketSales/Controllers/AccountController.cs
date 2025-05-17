using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using TicketSales.Data;
using TicketSales.Helper;
using TicketSales.Models;
using TicketSales.Services.Interfaces;
using TicketSales.ViewModels;
using Microsoft.Win32;

namespace TicketSales.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public AccountController(AppDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm login)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(login);
            }
            
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == login.Email); 

            var checkUser = PasswordHash.VerifyHashedPassword(user.Password,login.Password);
            if (!checkUser)
            {
                ModelState.AddModelError("", "Email və ya şifrə yanlışdır.");
                return View(login);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) 
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]  
        public IActionResult Register(RegisterVm register)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(register);
            }

            bool usernameExists = _context.Users.Any(u => u.Username == register.Username);
            bool emailExists = _context.Users.Any(u => u.Email == register.Email);

            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Bu istifadəçi adı artıq mövcuddur.");
            }

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Bu email artıq istifadə olunub.");
            }

            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var verificationCode = new Random().Next(1000, 9999).ToString();
            var emailHtml = $@"
    <div style='font-family: Arial, sans-serif; color: #111;'>
        <h2 style='color: #3b82f6;'>Təsdiq Kodu</h2>
        <p>Hörmətli {register.FullName},</p>
        <p>Qeydiyyatınızı tamamlamanız üçün aşağıdakı 4 rəqəmli kodu daxil edin:</p>
        <div style='font-size: 2rem; font-weight: bold; color: #ec4899; margin: 1rem 0;'>{verificationCode}</div>
        <p>Əgər siz bu sorğunu göndərməmisinizsə, zəhmət olmasa bu emaili nəzərə almayın.</p>
        <hr />
        <small style='color: #888;'>Thanks for attention</small>
    </div>";


            HttpContext.Session.SetString("VerificationEmail", register.Email);
            HttpContext.Session.SetString("VerificationCode", verificationCode);
            HttpContext.Session.SetString("Method", "Register");

            _emailService.Send(register.Email, "Təsdiq Kodu", emailHtml);

            var hashedPassword = PasswordHash.HashPassword(register.Password);  

            var user = new AppUser()
            {
                Email = register.Email,
                Password = hashedPassword,
                Username = register.Username,
                FullName = register.FullName,
                Role = "User"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("EnterVerificationCode");
        }
        [HttpGet]
        public IActionResult EnterEmail()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EnterEmail(EnterEmailVm enterEmail)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(enterEmail);
            }
            var user = _context.Users.FirstOrDefault(x => x.Email == enterEmail.Email);   
            if(user == null)
            {
                ModelState.AddModelError("", "Belə bir email yoxdur !.");
                return View(enterEmail);
            }
            else
            {
                var verificationCode = new Random().Next(1000, 9999).ToString();
                var emailHtml = $@"
    <div style='font-family: Arial, sans-serif; color: #111;'>
        <h2 style='color: #3b82f6;'>Təsdiq Kodu</h2>
        <p>Hörmətli İstifadəçi</p>
        <p>Parol dəyişməni tamamlamanız üçün aşağıdakı 4 rəqəmli kodu daxil edin:</p>
        <div style='font-size: 2rem; font-weight: bold; color: #ec4899; margin: 1rem 0;'>{verificationCode}</div>
        <p>Əgər siz bu sorğunu göndərməmisinizsə, zəhmət olmasa bu emaili nəzərə almayın.</p>
        <hr />
        <small style='color: #888;'>Thanks for attention</small>
    </div>";


                HttpContext.Session.SetString("VerificationEmail", enterEmail.Email);
                HttpContext.Session.SetString("VerificationCode", verificationCode);
                HttpContext.Session.SetString("Method", "ResetPassword");



                _emailService.Send(enterEmail.Email, "Təsdiq Kodu", emailHtml);

                return RedirectToAction("EnterVerificationCode");
            }

           
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordVm resetPassword)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun.");
                return View(resetPassword);
            }
            var email = HttpContext.Session.GetString("VerificationEmail");
            var user = _context.Users.FirstOrDefault(m => m.Email == email);
            
            var newHashedPassword = PasswordHash.HashPassword(resetPassword.Password);
            user.Password = newHashedPassword;
            _context.SaveChanges();
            HttpContext.Session.Remove("VerificationCode");
            HttpContext.Session.Remove("VerificationEmail");
            HttpContext.Session.Remove("Method");
            return RedirectToAction("Login");
            
        }

        [HttpGet]
        public IActionResult EnterVerificationCode()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EnterVerificationCode(string digit1, string digit2, string digit3, string digit4)
        {
            var method = HttpContext.Session.GetString("Method");
            var enteredCode = $"{digit1}{digit2}{digit3}{digit4}";

            var sessionCode = HttpContext.Session.GetString("VerificationCode");

            var email = HttpContext.Session.GetString("VerificationEmail");
            var user = _context.Users.FirstOrDefault(m => m.Email == email);
            if (method == "Register")
            {
              
                if (sessionCode != null && enteredCode == sessionCode)
                {

                    if (user != null)
                    {
                        user.EmailConfirm = true;
                        _context.SaveChanges();
                    }
                    else
                    {
                        return View(enteredCode);
                    }

                    HttpContext.Session.Remove("VerificationCode");
                    HttpContext.Session.Remove("VerificationEmail");
                    HttpContext.Session.Remove("Method");

                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Daxil etdiyiniz kod yanlışdır, zəhmət olmasa yenidən cəhd edin.";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                
                if (sessionCode != null && enteredCode == sessionCode)
                { 

                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    ViewBag.Error = "Daxil etdiyiniz kod yanlışdır, zəhmət olmasa yenidən cəhd edin.";
                    return RedirectToAction("Login");
                }
            }
          
        }

    }
}
