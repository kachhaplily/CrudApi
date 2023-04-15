using Azure.Core;
using CrudApi.Data;
using CrudApi.Models;
using CrudApi.TaskModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrudApi.Controllers
{

    [ApiController]
    [Route("api/Crud")]
    public class CrudController : Controller
    {
        private readonly CrudApiDbContext crudApiDbContext;
        private readonly IConfiguration _configuration;
        private readonly User userobj = new User();
        private readonly TasksData tasksData = new TasksData();

        public CrudController(CrudApiDbContext crudApiDbContext, IConfiguration configuration)
        {
            this.crudApiDbContext = crudApiDbContext;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await crudApiDbContext.User.ToListAsync());
        }
        [HttpPost("CrudRegistration")]
        public async Task<IActionResult>Crudregistration(registration reg)
        {
            CreatePasswordHash(reg.Password, out byte[] passwordHash, out byte[] PasswordSalt);
            userobj.Id = new Guid();
            userobj.Email = reg.Email;
            userobj.PasswordHash = passwordHash;
            userobj.PasswordSalt = PasswordSalt;
            await crudApiDbContext.User.AddAsync(userobj);
            await crudApiDbContext.SaveChangesAsync();
            return Ok(userobj);
        }
     
        [HttpPost("CrudLogin")]
        public ActionResult<string>Login(registration request)
        {
            if (userobj.Email != request.Email)
            {
                return BadRequest("user not found");
            }
            if (!VerifyPasswordHash(request.Password, userobj.PasswordHash, userobj.PasswordSalt))
            {
                return BadRequest("wrong password");
            }

            string token = createToken(userobj);
            return Ok(token);
        }


        //Hashpassword
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        //verification hascode

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512(userobj.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        // token
        private string createToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
       
        //token end here





        //   Taskdata[method] here  

        [HttpGet("alltask")]
      

        public async Task<ActionResult>AllTasksData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await crudApiDbContext.TasksData.ToListAsync());

        }
    
        [HttpPost("addTask")]
      

        public async Task<ActionResult>addTasks(AddTask addtask )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? maxId = await crudApiDbContext.TasksData.MaxAsync(t => (int?)t.Id);

            // If there are no existing records, start the ID value at 1
            int newId = (maxId ?? 0) + 1;

            // Increment the ID value and assign it to the new task
            tasksData.Id = newId;
            tasksData.TaskTitle = addtask.TaskTitle;
            tasksData.TaskDescription = addtask.TaskDescription;
            tasksData.Deadline = addtask.Deadline;
            tasksData.TaskStatus = addtask.TaskStatus;
            await crudApiDbContext.TasksData.AddAsync(tasksData);
            await crudApiDbContext.SaveChangesAsync();
            return Ok(tasksData);


        }

    }
    
        
    






}

