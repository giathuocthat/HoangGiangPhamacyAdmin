    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IRepository<Province> _provinceRepo;
        private readonly IRepository<Ward> _wardRepo;

        public ProvinceController(IRepository<Province> provinceRepo, IRepository<Ward> wardRepo)
        {
            _provinceRepo = provinceRepo;
            _wardRepo = wardRepo;
        }

        // GET: api/province
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var provinces = await _provinceRepo.GetAllAsync();
            var result = provinces.OrderBy(p => p.Name).Select(p => new ProvinceListDto(p.Id, p.Name, p.Code, p.CountryId)).ToList();
            return Ok(result);
        }

        // GET: api/province/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var province = await _provinceRepo.GetByIdAsync(id);
            if (province == null) return NotFound();

            var wards = (await _wardRepo.FindAsync(w => w.ProvinceId == id))
                        .Select(w => new WardListDto(w.Id, w.Name, w.Code))
                        .ToList();

            var detail = new ProvinceDetailDto(
                province.Id,
                province.Name,
                province.Code,
                province.Type,
                province.CountryId,
                province.ZipCode,
                wards
            );

            return Ok(detail);
        }

        // DTOs
        private sealed record ProvinceListDto(int Id, string Name, string? Code, int CountryId);
        private sealed record WardListDto(int Id, string? Name, string? Code);
        private sealed record ProvinceDetailDto(int Id, string Name, string? Code, string? Type, int CountryId, string? ZipCode, System.Collections.Generic.List<WardListDto> Wards);
    }
}
