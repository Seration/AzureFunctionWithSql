using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace SqlExampleTrigger
{
    public static class InsertMemberFunction
    {
        [FunctionName("InsertMemberFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string name = req.Query["name"];
                string surname = req.Query["surname"];
                int age = Convert.ToInt32(req.Query["age"]);

                var str = Environment.GetEnvironmentVariable("sqldb_connection");

                using (SqlConnection con = new SqlConnection(str))
                {
                    con.Open();

                    string query = "INSERT INTO dbo.Member (Name, Surname, Age) " +
                                       "VALUES (@Name, @Surname, @Age) ";

                    SqlCommand com = new SqlCommand(query, con);

                    com.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = name;
                    com.Parameters.Add("@Surname", SqlDbType.VarChar, 50).Value = surname;
                    com.Parameters.Add("@Age", SqlDbType.Int).Value = age;

                    com.ExecuteNonQuery();

                    con.Close();
                }

                return new OkObjectResult(name + surname + age);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

        }
    }
}
