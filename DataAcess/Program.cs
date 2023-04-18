// See https://aka.ms/new-console-template for more information
using Npgsql;

namespace DataAcess
{
    public class Program
    {
        //Microsoft.Data.SqlClient/ Microsoft.Npgsql
        public static void Main(string[] args)
        {
            // Ado.Execute();
            Dapper.Execute();
        }
    }
}