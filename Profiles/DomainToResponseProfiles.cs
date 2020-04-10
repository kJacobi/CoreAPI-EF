using AutoMapper;
using CoreAPI_EF.Contracts.V1.Responses;
using CoreAPI_EF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Profiles
{
    public class DomainToResponseProfiles : Profile
    {
        public DomainToResponseProfiles()
        {
            CreateMap<Inventory, Res_Inventory>();

            CreateMap<Project, Res_Project>();

            CreateMap<Transaction, Res_GetTransaction>();
            CreateMap<Transaction, Res_Transaction>();
        }
    }
}
