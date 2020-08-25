using System;
using System.ComponentModel.DataAnnotations;

namespace MimicAPI.V1.Models
{
    public class Palavra
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public int Pontuacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DtCriacao { get; set; }
        public DateTime? DtAtualizacao { get; set; }

    }
}
