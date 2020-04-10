using AutoMapper;
using CoreAPI_EF.Contracts.V1.Requests;
using CoreAPI_EF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Profiles
{
    public class RequestToDomainProfiles : Profile
    {
        public RequestToDomainProfiles()
        {
            CreateMap<Req_CreateInventory, Inventory>();
            CreateMap<Req_UpdateInventory, Inventory>();

            CreateMap<Req_CreateProject, Project>();

            CreateMap<Req_Transaction, Transaction>();
        }
    }
}
