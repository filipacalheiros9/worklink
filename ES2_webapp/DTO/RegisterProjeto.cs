namespace WebApplication2.DTO;

public class RegisterProjeto
{
    public class ProjetoCreate
    {
        public string NomeProjeto { get; set; }
        public string NomeCliente { get; set; }
        public decimal? PrecoHora { get; set; }
    }
}