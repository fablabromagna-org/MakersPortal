using AutoMapper;
using MakersPortal.Core.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<JsonWebKey, JwkDto>();
            CreateMap<Microsoft.Azure.KeyVault.WebKey.JsonWebKey, JwkDto>();
        }
    }
}