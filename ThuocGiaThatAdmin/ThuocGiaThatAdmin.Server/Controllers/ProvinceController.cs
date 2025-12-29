    using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using ThuocGiaThatAdmin.Common.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IRepository<Province> _provinceRepo;
        private readonly IRepository<Ward> _wardRepo;
        private readonly ICacheService _cacheService;

        public ProvinceController(IRepository<Province> provinceRepo, IRepository<Ward> wardRepo, ICacheService cacheService)
        {
            _provinceRepo = provinceRepo;
            _wardRepo = wardRepo;
            _cacheService = cacheService;
        }

        // GET: api/province
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string key = "provinces:all";
            var result = await _cacheService.GetOrSetAsync(key, async () =>
            {
                var provinces = await _provinceRepo.GetAllAsync();
                return provinces.OrderBy(p => p.Name).Select(p => new ProvinceListDto(p.Id, p.Name, p.Code, p.CountryId)).ToList();
            });

            return Ok(result);
        }

        // GET: api/province/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            string key = $"province:{id}";

            var data = await _cacheService.GetOrSetAsync(key, async () =>
            {
                var province = await _provinceRepo.GetByIdAsync(id);

                if (province == null) return null;

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

                return detail;
            });

            return Ok(data);
        }

        // DTOs
        private sealed record ProvinceListDto(int Id, string Name, string? Code, int CountryId);
        private sealed record WardListDto(int Id, string? Name, string? Code);
        private sealed record ProvinceDetailDto(int Id, string Name, string? Code, string? Type, int CountryId, string? ZipCode, System.Collections.Generic.List<WardListDto> Wards);
    }
}
