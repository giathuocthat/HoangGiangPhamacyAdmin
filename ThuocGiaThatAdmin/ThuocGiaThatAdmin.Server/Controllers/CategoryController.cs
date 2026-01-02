using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Commands.Categories;
using ThuocGiaThatAdmin.Common.Interfaces;
using ThuocGiaThatAdmin.Contracts.Responses;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Queries.IQueries;
using ThuocGiaThatAdmin.Service;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly ILogger<CategoryController> _logger;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public CategoryController(ICategoryService service, ILogger<CategoryController> logger, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger;
            _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
            _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        }

        /// <summary>
        /// Get all categories with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, total) = await _service.GetCategoriesAsync(pageNumber, pageSize);
                var response = new
                {
                    Data = items.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Slug,
                        c.Description,
                        c.ParentId,
                        c.DisplayOrder,
                        c.IsActive,
                        c.CreatedDate,
                        c.UpdatedDate
                    }),
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
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { message = $"Category with id {id} not found" });
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by id");
                return StatusCode(500, new { message = "An error occurred while retrieving the category", error = ex.Message });
            }
        }

        /// <summary>
        /// Get root categories (parent categories only)
        /// </summary>
        [HttpGet("root")]
        public async Task<IActionResult> GetRootCategories()
        {
            try
            {
                var items = await _service.GetRootCategoriesAsync();                
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root categories");
                return StatusCode(500, new { message = "An error occurred while retrieving root categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get root categories (parent categories only)
        /// </summary>
        [HttpGet("rootCountProducts")]
        public async Task<IActionResult> GeCategoryRootCountProducts()
        {
            try
            {
                var items = await _service.GetCategoryRootCountProductsAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting root categories");
                return StatusCode(500, new { message = "An error occurred while retrieving root categories", error = ex.Message });
            }
        }


        /// <summary>
        /// Get child categories by parent ID
        /// </summary>
        [HttpGet("{parentId:int}/children")]
        public async Task<IActionResult> GetChildCategories(int parentId)
        {
            try
            {
                var items = await _service.GetChildCategoriesAsync(parentId);
                var response = items.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.DisplayOrder,
                    c.IsActive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting child categories");
                return StatusCode(500, new { message = "An error occurred while retrieving child categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get child categories by parent ID
        /// </summary>
        [HttpGet("allchildren")]
        public async Task<IActionResult> GetAllChildrenCategories()
        {
            try
            {
                var items = await _service.GetAllChildrenAsync();
                var response = items.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.DisplayOrder,
                    c.IsActive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all children categories");
                return StatusCode(500, new { message = "An error occurred while retrieving children categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get categories in hierarchical tree structure (nested parent-child)
        /// </summary>
        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetHierarchy()
        {
            try
            {
                var query = new GetCategoryHierarchyQuery();
                var items = await _queryDispatcher.DispatchAsync(query);
               // var items = await _service.GetCategoryHierarchyAsync();
                return Ok(new { Data = items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category hierarchy");
                return StatusCode(500, new { message = "An error occurred while retrieving category hierarchy", error = ex.Message });
            }
        }

        /// <summary>
        /// Get categories in flat view with parent info and child count
        /// </summary>
        [HttpGet("flat")]
        public async Task<IActionResult> GetFlat()
        {
            try
            {
                var items = await _service.GetCategoryFlatAsync();
                return Ok(new { Data = items });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting flat category view");
                return StatusCode(500, new { message = "An error occurred while retrieving flat category view", error = ex.Message });
            }
        }

        /// <summary>
        /// Search categories by name
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest(new { message = "Search term cannot be empty" });

                var items = await _service.SearchByNameAsync(term);
                var response = items.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Slug,
                    c.Description,
                    c.ParentId,
                    c.DisplayOrder,
                    c.IsActive
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching categories");
                return StatusCode(500, new { message = "An error occurred while searching categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category dto)
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
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "An error occurred while creating the category", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing category
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(new { message = "Command cannot be null" });
                command.Id = id;

                var result = await _commandDispatcher.DispatchAsync(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return StatusCode(500, new { message = "An error occurred while updating the category", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a category by ID
        /// </summary>
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
                _logger.LogError(ex, "Error deleting category");
                return StatusCode(500, new { message = "An error occurred while deleting the category", error = ex.Message });
            }
        }
    }
}
