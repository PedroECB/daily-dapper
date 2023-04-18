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

                using (var dbConnection = new NpgsqlConnection(connectionString))
                {
                    Category category1 = new Category();
                    category1.Description = "Microsoft Azure";
                    category1.Code = "T23Q23A";

                    int rowsAffecteds = CreateCategory(dbConnection, category1);
                    Console.WriteLine($"AFFECTED ROWS: {rowsAffecteds}");

                    //QUERY SELECT
                    string sqlSelectString = "SELECT * FROM category";
                    ListCategories(dbConnection, sqlSelectString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao realizar consulta com dapper: {ex.Message}");
            }
        }

        /// <summary>
        /// Esse método é utilizadoa para listar categorias cadastradas na base de dados
        /// </summary>
        /// <param name="dbConnection">Objeto de conexão aberta.</param>
        /// <param name="sqlString">instrução SQL de inserção.</param>
        public static void ListCategories(NpgsqlConnection dbConnection, string sqlString)
        {
            List<Category> categories = new List<Category>();
            categories = dbConnection.Query<Category>(sqlString).AsList();

            Console.WriteLine("------------- Listando Categorias ---------------");
            foreach (Category category in categories)
            {
                Console.WriteLine($"Categoria: {category.Description} - {category.Code}");
            }
        }

        /// <summary>
        /// Esse método é utilizado para persistir os registros de categoria
        /// na base de dados.
        /// </summary>
        /// <param name="category">Entidade de categoria a ser inserida na base.</param>
        /// <param name="dbConnection">Objeto de conexão aberta.</param>
        /// <returns>
        /// (int) Um inteiro com a quantidade de linhas afetadas
        /// </returns>
        public static int CreateCategory(NpgsqlConnection dbConnection, Category category)
        {
            string insertSql = "INSERT INTO category VALUES (DEFAULT, :description, :code)";

            if (string.IsNullOrEmpty(category.Description) || string.IsNullOrEmpty(category.Code))
                throw new ApplicationException("Faltaram argumentos");

            int rowsAffected = dbConnection.Execute(insertSql, new
            {
                description = category.Description,
                code = category.Code
            });

            return rowsAffected;
        }
    }
}