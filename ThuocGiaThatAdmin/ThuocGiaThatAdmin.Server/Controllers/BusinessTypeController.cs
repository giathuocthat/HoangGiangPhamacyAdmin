using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessTypeController : ControllerBase
    {
        private readonly BusinessTypeService _service;
        private readonly ILogger<BusinessTypeController> _logger;

        public BusinessTypeController(BusinessTypeService service, ILogger<BusinessTypeController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, total) = await _service.GetBusinessTypesAsync(pageNumber, pageSize);
                var response = new
                {
                    Data = items.Select(i => new { i.Id, i.Name, i.CreatedDate, i.UpdatedDate }),
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = total,
                        TotalPages = (int)Math.Ceiling(total / (double)pageSize)
                    }
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business types");
                return StatusCode(500, new { message = "An error occurred while retrieving business types", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { message = $"BusinessType with id {id} not found" });
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business type by id");
                return StatusCode(500, new { message = "An error occurred while retrieving the business type", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BusinessType dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating business type");
                return StatusCode(500, new { message = "An error occurred while creating the business type", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BusinessType dto)
        {
            try
            {
                if (id != dto.Id) return BadRequest(new { message = "Id mismatch" });
                var updated = await _service.UpdateAsync(dto);
                return Ok(new { message = "Updated", affected = updated });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating business type");
                return StatusCode(500, new { message = "An error occurred while updating the business type", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                return Ok(new { message = "Deleted", affected = deleted });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business type");
                return StatusCode(500, new { message = "An error occurred while deleting the business type", error = ex.Message });
            }
        }
    }
}
