using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using backend.Contracts;
using Microsoft.Data.SqlClient;

namespace backend.Database;

public class BookRepository
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:AzureSDatabase");

    private static int CreateBook(Book book, SqlConnection connection, SqlTransaction transaction)
    {
        const string queryText =
            @" INSERT INTO dbo.Books(Title, Author, Cover) 
                output INSERTED.Id
                Values(@title, @author, @cover)";

        var titleParameter = new SqlParameter("@title", SqlDbType.NVarChar);
        titleParameter.Value = book.Title;
        var authorParameter = new SqlParameter("@author", SqlDbType.NVarChar);
        authorParameter.Value = book.Author;
        var coverParameter = new SqlParameter("@cover", SqlDbType.Image)
        {
            Value = book.Cover,
            Size = book.Cover.Length
        };

        var command = new SqlCommand(queryText, connection);
        command.Transaction = transaction;
        command.Parameters.AddRange(new[] { titleParameter, authorParameter, coverParameter });

        return (int)command.ExecuteScalar();
    }


    private static async Task CreateTags(List<string> tagNames, SqlConnection connection, SqlTransaction transaction)
    {
        const string queryText =
            @" IF NOT EXISTS(SELECT NULL FROM dbo.Tags WHERE Name = @tagName) 
               BEGIN
                   INSERT INTO dbo.Tags(Name)
                   Values(@tagName)
               END";

        var tagNameParameter = new SqlParameter("@tagName", SqlDbType.NVarChar);
        var command = new SqlCommand(queryText, connection);
        command.Transaction = transaction;

        foreach (var tagName in tagNames)
        {
            tagNameParameter.Value = tagName;
            command.Parameters.Add(tagNameParameter);

            await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();
        }
    }


    private static async Task<List<int>> GetTagsIds(IEnumerable<string> tagNames, SqlConnection connection,
        SqlTransaction transaction)
    {
        const string queryText =
            @" SELECT t.id FROM dbo.Tags t
                   INNER JOIN String_split(@tags, ',') ts on ts.value = t.name";

        var tagsParameter = new SqlParameter("@tags", SqlDbType.NVarChar);
        tagsParameter.Value = string.Join(",", tagNames);

        var command = new SqlCommand(queryText, connection);
        command.Transaction = transaction;
        command.Parameters.Add(tagsParameter);

        var reader = await command.ExecuteReaderAsync();
        var ids = new List<int>();
        while (reader.Read())
        {
            ids.Add(reader.GetInt32(0));
        }
        
        reader.Close();

        return ids;
    }

    public async Task<int> CreateBookWithTags(Book book, List<string> tagNames)
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open();
        var transaction = connection.BeginTransaction();

        try
        {
            var bookId = CreateBook(book, connection, transaction);
            await CreateTags(tagNames, connection, transaction);
            var tagsIds = await GetTagsIds(tagNames, connection, transaction);

            const string queryText =
                @"INSERT INTO dbo.BooksTags(BookId, TagId)
              VALUES (@bookId, @tagId)";

            var bookIdParameter = new SqlParameter("@bookId", SqlDbType.Int);
            bookIdParameter.Value = bookId;
            var tagIdParameter = new SqlParameter("@tagId", SqlDbType.Int);
            var command = new SqlCommand(queryText, connection);
            command.Transaction = transaction;

            foreach (var tagId in tagsIds)
            {
                tagIdParameter.Value = tagId;
                command.Parameters.Add(bookIdParameter);
                command.Parameters.Add(tagIdParameter);

                await command.ExecuteNonQueryAsync();

                command.Parameters.Clear();
            }

            transaction.Commit();

            return bookId;
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}