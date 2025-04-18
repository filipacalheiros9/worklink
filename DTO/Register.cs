namespace WebApplication2.DTO;

public class Register
{
    public class RegisterModel
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string cargo { get; set; } = "Utilizador";
    }

}