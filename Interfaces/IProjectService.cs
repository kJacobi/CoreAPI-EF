using CoreAPI_EF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Interfaces
{
    public interface IProjectService
    {
        Task<bool> CreateProjectAsync(Project projToCreate);
        Task<IList<Project>> GetAllProjectsFromTokenAsync(string token);
        Task<Project> GetProjectByUniqueIdAsync(Guid UniqueId);

    }
}
