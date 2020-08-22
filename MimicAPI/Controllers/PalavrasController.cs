using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using MimicAPI.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace MimicAPI.Controllers
{

    //Criando rotas por atributos
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository  _repository;
        private readonly IMapper _mapper;

        //ControllerBase usado para API, no entando posso usar tambem o Controller
        //Controller é mais pra construção de sites.

        //App
        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //APP  
        //Ex: /api/palavras/data=2019-05-02
       // [Route("")]
        [HttpGet("",Name = "ObterTodas")]
        public ActionResult ObterTodas([FromQuery]PalavraUrlQuery query )
        {

            var item = _repository.ObterPalavras(query);

            if (item.Results.Count == 0)
                return NotFound();

            PaginationList<PalavraDTO> lista = CriarLinksListPalavraDTO(query, item);

            return Ok(lista);
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


        #region Method Obter
        //web     
        //ex: /api/palavras/1

        //[Route("{id}")]  -- é funcioandl mas no momento não precisamos mais da rota, porque agora vamos usar o templapte da rota dento do HttpGet , conforme abaixo.
        [HttpGet("{id}", Name ="ObterPalavra")]
        public ActionResult Obter(int id)
        {

            var obj = _repository.Obter(id);
            if (obj == null)
                return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);
            //palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(
                        new LinkDTO("self",Url.Link("ObterPalavra",new {id = palavraDTO.Id}),"GET")
                );

            palavraDTO.Links.Add(
                        new LinkDTO("update",Url.Link("AtualizarPalavra", new { id = palavraDTO.Id }), "PUT")
                );

            palavraDTO.Links.Add(
                        new LinkDTO("DELETE", Url.Link("ExcluirPalavra", new { id = palavraDTO.Id }), "DELETE")
                );

            return Ok(palavraDTO);
        }
        #endregion

        [Route("")]
        // ex: /api/palavras (post:id.nome,ativo,pontuacao,data)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {

            //Validando  contexto geral da url
            if (palavra ==null)
                return BadRequest();

            //Validando os dados do objeto
            if (!ModelState.IsValid)
                return  UnprocessableEntity(ModelState);

            palavra.DtCriacao = DateTime.Now;
            palavra.Ativo = true;

            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            palavraDTO.Links.Add(
                        new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET")
                );


            return Created($"/api/palavras/{palavra.Id}", palavraDTO);

        }

        #region Atualizar
        //[Route("{id}")]    -- é funcioandl mas no momento não precisamos mais da rota, porque agora vamos usar o templapte da rota dento do HttpGet , conforme abaixo.
        //ex: /api/palavras/1 (put:id.nome,ativo,pontuacao,data)
        [HttpPut("{id}",Name ="AtualizarPalavra")]
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
        #endregion

        #region Delete
        //[Route("{id}")]  é funcional mas no momento não precisamos mais da rota, porque agora vamos usar o templapte da rota dento do HttpGet , conforme abaixo.
        // ex: /api/palavras/1
        [HttpDelete("{id}",Name ="ExcluirPalavra")]
        public ActionResult Deletar(int id)
        {
            var _palavra = _repository.Obter(id);
            if (_palavra == null)
                return NotFound();

            _repository.Deletar(id);
            return NoContent(); //Ok();

        }
        #endregion
    }
}
