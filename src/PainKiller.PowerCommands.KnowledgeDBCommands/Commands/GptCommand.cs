using System.Text;

namespace PainKiller.PowerCommands.KnowledgeDBCommands.Commands;


[PowerCommandDesign( description: "Run openAI query, you need an open API registered user and credit",
                        useAsync: true,
                         secrets: "!kdb_gpt",
                         example: "gpt \"What is the capital of Sweden\"")]
public class GptCommand : CommandBase<PowerCommandsConfiguration>
{
    public GptCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        var apiKey = Configuration.Secret.DecryptSecret("##kdb_gpt##");
        var apiUrl = Configuration.OpenAiApiUrl;

        var prompt = Input.SingleQuote;
        var maxTokens = Configuration.OpenAiMaxTokens;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            
        var requestBody = $"{{ \"prompt\": \"{prompt}\", \"max_tokens\": {maxTokens} }}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            
        var response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            WriteSuccessLine(responseContent);
        }
        else
        {
            WriteFailureLine($"API request failed with status code: {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            WriteFailureLine($"Response content: {responseContent}");
        }
        return Ok();
    }
}