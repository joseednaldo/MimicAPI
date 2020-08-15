using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Controllers
{

    //Criando rotas por atributos
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _Banco;

        //ControllerBase usado para API, no entando posso usar tambem o Controller
        //Controller é mais pra construção de sites.

        //App
        public PalavrasController(MimicContext banco)
        {
            _Banco = banco;
        }

        //APP  
        //Ex: /api/palavras/data=2019-05-02
        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas(DateTime? data)
        {
            //O interesando é fazer uso do padrao "Repositoy" mas no momento, faremos dessa forma...pra estudo...

            var item = _Banco.Palavras.AsQueryable();
            if (data.HasValue)
            {
                item = item.Where(a => a.DtCriacao > data.Value || a.DtAtualizacao > data.Value);
            }
            
            return Ok(item);
        }

        //web     
        //ex: /api/palavras/1
        [Route("{id}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {

            var obj = _Banco.Palavras.Find(id);
            if (obj == null)
                return NotFound();

            return Ok(_Banco.Palavras.Find(id));
        }

        [Route("")]
        // ex: /api/palavras (post:id.nome,ativo,pontuacao,data)
        [HttpPost]
        public ActionResult Cadastrar([FromBody]Palavra palavra)
        {
            _Banco.Palavras.Add(palavra);
            _Banco.SaveChanges();
            //return Ok();

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        [Route("{id}")]
        //ex: /api/palavras/1 (put:id.nome,ativo,pontuacao,data)
        [HttpPut]
        public ActionResult Atualizar(int id, [FromBody]Palavra palavra)
        {

            var obj = _Banco.Palavras.AsNoTracking().FirstOrDefault(p=>p.Id == id);
            if (obj == null)
                return NotFound();


            palavra.Id = id;
            _Banco.Palavras.Update(palavra);
            _Banco.SaveChanges();
            return Ok();
        }

        [Route("{id}")]
        // ex: /api/palavras/1
        [HttpDelete]
        public ActionResult Deletar(int id)
        {
            var _palavra = _Banco.Palavras.Find(id);
            if (_palavra == null)
                return NotFound();

            _palavra.Ativo = false;
            _Banco.Palavras.Update(_palavra);
            _Banco.SaveChanges();

            return NoContent(); //Ok();

        }
    }
}
