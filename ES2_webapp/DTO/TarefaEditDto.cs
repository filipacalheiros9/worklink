﻿namespace WebApplication2.DTO;


public class TarefaEditDto
{
    public string NomeTarefa { get; set; }
    public DateOnly? DtFim { get; set; }
    public string? HrFim { get; set; }
    public decimal PrecoHora { get; set; }

    // Opcional: só se quiseres alterar o responsável
    public int? IdUtilizadorResponsavel { get; set; }
}
