using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectSales.Models;

namespace ProjectSales.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WideWorldImportersContext _context;

        public UsersController(WideWorldImportersContext context)
        {
            _context = context;
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Get List User
        /// </summary>
        /// <returns></returns>
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Get User for id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Update User for id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
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

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {

                if (string.IsNullOrEmpty(user.IdUserType.ToString()) || string.IsNullOrEmpty(user.Userr) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
                {
                    return BadRequest("Empty mandatory fields were found: \n" + JsonConvert.SerializeObject(user));
                }

                if (user.Password.Length < 4)
                {
                    return BadRequest("The password must have more than 4 characters");
                }

                if (UserExistsUser(user.Userr))
                {
                    return BadRequest("This user is already registered");
                }
                else if (user.IdUserType != Constantes.USUARIOADMINISTRADOR && user.IdUserType != Constantes.USUARIONORMAL)
                {
                    return BadRequest("Allowed user types: \n 1 - Administrator \n 2 - Normal");
                }
                else
                {
                    user.IdEstate = Constantes.ESTADOACTIVO;
                    _context.User.Add(user);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetUser", new { id = user.Id }, user);
                }               
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }         
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Delete user for id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Validate exists for id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Validate Exists for User
        /// </summary>
        /// <param name="userr"></param>
        /// <returns></returns>
        private bool UserExistsUser(string userr)
        {
            return _context.User.Any(e => e.Userr == userr);
        }
    }
}
