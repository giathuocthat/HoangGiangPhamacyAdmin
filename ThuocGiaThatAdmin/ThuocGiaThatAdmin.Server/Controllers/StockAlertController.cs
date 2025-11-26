using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class StockAlertController : BaseApiController
    {
        private readonly StockAlertService _alertService;

        public StockAlertController(
            StockAlertService alertService,
            ILogger<StockAlertController> logger) : base(logger)
        {
            _alertService = alertService;
        }

        /// <summary>
        /// Get unread alerts
        /// </summary>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            return await ExecuteActionAsync(async () =>
            {
                var alerts = await _alertService.GetUnreadAlertsAsync();
                return Ok(alerts);
            }, "Get Unread Alerts");
        }

        /// <summary>
        /// Get unresolved alerts
        /// </summary>
        [HttpGet("unresolved")]
        public async Task<IActionResult> GetUnresolved()
        {
            return await ExecuteActionAsync(async () =>
            {
                var alerts = await _alertService.GetUnresolvedAlertsAsync();
                return Ok(alerts);
            }, "Get Unresolved Alerts");
        }

        /// <summary>
        /// Get alerts by type
        /// </summary>
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(AlertType type)
        {
            return await ExecuteActionAsync(async () =>
            {
                var alerts = await _alertService.GetAlertsByTypeAsync(type);
                return Ok(alerts);
            }, "Get Alerts By Type");
        }

        /// <summary>
        /// Get alerts by priority
        /// </summary>
        [HttpGet("priority/{priority}")]
        public async Task<IActionResult> GetByPriority(AlertPriority priority)
        {
            return await ExecuteActionAsync(async () =>
            {
                var alerts = await _alertService.GetAlertsByPriorityAsync(priority);
                return Ok(alerts);
            }, "Get Alerts By Priority");
        }

        /// <summary>
        /// Mark alert as read
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _alertService.MarkAsReadAsync(id);
                return Ok("Alert marked as read");
            }, "Mark Alert As Read");
        }

        /// <summary>
        /// Resolve alert
        /// </summary>
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveAlertDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // TODO: Get userId from authentication
                await _alertService.ResolveAlertAsync(id, dto, userId: null);
                return Ok("Alert resolved successfully");
            }, "Resolve Alert");
        }
    }
}
