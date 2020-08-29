using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
namespace MimicAPI.V1.Controllers
{

    [ApiController]
    // [Route("api/palavras")]  //Criando rotas por atributos
    [Route("api/v{verion:apiVersion}/[controller]")] // é o mesmo que => [Route("api/palavras")]  //Criando rotas por atributos
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]//fala que o controlador tem as 2 versões.

    // [ApiVersion("1.0", Deprecated = true)]  indica que a versao 1 esta absoleta.
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        //ControllerBase usado para API, no entando posso usar tambem o Controller
        //Controller é mais pra construção de sites.

        //App
        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        /// <summary>
        /// Operação que pega do banco todas as palavras existente
        /// </summary>
        /// <param name="query">Filtros de pesquisas</param>
        /// <returns>Listagem de palavras</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
       
        //APP  
        //Ex: /api/palavras/data=2019-05-02
        // [Route("")]
        [HttpGet("", Name = "ObterTodas")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query)
        {

            var item = _repository.ObterPalavras(query);

            if (item.Results.Count == 0)
                return NotFound();

            PaginationList<PalavraDTO> lista = CriarLinksListPalavraDTO(query, item);

            return Ok(lista);
        }


        /// <summary>
        /// Operação que pega uma única palavra da base de dados
        /// </summary>
        /// <param name="id">Identificador da palavra</param>
        /// <returns>Objeto de palavras</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        //web - ex: /api/palavras/1
        //[Route("{id}")]  -- é funcioandl mas no momento não precisamos mais da rota, porque agora vamos usar o templapte da rota dento do HttpGet , conforme abaixo.
        [HttpGet("{id}", Name = "ObterPalavra")]
        public ActionResult Obter(int id)
        {

            var obj = _repository.Obter(id);
            if (obj == null)
                return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);
            //palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(
                        new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET")
                );

            palavraDTO.Links.Add(
                        new LinkDTO("update", Url.Link("AtualizarPalavra", new { id = palavraDTO.Id }), "PUT")
                );

            palavraDTO.Links.Add(
                        new LinkDTO("DELETE", Url.Link("ExcluirPalavra", new { id = palavraDTO.Id }), "DELETE")
                );

            return Ok(palavraDTO);
        }


        /// <summary>
        /// Operação que realizada o cadastro da plavra 
        /// </summary>
        /// <param name="palavra">Um objeto palavra</param>
        /// <returns>Um objeto palavra com seu identificador</returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        // ex: /api/palavras (post:id.nome,ativo,pontuacao,data)
        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {

            //Validando  contexto geral da url
            if (palavra == null)
                return BadRequest();

            //Validando os dados do objeto
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            palavra.DtCriacao = DateTime.Now;
            palavra.Ativo = true;

            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                        new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET")
                );


            return Created($"/api/palavras/{palavra.Id}", palavraDTO);

        }


        /// <summary>
        /// Operação que atualiza no banco um palavra especifica
        /// </summary>
        /// <param name="id">Identificador da palavra a ser alterada</param>
        /// <param name="palavra">Objeto palavra comos dados de alteração</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        //[Route("{id}")]    -- é funcioandl mas no momento não precisamos mais da rota, porque agora vamos usar o templapte da rota dento do HttpGet , conforme abaixo.
        //ex: /api/palavras/1 (put:id.nome,ativo,pontuacao,data)
        [HttpPut("{id}", Name = "AtualizarPalavra")]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {

            var obj = _repository.Obter(id);
            if (obj == null)
                return NotFound();

            palavra.Id = id;
            palavra.Ativo = obj.Ativo;
            palavra.DtCriacao = obj.DtCriacao;
            palavra.DtAtualizacao = DateTime.Now;

            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "PUT")
            );

            return Ok();
        }


        /// <summary>
        /// Operação que desativa uma palavra no banco de dados
        /// </summary>
        /// <param name="id">Identificador da palavra</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        //[Route("{id}")]  é funcional mas no momento não precisamos mais da rota, porque agora vamos usar o templapte da rota dento do HttpGet , conforme abaixo.
        // ex: /api/palavras/1
        [MapToApiVersion("1.1")]  // ESSE METODO SÓ FUNCIONA NA VERSAO 1.1
        [HttpDelete("{id}", Name = "ExcluirPalavra")]
        public ActionResult Deletar(int id)
        {
            var _palavra = _repository.Obter(id);
            if (_palavra == null)
                return NotFound();

            _repository.Deletar(id);
            return NoContent(); //Ok();

        }
    
        private PaginationList<PalavraDTO> CriarLinksListPalavraDTO(PalavraUrlQuery query, PaginationList<Palavra> item)
        {
            var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(item);

            foreach (var palavra in lista.Results)
            {
                //palavra.Links = new List<LinkDTO>();
                palavra.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavra.Id }), "GET"));
            }

            lista.Links.Add(new LinkDTO("self", Url.Link("ObterTodas", query), "GET"));

            if (item.Paginacao != null)
            {

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));

                if (query.PagNumero + 1 <= item.Paginacao.TotalPaginas)
                {
                    var queryString = new PalavraUrlQuery()
                    {
                        PagNumero = query.PagNumero + 1,
                        PagRegistro = query.PagRegistro,
                        Data = query.Data
                    };
                    lista.Links.Add(new LinkDTO("next", Url.Link("ObterTodas", queryString), "GET"));
                }

                if (query.PagNumero - 1 > 0)
                {
                    var queryString = new PalavraUrlQuery()
                    {
                        PagNumero = query.PagNumero - 1,
                        PagRegistro = query.PagRegistro,
                        Data = query.Data
                    };

                    lista.Links.Add(new LinkDTO("prev", Url.Link("ObterTodas", queryString), "GET"));
                }

            }

            return lista;
        }
    }
}
