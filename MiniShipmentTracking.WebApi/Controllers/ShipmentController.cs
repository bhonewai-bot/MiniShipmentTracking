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
        
        [HttpGet("{pageNo}/{pageSize}")]
        public async Task<IActionResult> GetShipments(int pageNo = 1, int pageSize = 10)
        {
            if (!HttpContext.HasRole(UserRole.Admin, UserRole.Staff))
                return Forbid();
            
            var result = await _shipmentService.GetShipments(pageNo, pageSize);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpGet("{shipmentId}")]
        public async Task<IActionResult> GetShipment(int shipmentId)
        {
            if (!HttpContext.HasRole(UserRole.Admin, UserRole.Staff))
                return Forbid();
            
            var result = await _shipmentService.GetShipment(shipmentId);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }
        
        [HttpGet("track/{trackingNo}")]
        public async Task<IActionResult> GetShipmentByTicketNo(string trackingNo)
        {
            var result = await _shipmentService.GetShipmentByTicketNo(trackingNo);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);

            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
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

        [HttpPatch("events/{eventId}")]
        public async Task<IActionResult> UpdateTrackingEvent(int eventId, TrackingEventUpdateRequestDto request)
        {
            if (!HttpContext.HasRole(UserRole.Admin))
                return Forbid();
            
            int userId = HttpContext.GetUserId();
            var result = await _shipmentService.UpdateTrackingEvent(userId, eventId, request);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }
    }
}
