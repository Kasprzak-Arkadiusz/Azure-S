using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using backend.Contracts;
using backend.Database;
using backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace backend;

public static class CreateBookFunction
{
    [FunctionName("createBook")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "POST")]
        HttpRequest req, ILogger log)
    {
        try
        {
            var requestAsString = await req.ReadAsStringAsync();
            var requestBody = JsonConvert.DeserializeObject<Book>(requestAsString);

            var tagImageDetails = await CallCognitiveServiceAsync(requestBody.Cover);

            var bookRepository = new BookRepository();
            var bookId =
                await bookRepository.CreateBookWithTags(requestBody, tagImageDetails.Tags.Select(t => t.Name).ToList());

            return new ObjectResult("Pomyślnie dodano książkę")
            {
                StatusCode = StatusCodes.Status201Created,
                Value = bookId
            };
        }
        catch (CognitiveServiceException e)
        {
            return new ObjectResult(e.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        catch (Exception ex)
        {
            return new ObjectResult(ex.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }

    private static async Task<TagImageResponse> CallCognitiveServiceAsync(byte[] cover)
    {
        var uri = Environment.GetEnvironmentVariable("CognitiveServiceUri");
        var subscriptionKey = Environment.GetEnvironmentVariable("CognitiveServiceKey");
        
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        
        using var content = new ByteArrayContent(cover);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var response = await client.PostAsync(uri, content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new CognitiveServiceException("Wystąpił problem z Cognitive Service");
        }
        
        var responseAsString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TagImageResponse>(responseAsString);
    }
}