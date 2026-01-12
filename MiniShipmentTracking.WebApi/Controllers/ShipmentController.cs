using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniShipmentTracking.WebApi.Dtos;
using MiniShipmentTracking.WebApi.Services;

namespace MiniShipmentTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipment(ShipmentCreateRequestDto request)
        {
            if (!HttpContext.HasRole(UserRole.Admin, UserRole.Staff))
                return Forbid();
            
            int userId = HttpContext.GetUserId();
            var result = await _shipmentService.CreateShipment(userId, request);

            if (result.IsValidatorError)
                return BadRequest(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpPatch("{shipmentId}/events")]
        public async Task<IActionResult> UpdateShipmentStatus(int shipmentId, ShipmentUpdateStatusRequestDto request)
        {
            if (!HttpContext.HasRole(UserRole.Admin, UserRole.Staff))
                return Forbid();
            
            int userId = HttpContext.GetUserId();
            var result = await _shipmentService.UpdateShipmentStatus(userId, shipmentId, request);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }
    }
}
