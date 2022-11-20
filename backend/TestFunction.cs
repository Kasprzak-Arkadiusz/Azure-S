using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;

namespace backend;

public class BookVm
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public byte[] Cover { get; set; }
}

public class TestFunction
{
    [FunctionName("books")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET")]
        HttpRequest request, ILogger log)
    {
        try
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            const int minTags = 2;
            var tags = request.Query["tags"].ToString();
            var response = new List<BookVm>();

            var connection = new SqlConnection(connectionString);
            const string queryText = @"IF (LEN(@tags) < 1)
            BEGIN
            SELECT * FROM dbo.Books;
            END
                ELSE
            BEGIN
                select b.*
                from dbo.Tags t
            inner join BooksTags bt on bt.TagId = t.Id
            inner join Books b on b.Id = bt.BookId
            inner join String_split(@tags, ',') ts on ts.value = t.Name
            group by b.Id, b.Title, b.Author, b.Cover
                having count(*) >= @minTags
            order by b.Title, b.Author;
            END";
            connection.Open();

            var tagParameter = new SqlParameter("@tags", SqlDbType.NVarChar);
            tagParameter.Value = tags;
            var minTagsParameter = new SqlParameter("@minTags", SqlDbType.Int);
            minTagsParameter.Value = minTags;

            var command = new SqlCommand(queryText, connection);
            command.Parameters.Add(tagParameter);
            command.Parameters.Add(minTagsParameter);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                response.Add(new BookVm
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Author = reader.GetString(2),
                    Cover = reader.GetSqlBytes(3).Buffer
                });

                log.LogInformation("Response: {Response}", response.ToString());
            }

            return new OkObjectResult(response);
        }
        catch (DbException e)
        {
            log.LogError("Error with database: {Message}", e.Message);
            return new InternalServerErrorResult();
        }
    }
}