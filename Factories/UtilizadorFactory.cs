using WebApplication2.Entities;

namespace WebApplication2.Factories;


    public static class UtilizadorFactory
    {
        public static Utilizador CriarNovo(string nome, string username, string password, int totalUtilizadoresExistentes)
        {
            var cargo = (totalUtilizadoresExistentes == 0) ? "Admin" : "Utilizador";

            return new Utilizador
            {
                Nome = nome,
                Username = username,
                Password = password,
                NHabitualHoras = 8,
                cargo = cargo
            };
        }
    }
