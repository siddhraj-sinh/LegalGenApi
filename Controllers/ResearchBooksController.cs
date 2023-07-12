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
    public class ResearchBooksController : ControllerBase
    {
        private readonly LegalGenContext _context;

        public ResearchBooksController(LegalGenContext context)
        {
            _context = context;
        }

        // GET: api/ResearchBooks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResearchBook>>> GetResearchBooks()
        {
            return await _context.ResearchBooks.ToListAsync();
        }

        // GET: api/ResearchBooks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResearchBook>> GetResearchBook(int id)
        {
            var researchBook = await _context.ResearchBooks.FindAsync(id);

            if (researchBook == null)
            {
                return NotFound();
            }

            return researchBook;
        }

        // PUT: api/ResearchBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResearchBook(int id, ResearchBook researchBook)
        {
            if (id != researchBook.Id)
            {
                return BadRequest();
            }

            _context.Entry(researchBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResearchBookExists(id))
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

        // POST: api/ResearchBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResearchBook>> PostResearchBook(ResearchBook researchBook)
        {
            _context.ResearchBooks.Add(researchBook);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResearchBook", new { id = researchBook.Id }, researchBook);
        }

        // DELETE: api/ResearchBooks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResearchBook(int id)
        {
            var researchBook = await _context.ResearchBooks.FindAsync(id);
            if (researchBook == null)
            {
                return NotFound();
            }

            _context.ResearchBooks.Remove(researchBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResearchBookExists(int id)
        {
            return _context.ResearchBooks.Any(e => e.Id == id);
        }
    }
}
