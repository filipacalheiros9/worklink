using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Tarefa
{
    public decimal IdTarefa { get; set; }

    public string NomeTarefa { get; set; } = null!;

    public DateOnly? DtInicio { get; set; }

    public decimal? HrInicio { get; set; }

    public DateOnly? DtFim { get; set; }

    public decimal? HrFim { get; set; }

    public decimal IdProjeto { get; set; }

    public virtual Projeto IdProjetoNavigation { get; set; } = null!;
}
