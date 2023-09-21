using System;
using System.Data;
using Npgsql;
using Dapper;
using Models;
using NpgsqlTypes;
using Npgsql.Schema;
using System.Reflection.Metadata;
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
                    // Category category1 = new Category();
                    // category1.Description = "Microsoft Azure";
                    // category1.Code = "T23Q23A";

                    // int rowsAffecteds = CreateCategory(dbConnection, category1);
                    // Console.WriteLine($"AFFECTED ROWS: {rowsAffecteds}");

                    //QUERY SELECT
                    // string sqlSelectString = "SELECT * FROM category";
                    // ListCategories(dbConnection, sqlSelectString);

                    //CALL PROCEDURE
                    // ExecuteProcedureInsertStudent(dbConnection, "João Frederico");

                    //CALL FUNCTION OUTPUT RESULTSET
                    // ExecuteReadFunction(dbConnection);

                    //CALL EXECUTESCALAR
                    // ExecuteScalar(dbConnection);

                    //SELECT FROM VIEW
                    // ReadView(dbConnection);

                    //QUERY MULTIPLE
                    ExecuteQueryMultiple(dbConnection);
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

            // //Execução alternativa com executeManyInserts
            // int rowsAffectedMany = dbConnection.Execute(insertSql, new[]{
            // new
            // {
            //     description = category.Description,
            //     code = category.Code
            // },
            // new
            // {
            //     description = category.Description,
            //     code = category.Code
            // },
            // new
            // {
            //     description = category.Description,
            //     code = category.Code
            // }
            // });

            return rowsAffected;
        }

        public static int ExecuteProcedureInsertStudent(NpgsqlConnection dbConnection, string? studentName = null)
        {
            Console.WriteLine("------------------- INSERINDO NOVO ALUNO --------------------------");
            string sqlString = "CALL pr_insert_student(:p_name, :p_qtd_rows)";
            var parameters = new { p_name = studentName };
            int affectedRows = dbConnection.Execute(sqlString, parameters, commandType: CommandType.Text);
            return affectedRows;
        }

        public static int ExecuteProcedureInsertStudentCommand(NpgsqlConnection dbConnection, string? studentName = null)
        {


            //Get returned values from stored procedure npgsql using command
            dbConnection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("CALL pr_insert_student(:p_name)", dbConnection);
            int affectedRows;
            var p_name = new NpgsqlParameter(":p_name", studentName);
            var p_qtd_rows = new NpgsqlParameter(":p_qtd_rows", NpgsqlDbType.Integer, 200, "p_qtd_rows", ParameterDirection.Output, false, 1, 1, DataRowVersion.Default, null);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(p_name);
            cmd.Parameters.Add(p_qtd_rows);
            var reader = cmd.ExecuteReader();
            // int affectedRows = cmd.Parameters[1].Value != null ? int.Parse(cmd.Parameters[1].Value.ToString()) : 0;
            int.TryParse(cmd.Parameters[1]?.Value.ToString(), out affectedRows);
            dbConnection.Close();

            return affectedRows;
        }

        public static void ExecuteReadFunction(NpgsqlConnection dbConnection)
        {
            // string sqlSelectProc = "BEGIN; SELECT fn_list_student('cursor')";
            string sqlSelectProc = "BEGIN; SELECT fn_list_student('cursor'); FETCH ALL IN \"cursor\"; COMMIT;";
            var parameters = new { };
            var listStudents = dbConnection.Query(sqlSelectProc, parameters);
        }

        /// <summary>
        /// Método utilizado para retornar a primeira coluna do registro retornado
        /// da base.
        /// </summary>
        /// <param name="dbConnection">Objeto de conexão aberta.</param>
        /// <returns>
        /// (dynamic) Valor da primeira célula retornada da query
        /// </returns>
        public static dynamic ExecuteScalar(NpgsqlConnection dbConnection)
        {
            Category student = new Category() { Code = "PTK231", Description = "My Category 4" };
            string sqlString = "INSERT INTO category VALUES (default, :description, :code) RETURNING category.\"Id\";";
            var returnedValue = dbConnection.ExecuteScalar(sqlString, student);
            return returnedValue;
        }

        /// <summary>
        /// Método utilizado para retornar listagem de uma view
        /// </summary>
        /// <param name="dbConnection">Objeto de conexão aberta.</param>
        /// <returns>
        /// List<dynamic> Lista de registros retornados na view
        /// </returns>
        public static IEnumerable<dynamic> ReadView(NpgsqlConnection dbConnection)
        {
            string sqlString = "SELECT * FROM vw_student_course";
            var listFromView = dbConnection.Query(sqlString);

            // foreach (var item in listFromView)
            //     Console.WriteLine($" ID: {item.Id} NAME: {item.Name} COURSE: {item.description}");

            return listFromView;
        }

        /// <summary>
        /// Método utilizado para retornar listagem de diferentes querys em conjunto
        /// </summary>
        /// <param name="dbConnection">Objeto de conexão aberta.</param>
        /// <returns>
        /// Void
        /// </returns>
        public static void ExecuteQueryMultiple(NpgsqlConnection dbConnection)
        {
            string stringSqlQuery = "SELECT * FROM author; SELECT * FROM course";

            using (var multi = dbConnection.QueryMultiple(stringSqlQuery))
            {
                var authors = multi.Read();
                var courses = multi.Read();
            }
        }


    }
}