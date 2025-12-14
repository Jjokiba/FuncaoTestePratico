using System.ComponentModel.DataAnnotations;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Beneficiario
    /// </summary>
    public class BeneficiarioModel
    {
        public long Id { get; set; }

        [Required]
        public long IdCliente { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Nome { get; set; }

        /// <summary>
        /// CPF
        /// </summary>
        [Required]
        [MaxLength(14)] 
        [ValidarCPF]
        public string CPF { get; set; }

    }    
}