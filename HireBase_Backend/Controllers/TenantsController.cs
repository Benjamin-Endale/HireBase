using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Data;
using HireBase_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using HireBase_Backend.DTOs;
using HireBase_Backend.Filters;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("SuperAdmin, SystemAdmin,HR")]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TenantsController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] Tenant tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant.Domain))
                ModelState.AddModelError(nameof(tenant.Domain), "Domain is required.");

            // Check if domain already exists
            var domainExists = await _context.Tenants
                .AnyAsync(t => t.Domain.ToUpper() == tenant.Domain.Trim().ToUpper());

            if (domainExists)
                return BadRequest(new { message = $"The domain '{tenant.Domain}' is already in use." });

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (tenant.Id == Guid.Empty)
                tenant.Id = Guid.NewGuid();

            tenant.CreatedAt = DateTime.UtcNow;
            tenant.UpdatedAt = DateTime.UtcNow;

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenantById), new { id = tenant.Id }, tenant);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTenantById(Guid id)
        {
            var tenant = await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll()
        {
            var tenants = await _context.Tenants
                .AsNoTracking()
                .Select(t => new TenantDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Domain = t.Domain,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    Industry = t.Industry,
                    Location = t.Location,
                    Description = t.Description,
                    Country = t.Country,
                    TimeZone = t.TimeZone
                })
                .ToListAsync();

            return Ok(tenants);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Tenant body)
        {
            var t = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            if (string.IsNullOrWhiteSpace(body.Domain))
                return BadRequest(new { message = "Domain is required." });

            t.Name = body.Name?.Trim() ?? t.Name;
            t.Domain = body.Domain.Trim();
            t.Industry = body.Industry;
            t.Location = body.Location;
            t.Country = body.Country;
            t.TimeZone = body.TimeZone;
            t.Description = body.Description;

            t.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null) return NotFound();

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null) return NotFound();

            if (status != "Active" && status != "Suspended")
                return BadRequest(new { message = "Status must be 'Active' or 'Suspended'." });

            tenant.Status = status;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { tenant.Id, tenant.Name, tenant.Status });
        }
    }
}
