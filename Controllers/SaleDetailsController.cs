using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSales.Models;

namespace ProjectSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleDetailsController : ControllerBase
    {
        private readonly WideWorldImportersContext _context;

        public SaleDetailsController(WideWorldImportersContext context)
        {
            _context = context;
        }

        // GET: api/SaleDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDetail>>> GetSaleDetail()
        {
            return await _context.SaleDetail.ToListAsync();
        }

        // GET: api/SaleDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDetail>> GetSaleDetail(int id)
        {
            var saleDetail = await _context.SaleDetail.FindAsync(id);

            if (saleDetail == null)
            {
                return NotFound();
            }

            return saleDetail;
        }

        // PUT: api/SaleDetails/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSaleDetail(int id, SaleDetail saleDetail)
        {
            if (id != saleDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(saleDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleDetailExists(id))
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

        // POST: api/SaleDetails
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<SaleDetail>> PostSaleDetail(SaleDetail saleDetail)
        {
            _context.SaleDetail.Add(saleDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSaleDetail", new { id = saleDetail.Id }, saleDetail);
        }

        // DELETE: api/SaleDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SaleDetail>> DeleteSaleDetail(int id)
        {
            var saleDetail = await _context.SaleDetail.FindAsync(id);
            if (saleDetail == null)
            {
                return NotFound();
            }

            _context.SaleDetail.Remove(saleDetail);
            await _context.SaveChangesAsync();

            return saleDetail;
        }

        private bool SaleDetailExists(int id)
        {
            return _context.SaleDetail.Any(e => e.Id == id);
        }
    }
}
