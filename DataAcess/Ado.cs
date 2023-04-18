// See https://aka.ms/new-console-template for more information
using Models;
using Npgsql;

namespace DataAcess
{
    public class Ado
    {
        //dotnet add package Microsoft.Data.SqlClient/ Microsoft.Npgsql
        public static void Execute()
        {
            const string connectionString = Constants.STRING_CONNECTION;
            Console.Clear();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine("Conectado");
                    connection.Open();

                    var command = new NpgsqlCommand();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "SELECT * FROM author";

                    var reader = command.ExecuteReader();

                    Console.WriteLine($"Registros encontrados: {reader.Rows}");
                    Console.WriteLine("ID | NOME | Título | URL_IMAGE");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader.GetString(0)} | {reader.GetString(1)} | {reader.GetString(2)} | {(reader.IsDBNull(3) ? "NULL" : reader.GetString(3))}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}