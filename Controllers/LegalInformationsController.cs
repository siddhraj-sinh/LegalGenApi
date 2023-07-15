using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LegalGenApi.Data;
using LegalGenApi.Models;

namespace LegalGenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegalInformationsController : ControllerBase
    {
        private readonly LegalGenContext _context;

        public LegalInformationsController(LegalGenContext context)
        {
            _context = context;
        }

        // GET: api/LegalInformations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LegalInformation>>> GetLegalInformation()
        {
            return await _context.LegalInformation.ToListAsync();
        }

        // GET: api/LegalInformations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LegalInformation>> GetLegalInformation(int id)
        {
            var legalInformation = await _context.LegalInformation.FindAsync(id);

            if (legalInformation == null)
            {
                return NotFound();
            }

            return legalInformation;
        }

        // PUT: api/LegalInformations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLegalInformation(int id, LegalInformation legalInformation)
        {
            if (id != legalInformation.Id)
            {
                return BadRequest();
            }

            _context.Entry(legalInformation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LegalInformationExists(id))
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

        // POST: api/LegalInformations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LegalInformation>> PostLegalInformation(LegalInformation legalInformation)
        {
            _context.LegalInformation.Add(legalInformation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLegalInformation", new { id = legalInformation.Id }, legalInformation);
        }

        // DELETE: api/LegalInformations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLegalInformation(int id)
        {
            var legalInformation = await _context.LegalInformation.FindAsync(id);
            if (legalInformation == null)
            {
                return NotFound();
            }

            _context.LegalInformation.Remove(legalInformation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LegalInformationExists(int id)
        {
            return _context.LegalInformation.Any(e => e.Id == id);
        }
    }
}
