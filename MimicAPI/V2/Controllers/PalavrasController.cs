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
        //APP  
        //Ex: /api/palavras/data=2019-05-02
        // [Route("")]
        [HttpGet("", Name = "ObterTodas")]
        public string ObterTodas()
        {
            return "Versao 2.0";
        }
    }
}
