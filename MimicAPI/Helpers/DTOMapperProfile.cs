using AutoMapper;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile: Profile
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
