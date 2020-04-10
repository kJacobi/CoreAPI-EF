using CoreAPI_EF.Data;
using CoreAPI_EF.Domain;
using CoreAPI_EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dataContext;
        private readonly ITokenHelperService _tokenHelperService;

        public ProjectService(DataContext dataContext, ITokenHelperService tokenHelperService)
        {
            _dataContext = dataContext;
            _tokenHelperService = tokenHelperService;
        }

        /*******************************************************
         * CreateProjectAsync
         * ****************************************************/
        public async Task<bool> CreateProjectAsync(Project projToCreate)
        {
            await _dataContext.Projects.AddAsync(projToCreate);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        /*******************************************************
         * GetProjectByUniqueIdAsync
         * ****************************************************/
        public async Task<Project> GetProjectByUniqueIdAsync(Guid UniqueId)
        {
            return await _dataContext.Projects.SingleOrDefaultAsync(x => x.UniqueId == UniqueId);
        }

        /*******************************************************
         * GetAllProjectsFromTokenAsync
         * ****************************************************/
        public async Task<IList<Project>> GetAllProjectsFromTokenAsync(string token)
        {
            var validatedToken = _tokenHelperService.GetPrincipalFromToken(token);
            try
            {
                if (validatedToken is null)
                    return null;

                var user = await _tokenHelperService.GetUserFromToken(token);

                IList<Project> ProjectList = (from p in _dataContext.Projects
                                              join up in _dataContext.UserProjects
                                              on p.ProjectKey equals up.ProjectKey
                                              where up.UserName == user.UserName
                                              select p).ToList();
                return ProjectList;
            }
            catch
            {
                return null;
            }
        }
    }
}
