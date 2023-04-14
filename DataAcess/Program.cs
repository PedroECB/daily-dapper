// See https://aka.ms/new-console-template for more information
namespace DataAcess
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string connectionString = "Server=localhost;Port=5432;Database=balta; User ID=postgres;Password=123";
            //Microsoft.Data.SqlClient
            Console.WriteLine("Hello, world!");
            var teste = Guid.NewGuid();

            Console.WriteLine(teste);
        }
    }
}