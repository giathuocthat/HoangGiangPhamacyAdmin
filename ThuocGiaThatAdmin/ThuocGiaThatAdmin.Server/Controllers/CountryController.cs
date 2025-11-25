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
    public class CountryController : ControllerBase
    {
        private readonly IRepository<Country> _countryRepo;
        private readonly IRepository<Province> _provinceRepo;

        public CountryController(IRepository<Country> countryRepo, IRepository<Province> provinceRepo)
        {
            _countryRepo = countryRepo;
            _provinceRepo = provinceRepo;
        }

        // GET: api/country
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _countryRepo.GetAllAsync();
            var result = countries.Select(c => new CountryListDto(c.Id, c.CountryCode, c.CommonName)).ToList();
            return Ok(result);
        }

        // GET: api/country/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var country = await _countryRepo.GetByIdAsync(id);
            if (country == null) return NotFound();

            var provinces = (await _provinceRepo.FindAsync(p => p.CountryId == id))
                            .Select(p => new ProvinceListDto(p.Id, p.Name, p.Code))
                            .ToList();

            var detail = new CountryDetailDto(
                country.Id,
                country.CountryCode,
                country.CommonName,
                country.FormalName,
                country.Capital,
                provinces
            );

            return Ok(detail);
        }

        // DTOs
        private sealed record CountryListDto(int Id, string CountryCode, string? CommonName);
        private sealed record ProvinceListDto(int Id, string Name, string? Code);
        private sealed record CountryDetailDto(int Id, string CountryCode, string? CommonName, string? FormalName, string? Capital, System.Collections.Generic.List<ProvinceListDto> Provinces);
    }
}
