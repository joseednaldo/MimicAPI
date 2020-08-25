using AutoMapper;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {

            /*
               AutoMapper = Objetivo de converte de um objeto pra outro.
             */
            //Convertendo  "palavra" para o objeto "palavraDTO"
            CreateMap<Palavra, PalavraDTO>();
            CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();
        }
    }
}
