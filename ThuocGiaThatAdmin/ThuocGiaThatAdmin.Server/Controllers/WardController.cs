        using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardController : ControllerBase
    {
        private readonly IRepository<Ward> _wardRepo;
        private readonly IRepository<Province> _provinceRepo;

        public WardController(IRepository<Ward> wardRepo, IRepository<Province> provinceRepo)
        {
            _wardRepo = wardRepo;
            _provinceRepo = provinceRepo;
        }

        // GET: api/ward
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var wards = await _wardRepo.GetAllAsync();
            var result = wards.Select(w => new WardListDto(w.Id, w.Name, w.Code, w.ProvinceId)).ToList();
            return Ok(result);
        }

        // GET: api/ward/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var ward = await _wardRepo.GetByIdAsync(id);
            if (ward == null) return NotFound();

            Province? province = null;
            if (ward.ProvinceId.HasValue)
            {
                province = await _provinceRepo.GetByIdAsync(ward.ProvinceId.Value);
            }

            var detail = new WardDetailDto(
                ward.Id,
                ward.Name,
                ward.Code,
                ward.Type,
                ward.ProvinceId,
                ward.ZipCode,
                province is null ? null : new ProvinceBriefDto(province.Id, province.Name, province.Code)
            );

            return Ok(detail);
        }

        // GET: api/ward
        [HttpGet("GetByProvinceId/{provinceId:int}")]
        public async Task<IActionResult> GetByProvinceId(int provinceId)
        {
            var wards = await _wardRepo.FindAsync(x => x.ProvinceId == provinceId);
            var result = wards.Select(w => new WardListDto(w.Id, w.Name, w.Code, w.ProvinceId)).ToList();
            return Ok(result);
        }

        // DTOs
        private sealed record WardListDto(int Id, string? Name, string? Code, int? ProvinceId);
        private sealed record ProvinceBriefDto(int Id, string Name, string? Code);
        private sealed record WardDetailDto(int Id, string? Name, string? Code, string? Type, int? ProvinceId, string? ZipCode, ProvinceBriefDto? Province);
    }
}
