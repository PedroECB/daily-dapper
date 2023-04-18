using System;
using Npgsql;
using Dapper;
using Models;

namespace DataAcess
{
    public class Dapper
    {
        //dotnet add package dapper --version 2.0.123
        public static void Execute()
        {
            Console.Clear();
            try
            {
                const string connectionString = Constants.STRING_CONNECTION;

                List<Category> categories = new List<Category>();
                using (var db = new NpgsqlConnection(connectionString))
                {
                    string sqlString = "SELECT * FROM category";
                    categories = db.Query<Category>(sqlString).AsList();
                }

                Console.WriteLine("------------- Listando Categorias ---------------");
                foreach (Category category in categories)
                {
                    Console.WriteLine($"Categoria: {category.Description} - {category.Code}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao realizar consulta com dapper: {ex.Message}");
            }
        }
    }
}