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

namespace CoreAPI_EF.Controllers.V1
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Super, Admin, Manager")]
	[ApiController]
	public class ProjectController : ControllerBase
	{
		private readonly IProjectService _projectService;
		private readonly IMapper _mapper;

		public ProjectController(IProjectService projectService, IMapper mapper)
		{
			_projectService = projectService;
			_mapper = mapper;
		}


		[HttpPost(ApiRoutes.Project.CreateProject)]
		public async Task<IActionResult> CreateProject([FromBody] Req_CreateProject request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new Res_Common
				{
					Success = false,
					Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
				});
			}

			var proj = _mapper.Map<Project>(request);
			await _projectService.CreateProjectAsync(proj);

			var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
			var locationUri = baseUrl + "/" + ApiRoutes.Project.GetProject.Replace("{ProjectKey}", proj.ProjectKey.ToString());

			var response = _mapper.Map<Res_Project>(proj);
			return Created(locationUri, response);
		}


		[HttpGet(ApiRoutes.Project.GetAllProjects)]
		public async Task<IActionResult> GetAllProjects()
		{
			var auth = Request.Headers["Authorization"];
			var token = auth.First().Remove(0, "Bearer ".Length).Trim();

			var projList = await _projectService.GetAllProjectsFromTokenAsync(token);
			if (projList is null)
				return NotFound();

			var response = _mapper.Map<List<Res_Project>>(projList);
			return Ok(response);
		}


		[HttpGet(ApiRoutes.Project.GetProject)]
		public async Task<IActionResult> GetProject([FromRoute] Guid UniqueId)
		{
			var proj = await _projectService.GetProjectByUniqueIdAsync(UniqueId);
			if (proj is null)
				return NotFound();

			var response = _mapper.Map<Res_Project>(proj);
			return Ok(response);
		}
	}
}