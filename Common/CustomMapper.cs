using AutoMapper;
using Microsoft.Extensions.Configuration;
using OnlineNote.Models;

namespace OnlineNote.Common
{
    public static class CustomMapper
    {

        public static IMapper MapperObject;

        public static void ConstructMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AccountEntity, Account>().ReverseMap();
                cfg.CreateMap<NoteEntity, Note>().ReverseMap();
            });

            MapperObject = configuration.CreateMapper();
        }
    }
}
