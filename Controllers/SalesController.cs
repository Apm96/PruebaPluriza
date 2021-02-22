using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSales.Models;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProjectSales.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly WideWorldImportersContext _context;

        public SalesController(WideWorldImportersContext context)
        {
            _context = context;
        }

        public SalesController()
        {
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sales>>> GetSales()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var IdUserType = identity.FindFirst("IdUserType").Value;

                if (Convert.ToInt32(IdUserType) != Constantes.USUARIOADMINISTRADOR)
                {
                    return BadRequest("You are not authorized to perform this action");
                }
                else
                {
                    return await _context.Sales.Where(x => x.IdEstate == Constantes.ESTADOACTIVO).ToListAsync();
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =  new System.Globalization.CultureInfo("en-US");
                decimal Total = 0;
                //var apiKey = Environment.GetEnvironmentVariable("ClaveApi");
                var client = new SendGridClient("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");
                var from = new EmailAddress("apm96@misena.edu.co", "Andres Peinado Mazzilli");
                var subject = "Sales information " + DateTime.Now.AddDays(-1).ToString("yyyy /MM/dd");
                var to = new EmailAddress("hr@pluriza.com", "Pluriza");
                var plainTextContent = "Sales information of the day: " + DateTime.Now.AddDays(-1).ToString("yyyy /MM/dd");

                var sales = await _context.Sales.Where(x => x.Date.Value.Date == DateTime.Now.Date.AddDays(-1)).ToListAsync();

                if (sales.Count > 0)
                {                   
                    foreach (var ventas in sales)
                    {
                        ventas.SaleDetail = _context.SaleDetail.Where(x => x.IdSales == ventas.Id).ToList();
                        foreach (var item in ventas.SaleDetail)
                        {
                            Total += (Convert.ToDecimal(string.IsNullOrEmpty(item.Amount.ToString()) ? 0 : item.Amount) * Convert.ToDecimal(string.IsNullOrEmpty(item.Price.ToString()) ? 0 : item.Price));
                        }
                    }
                }              

                var htmlContent = "<strong>Total sales of the previous day: " + Total.ToString("c") + " </strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
          
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        ///  Get Object for settled
        /// </summary>
        /// <param name="settled"></param>
        /// <returns></returns>
        // GET: api/Sales/5
        [HttpGet("{settled}")]
        public async Task<ActionResult<Sales>> GetSales(string settled)
        {
            var sales = await _context.Sales.Where(x => x.Settled == settled).FirstOrDefaultAsync();

            sales.SaleDetail = _context.SaleDetail.Where(x => x.IdSales == sales.Id).ToList();

            if (sales == null)
            {
                return NotFound();
            }

            return sales;
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// PUT for id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sales"></param>
        /// <returns></returns>
        // PUT: api/Sales/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSales(int id, Sales sales)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var IdUserType = identity.FindFirst("IdUserType").Value;

            if (id != sales.Id)
            {
                return BadRequest();
            }

            if (!SalesExistsSettled(sales.Settled))
            {
                return BadRequest("There is no sale with this filing number");
            }

            if (Convert.ToInt32(IdUserType) == Constantes.USUARIONORMAL)
            {
                return BadRequest("You are not authorized to perform this action");
            }

            try
            {
                var existingSale = _context.Sales.Include(b => b.SaleDetail).FirstOrDefault(b => b.Id == sales.Id);

                if (existingSale == null)
                {
                    return BadRequest(); //or 404 response, or custom exception, etc...
                }
                else
                {
                    _context.Entry(existingSale).CurrentValues.SetValues(sales);
                    foreach (var saledetail in sales.SaleDetail)
                    {
                        var existingSaleDetail = existingSale.SaleDetail
                            .FirstOrDefault(p => p.Id == saledetail.Id);

                        if (existingSaleDetail == null)
                        {
                            existingSale.SaleDetail.Add(saledetail);
                        }
                        else
                        {
                            _context.Entry(existingSaleDetail).CurrentValues.SetValues(saledetail);
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return Ok("Updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Post new Sale in format JSON
        /// </summary>
        /// <param name="sales"></param>
        /// <returns></returns>
        // POST: api/Sales
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sales>> PostSales(Sales sales)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var IdUserType = identity.FindFirst("IdUserType").Value;
                var IdUser = identity.FindFirst("Id").Value;

                if (Convert.ToInt32(IdUserType) == Constantes.USUARIOADMINISTRADOR)
                {
                    return BadRequest("You are not authorized to perform this action");
                }

                if (string.IsNullOrEmpty(sales.Settled) || string.IsNullOrEmpty(sales.Date.ToString()))
                {
                    return BadRequest("Empty mandatory fields were found: \n" + JsonConvert.SerializeObject(sales));
                }

                if (!DateTime.TryParse(sales.Date.ToString(), out DateTime Fecha))
                {
                    return BadRequest("Incorrect document date");
                }

                if (sales.SaleDetail.Count == 0)
                {
                    return BadRequest("The sale must have a maximum of one item");
                }

                if (SalesExistsSettled(sales.Settled))
                {
                    return BadRequest("There is already a sales document with this filing number");
                }
                else
                {
                    sales.IdUser = Convert.ToInt32(IdUser);
                    sales.IdEstate = Constantes.ESTADOACTIVO;
                    sales.AuditDate = DateTime.Now;
                    _context.Sales.Add(sales);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetSales", new { id = sales.Id }, sales);
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
        /// Delete, but update state of sale to inactive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sales>> DeleteSales(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var IdUserType = identity.FindFirst("IdUserType").Value;

            if (Convert.ToInt32(IdUserType) == Constantes.USUARIONORMAL)
            {
                return BadRequest("You are not authorized to perform this action");
            }

            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }

            _context.Sales.Update(sales);
            sales.IdEstate = Constantes.ESTADOINACTIVO;
            await _context.SaveChangesAsync();

            return Ok("Record removed");
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Get bool if Existes register for id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool SalesExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }

        /// Autor: Andres Peinado Mazzilli
        /// Fecha: 2021/02/21
        /// <summary>
        /// Get bool if Existes register for settled
        /// </summary>
        /// <param name="settled"></param>
        /// <returns></returns>
        private bool SalesExistsSettled(string settled)
        {
            return _context.Sales.Any(e => e.Settled == settled);
        }
    }
}
