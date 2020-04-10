using AutoMapper;
using CoreAPI_EF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI_EF.Profiles
{
    public class DomainToDomainProfiles : Profile
    {
        public DomainToDomainProfiles()
        {
            CreateMap<Transaction, Transaction>();

            CreateMap<Transaction, TransactionLog>()
                .ForMember(tlog => tlog.Id, t => t.Ignore())
                .ForMember(tlog => tlog.TransactionId, t => t.MapFrom(tran => tran.Id))
                .ForMember(tlog => tlog.TransactionUniqueId, t => t.MapFrom(tran => tran.UniqueId));
        }
    }
}
