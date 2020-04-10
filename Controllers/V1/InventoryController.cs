using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPI_EF.Contracts.V1;
using CoreAPI_EF.Contracts.V1.Requests;
using CoreAPI_EF.Contracts.V1.Responses;
using CoreAPI_EF.Domain;
using CoreAPI_EF.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreAPI_EF.Controllers.V1
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super, Admin, Manager")]
	[ApiController]
	public class InventoryController : ControllerBase
	{
		private readonly IInventoryService _inventoryService;
		private readonly IMapper _mapper;


		public InventoryController(IInventoryService inventoryService, IMapper mapper)
		{
			_inventoryService = inventoryService;
			_mapper = mapper;
		}

		[HttpGet(ApiRoutes.Inventory.GetAllInventoryByProject)]
		public async Task<IActionResult> GetAllInventoryByProject([FromRoute] string ProjectKey)
		{
			var auth = Request.Headers["Authorization"];
			var token = auth.First().Remove(0, "Bearer ".Length).Trim();
			var allowed = await _inventoryService.ValidateProjectAccessFromToken(ProjectKey, token);
			if (!allowed)
				return Forbid();

			var invList = await _inventoryService.GetAllInventoryByProjectAsync(ProjectKey);
			if (invList is null)
				return NotFound();

			var response = _mapper.Map<List<Res_Inventory>>(invList);
			return Ok(response);
		}

		[HttpGet(ApiRoutes.Inventory.GetInventory)]
		public async Task<IActionResult> GetInventory([FromRoute] Guid UniqueId)
		{
			var inv = await _inventoryService.GetInventoryByUniqueIdAsync(UniqueId);
			if (inv is null)
				return NotFound();

			var response = _mapper.Map<Res_Inventory>(inv);
			return Ok(response);
		}

		[HttpPost(ApiRoutes.Inventory.CreateInventory)]
		public async Task<IActionResult> CreateInventory([FromBody] Req_CreateInventory request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new Res_Common
				{
					Success = false,
					Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
				});
			}

			var inv = _mapper.Map<Inventory>(request);
			await _inventoryService.CreateInventoryAsync(inv);

			var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
			var locationUri = baseUrl + "/" + ApiRoutes.Inventory.GetInventory.Replace("{UniqueId}", inv.UniqueId.ToString());

			var response = _mapper.Map<Res_Inventory>(inv);
			return Created(locationUri, response);
		}

		[HttpPut(ApiRoutes.Inventory.UpdateInventory)]
		public async Task<IActionResult> UpdateInventory([FromRoute] Guid UniqueId, [FromBody] Req_UpdateInventory request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new Res_Common
				{
					Success = false,
					Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
				});
			}

			var jsonInv = JsonConvert.SerializeObject(request);
			IDictionary<string, string> updateInvDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonInv);

			var inv = await _inventoryService.UpdateInventoryAsync(UniqueId, updateInvDict);

			if (inv != null)
			{
				var response = _mapper.Map<Res_Inventory>(inv);
				return Ok(response);
			}

			return NotFound();
		}

		[Authorize(Roles = "Super, Admin")]
		[HttpPut(ApiRoutes.Inventory.DeleteInventory)]
		public async Task<IActionResult> DeleteInventory([FromRoute] Guid UniqueId)
		{
			var deleted = await _inventoryService.DeleteInventoryAsync(UniqueId);

			if (deleted)
				return NoContent();

			return NotFound();
		}
	}
}