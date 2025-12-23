using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Server.Models;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class ActiveIngredientController : BaseApiController
    {
        private readonly ActiveIngredientService _service;

        public ActiveIngredientController(ActiveIngredientService service, ILogger<ActiveIngredientController> logger)
            : base(logger)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all active ingredients
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var ingredients = await _service.GetAllAsync();
                return Ok(ingredients);
            }, "Get All Active Ingredients");
        }

        /// <summary>
        /// Search active ingredients by name
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Ok(await _service.GetAllAsync());

                var ingredients = await _service.SearchByNameAsync(q);
                return Ok(ingredients);
            }, "Search Active Ingredients");
        }

        /// <summary>
        /// Create a new active ingredient
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActiveIngredientRequest request)
        {
            return await ExecuteActionAsync(async () =>
            {
                var ingredient = await _service.CreateAsync(request.Name);
                return Created(ingredient, "Active ingredient created successfully");
            }, "Create Active Ingredient");
        }
    }

    public class CreateActiveIngredientRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
