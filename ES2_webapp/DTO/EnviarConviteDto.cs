namespace WebApplication2.DTO;

public class EnviarConviteDto
{
    public string UsernameDestinatario { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
    public int IdEquipa { get; set; }
}