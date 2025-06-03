namespace WebApplication2.DTO
{
    public class ProjetosEquipasViewModel
    {
        public int    IdProjeto   { get; set; }
        public string NomeProjeto { get; set; } = string.Empty;
        public string NomeCliente { get; set; } = string.Empty;
        public decimal IdUtilizador { get; set; }
        public int?   EquipaId    { get; set; }
        // public List<ProjetoTarefaDto> Tarefas { get; set; }
    }
}