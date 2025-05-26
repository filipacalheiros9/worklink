using WebApplication2.Entities;

namespace WebApplication2.Factories;

public static class UtilizadorFactory
{
    public static Utilizador CriarNovo(string nome, string username, string password, int totalUtilizadoresExistentes)
    {
        var cargoId = (totalUtilizadoresExistentes == 0) ? 1 : 2;

        return new Utilizador
        {
            Nome = nome,
            Username = username,
            Password = password,
            NHabitualHoras = 8,
            CargoId = cargoId
        };
    }
}