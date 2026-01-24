using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HireBase_Backend.Data;
using HireBase_Backend.DTOs;
using HireBase_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using HireBase_Backend.Filters;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Produces("application/json")]
    [RoleAuthorize("SuperAdmin , SystemAdmin")]
    //[RoleAuthorize("SuperAdmin , SystemAdmin ")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Tenant existence check
            var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId);
            if (!tenantExists) return BadRequest($"Tenant {dto.TenantId} not found.");

            // Email uniqueness check
            var emailClash = await _context.Employees.AnyAsync(e =>
                e.TenantId == dto.TenantId &&
                e.Email.ToLower() == dto.Email.ToLower());
            if (emailClash)
                return Conflict(new { message = "Email already exists in this tenant." });

            // EmployeeCode: generate if null/empty; ensure uniqueness per tenant
            var employeeCode = string.IsNullOrWhiteSpace(dto.EmployeeCode)
                ? await GenerateUniqueEmployeeCodeAsync(dto.TenantId, dto.FirstName, dto.LastName)
                : dto.EmployeeCode!.Trim().ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(dto.EmployeeCode))
            {
                var codeClash = await _context.Employees.AnyAsync(e => e.TenantId == dto.TenantId && e.EmployeeCode == employeeCode);
                if (codeClash) return Conflict(new { message = "Employee code already exists in tenant." });
            }

            // File uploads
            string? photoUrl = null;
            string? resumeUrl = null;
            string? contractUrl = null;
            string? certificationUrl = null;
            var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (dto.Photo != null)
                photoUrl = await SaveFileAsync(dto.Photo, Path.Combine(rootPath, "uploads", "employees", "Photo"));

            if (dto.ResumeFile != null)
                resumeUrl = await SaveFileAsync(dto.ResumeFile, Path.Combine(rootPath, "uploads", "employees", "ResumeFile"));

            if (dto.ContractFile != null)
                contractUrl = await SaveFileAsync(dto.ContractFile, Path.Combine(rootPath, "uploads", "employees", "ContractFile"));

            if (dto.CertificationFile != null)
                certificationUrl = await SaveFileAsync(dto.CertificationFile, Path.Combine(rootPath, "uploads", "employees", "certifications"));

            // Create entity
            var entity = new Employee
            {
                EmployeeID = Guid.NewGuid(),
                TenantId = dto.TenantId,

                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender.Trim(),
                Nationality = dto.Nationality.Trim(),
                MaritalStatus = dto.MaritalStatus.Trim(),
                Address = dto.Address.Trim(),

                Email = dto.Email.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                EmergencyContactName = dto.EmergencyContactName.Trim(),
                EmergencyContactNumber = dto.EmergencyContactNumber.Trim(),

                JobTitle = dto.JobTitle.Trim(),
                EmploymentType = dto.EmploymentType.Trim(),
                EmployeeEducationStatus = dto.EmployeeEducationStatus.Trim(),

                PhotoUrl = photoUrl ?? string.Empty,
                Resume = resumeUrl ?? string.Empty,
                ContractFile = contractUrl ?? string.Empty,
                Certification = certificationUrl ?? string.Empty,

                HireDate = dto.HireDate == default ? DateTime.UtcNow : dto.HireDate,
                EmployeeCode = employeeCode,

                BenefitsEnrollment = dto.BenefitsEnrollment,
                ShiftDetails = dto.ShiftDetails,
                Salary = dto.Salary,
                Currency = dto.Currency,
                PaymentMethod = dto.PaymentMethod,
                BankAccountNumber = dto.BankAccountNumber,
                TaxIdenitificationNumber = dto.TaxIdenitificationNumber,
                PassportNumber = dto.PassportNumber,
                WorkLocation = dto.WorkLocation,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.EmployeeID }, entity);
        }

        // PUT: api/employees/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] EmployeeUpdateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var e = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e == null) return NotFound();

            // Email uniqueness
            if (!string.Equals(e.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailClash = await _context.Employees.AnyAsync(x =>
                    x.TenantId == dto.TenantId && x.Email == dto.Email && x.EmployeeID != e.EmployeeID);
                if (emailClash) return Conflict(new { message = "Email already exists in tenant." });
            }

            var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Handle file uploads
            if (dto.Photo != null && dto.Photo.Length > 0)
                e.PhotoUrl = await UpdateFileAsync(dto.Photo, e.PhotoUrl, Path.Combine(rootPath, "uploads", "employees", "Photo"));

            if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
                e.Resume = await UpdateFileAsync(dto.ResumeFile, e.Resume, Path.Combine(rootPath, "uploads", "employees", "ResumeFile"));

            if (dto.ContractFile != null && dto.ContractFile.Length > 0)
                e.ContractFile = await UpdateFileAsync(dto.ContractFile, e.ContractFile, Path.Combine(rootPath, "uploads", "employees", "ContractFile"));

            if (dto.CertificationFile != null && dto.CertificationFile.Length > 0)
                e.Certification = await UpdateFileAsync(dto.CertificationFile, e.Certification, Path.Combine(rootPath, "uploads", "employees", "certifications"));

            // EmployeeCode uniqueness
            if (!string.IsNullOrWhiteSpace(dto.EmployeeCode) && !string.Equals(e.EmployeeCode, dto.EmployeeCode, StringComparison.Ordinal))
            {
                var codeClash = await _context.Employees.AnyAsync(x =>
                    x.TenantId == dto.TenantId && x.EmployeeCode == dto.EmployeeCode && x.EmployeeID != e.EmployeeID);
                if (codeClash) return Conflict(new { message = "Employee code already exists in tenant." });
                e.EmployeeCode = dto.EmployeeCode.Trim().ToUpperInvariant();
            }
            else if (string.IsNullOrWhiteSpace(e.EmployeeCode))
            {
                e.EmployeeCode = await GenerateUniqueEmployeeCodeAsync(dto.TenantId, dto.FirstName, dto.LastName);
            }

            // Apply changes
            e.FirstName = dto.FirstName.Trim();
            e.LastName = dto.LastName.Trim();
            e.DateOfBirth = dto.DateOfBirth;
            e.Gender = dto.Gender.Trim();
            e.Nationality = dto.Nationality.Trim();
            e.MaritalStatus = dto.MaritalStatus.Trim();
            e.Address = dto.Address.Trim();
            e.Email = dto.Email.Trim();
            e.PhoneNumber = dto.PhoneNumber.Trim();
            e.EmergencyContactName = dto.EmergencyContactName.Trim();
            e.EmergencyContactNumber = dto.EmergencyContactNumber.Trim();
            e.JobTitle = dto.JobTitle.Trim();
            e.EmployeeEducationStatus = dto.EmployeeEducationStatus.Trim();
            e.EmploymentType = dto.EmploymentType.Trim();
            e.HireDate = dto.HireDate;
            e.BenefitsEnrollment = dto.BenefitsEnrollment;
            e.ShiftDetails = dto.ShiftDetails;
            e.Salary = dto.Salary;
            e.Currency = dto.Currency;
            e.PaymentMethod = dto.PaymentMethod;
            e.BankAccountNumber = dto.BankAccountNumber;
            e.TaxIdenitificationNumber = dto.TaxIdenitificationNumber;
            e.PassportNumber = dto.PassportNumber;
            e.WorkLocation = dto.WorkLocation;

            e.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null) return NotFound();

            _context.Employees.Remove(e);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/employees/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var e = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (e == null) return NotFound();
            return Ok(e);
        }

        // GET: api/employees/by-employee-code/{tenantId}/{employeeCode}
        [HttpGet("by-employee-code/{tenantId}/{employeeCode}")]
        public async Task<IActionResult> GetByEmployeeCode(Guid tenantId, string employeeCode)
        {
            if (tenantId == Guid.Empty || string.IsNullOrWhiteSpace(employeeCode))
                return BadRequest("tenantId and employeeCode are required.");

            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.TenantId == tenantId && e.EmployeeCode == employeeCode)
                .FirstOrDefaultAsync();

            if (employee == null) return NotFound($"Employee with code {employeeCode} not found.");
            return Ok(employee);
        }

        // GET: api/employees/total-employees/{tenantId}
        [HttpGet("total-employees/{tenantId}")]
        public async Task<IActionResult> GetTotalEmployees(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
                return BadRequest("tenantId is required.");

            var totalEmployees = await _context.Employees
                .AsNoTracking()
                .CountAsync(e => e.TenantId == tenantId);

            return Ok(new { count = totalEmployees });
        }

        // GET: api/employees
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .ToListAsync();

            return Ok(employees);
        }

        // Helper methods
        private async Task<string> GenerateUniqueEmployeeCodeAsync(Guid tenantId, string firstName, string lastName)
        {
            var basePart = new string($"{firstName}{lastName}".Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
            if (basePart.Length < 4) basePart = (basePart + "XXXX").Substring(0, 4);
            else basePart = basePart.Substring(0, Math.Min(6, basePart.Length));

            string candidate;
            var rnd = RandomNumberGenerator.Create();
            do
            {
                var bytes = new byte[2];
                rnd.GetBytes(bytes);
                var suffix = (BitConverter.ToUInt16(bytes, 0) % 900 + 100);
                candidate = $"{basePart}-{suffix}";
            }
            while (await _context.Employees.AnyAsync(e => e.TenantId == tenantId && e.EmployeeCode == candidate));

            return candidate;
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/employees/{fileName}";
        }

        private async Task<string> UpdateFileAsync(IFormFile file, string? oldFileUrl, string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            if (!string.IsNullOrWhiteSpace(oldFileUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldFileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }
            return await SaveFileAsync(file, folderPath);
        }
    }
}
