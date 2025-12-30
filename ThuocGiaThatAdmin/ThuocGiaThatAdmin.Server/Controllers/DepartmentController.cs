using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseApiController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService,
            ILogger<DepartmentController> logger) : base(logger)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// GET: api/department
        /// Lấy danh sách tất cả departments đang active
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            return await ExecuteActionAsync(async () =>
            {
                var departments = await _departmentService.GetAllDepartmentsAsync();
                return Success(departments);
            });
        }

        /// <summary>
        /// GET: api/department/paged
        /// Lấy danh sách departments với phân trang
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedDepartments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchText = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (pageSize > 100)
                {
                    return BadRequestResponse("Page size cannot exceed 100");
                }

                var (departments, totalCount) = await _departmentService.GetPagedDepartmentsAsync(
                    pageNumber,
                    pageSize,
                    searchText);

                var response = new
                {
                    Data = departments,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            });
        }

        /// <summary>
        /// GET: api/department/{id}
        /// Lấy thông tin chi tiết department theo ID với thống kê
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var department = await _departmentService.GetDepartmentByIdAsync(id);

                if (department == null)
                {
                    return NotFoundResponse("Department not found");
                }

                return Success(department);
            });
        }

        /// <summary>
        /// POST: api/department
        /// Tạo department mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, department) = await _departmentService.CreateDepartmentAsync(dto);

                if (!success)
                {
                    return BadRequestResponse(message);
                }

                return Created($"/api/department/{department!.Id}", department);
            });
        }

        /// <summary>
        /// PUT: api/department/{id}
        /// Cập nhật thông tin department
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, department) = await _departmentService.UpdateDepartmentAsync(id, dto);

                if (!success)
                {
                    if (message == "Department not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(department, message);
            });
        }

        /// <summary>
        /// DELETE: api/department/{id}
        /// Xóa department (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message) = await _departmentService.DeleteDepartmentAsync(id);

                if (!success)
                {
                    if (message == "Department not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(new { message });
            });
        }

        // ========== Department Role Management Endpoints ==========

        /// <summary>
        /// GET: api/department/{id}/roles
        /// Lấy danh sách roles của department
        /// </summary>
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetDepartmentRoles(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var roles = await _departmentService.GetDepartmentRolesAsync(id);
                return Success(roles);
            });
        }

        /// <summary>
        /// POST: api/department/{id}/roles
        /// Assign role cho department
        /// </summary>
        [HttpPost("{id}/roles")]
        public async Task<IActionResult> AssignRoleToDepartment(int id, [FromBody] AssignRoleToDepartmentDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message) = await _departmentService.AssignRoleToDepartmentAsync(id, dto);

                if (!success)
                {
                    if (message == "Department not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(new { message });
            });
        }

        /// <summary>
        /// DELETE: api/department/{id}/roles/{roleId}
        /// Remove role khỏi department
        /// </summary>
        [HttpDelete("{id}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromDepartment(int id, string roleId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message) = await _departmentService.RemoveRoleFromDepartmentAsync(id, roleId);

                if (!success)
                {
                    return NotFoundResponse(message);
                }

                return Success(new { message });
            });
        }

        /// <summary>
        /// GET: api/department/{id}/available-roles
        /// Lấy danh sách roles chưa được assign cho department
        /// </summary>
        [HttpGet("{id}/available-roles")]
        public async Task<IActionResult> GetAvailableRoles(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var roles = await _departmentService.GetAvailableRolesForDepartmentAsync(id);
                return Success(roles);
            });
        }
    }
}
