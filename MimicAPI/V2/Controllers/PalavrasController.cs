using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.V2.Controllers
{
    [ApiController]
    // [Route("api/v{verion:apiVersion}/palavras")]  
    [Route("api/v{verion:apiVersion}/[controller]")]  // è o mesmo que =>[Route("api/v{verion:apiVersion}/palavras")]  
    [ApiVersion("2.0")]
    public class PalavrasController: ControllerBase
    {

        /// <summary>
        /// Operação que pega do banco todas as palavras existente
        /// </summary>
        /// <param name="query">Filtros de pesquisas</param>
        /// <returns>Listagem de palavras</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]

        [HttpGet("", Name = "ObterTodas")]
        public string ObterTodas()
        {
            return "Versao 2.0";
        }
    }
}
