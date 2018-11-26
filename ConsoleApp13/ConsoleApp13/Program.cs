using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace ConsoleApp13
{
    class Program
    {
        static void Main(string[] args)
        {
            var title = "Post";
            var text = "Text";
            var author = "Author";

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = "";
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var command = new SqlCommand();
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = $"Insert into posts values (@title, @text, @author)";

                        var authorParameters = new SqlParameter();
                        authorParameters.DbType = DbType.String;
                        authorParameters.ParameterName = "@author";
                        authorParameters.Value = author;
                        authorParameters.IsNullable = false;


                        command.Parameters.AddRange(new SqlParameter[]
                        {
                        new SqlParameter("@title",title),
                        new SqlParameter("@text",text),
                        authorParameters
                        }
                    );
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch(SqlException exception)
                    {
                        Console.WriteLine(exception.Message);
                        transaction.Rollback();
                    }
                    catch (DbException exception)
                    {
                        Console.WriteLine(exception.Message);
                        transaction.Rollback();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                        transaction.Rollback();
                    }
                }
               ExecuteInTransaction(
               connection,
               new SqlCommand(),
               new SqlCommand(),
               new SqlCommand());
            }         
        }
        public static bool ExecuteInTransaction(DbConnection connection, params DbCommand[] commands)
        {
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var command in commands)
                    {
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (SqlException exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                    return false;
                }
                catch (DbException exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                    return false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
