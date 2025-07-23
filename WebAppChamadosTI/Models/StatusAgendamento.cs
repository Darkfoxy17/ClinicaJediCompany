using WebAppChamadosTI.Models;
using System.ComponentModel.DataAnnotations;


public class StatusAgendamento
{
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; }

    public ICollection<Agendamento> Agendamentos { get; set; }
}
