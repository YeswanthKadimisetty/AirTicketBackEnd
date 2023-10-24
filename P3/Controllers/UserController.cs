﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P3.Models;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace P2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext context;
        public UsersController(MyDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await context.UsersTable.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUser addUser)
        {
            var user = new UserDetails()
            {
                Name = addUser.Name,
                Email = addUser.Email,
                Password = addUser.Password,
                Phone = addUser.Phone
            };

            MailMessage message = new MailMessage();
            message.From = new MailAddress("aticket79@gmail.com");
            message.Subject = $"{addUser.Name},Welcome to AirTicket.com!";
            message.To.Add(new MailAddress(addUser.Email));
            message.Body = $"<html><body> <h3> {addUser.Name},Welcome to Booking.com!</h3> <p>There’s a lot of world out there to explore, and your new account will help you do just that.</p></body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("aticket79@gmail.com", "qiwp turh muvt bkwi"),
                EnableSsl = true,
            };

            smtpClient.Send(message);

            await context.UsersTable.AddAsync(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUser updateUser)
        {
            var user = await context.UsersTable.FindAsync(id);
            if (user != null)
            {
                user.Name = updateUser.UserName;
                user.Email = updateUser.Email;
                user.Password = updateUser.Password;
                user.Phone = updateUser.Phone;

                await context.SaveChangesAsync();
                return Ok(user);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("{password}")]
        public async Task<IActionResult> GetUserByPassword(string password)
        {
            var user = await context.UsersTable.Where(m => m.Password == password).ToListAsync();
            if (user != null)
            {
                return Ok(user[0]);
            }
            return NotFound("User Not Found");

        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await context.UsersTable.FindAsync(id);
            if (user != null)
            {
                context.Remove(user);
                await context.SaveChangesAsync();
                return Ok($"User {id} deleted");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("{email},{password}")]
        public async Task<IActionResult> Login(string email,string password)
        {
            //var user = await context.Users.FindAsync(id);
            var user = await context.UsersTable.Where(m => m.Email == email).ToListAsync();
            var pw = await context.UsersTable.Where(m => m.Password == password).ToListAsync();
            if (user.Count() != 0 && pw.Count() != 0)
            {
                return Ok(user[0]);
            }
            return NotFound();


        }
    }
}
