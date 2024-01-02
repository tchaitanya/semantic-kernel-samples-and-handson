using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.Extensions.Azure;

/// <summary>
/// Load the settings from the default config file or from user secrets.
/// This will add the AI (LLM) or connector to the Kernel, which will process the requests.
/// </summary>
var connector = KernelSettings.LoadSettings();

#region prompt_engineering_semantic_function
// var kernelBuilder = Kernel.CreateBuilder()
//                     .AddAzureOpenAIChatCompletion(
//                         endpoint: connector.Endpoint,
//                         apiKey: connector.ApiKey,
//                         deploymentName: connector.DeploymentOrModelId
//                     )
//                     .Build();

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 1. Invoke the kernel with a prompt and display the result");
// Console.WriteLine(await kernel.InvokePromptAsync("What color is the sky?"));
// Console.WriteLine();

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 2. Invoke the kernel with a templated prompt and display the result");
// KernelArguments arguments = new() { { "topic", "sea" } };
// Console.WriteLine(await kernel.InvokePromptAsync("What color is the {{$topic}}?", arguments));
// Console.WriteLine();

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 3. Invoke the kernel with a templated prompt and stream the results to the display");
// await foreach (var update in kernel.InvokePromptStreamingAsync("What color is the {{$topic}}? Provide a detailed explanation.", arguments))
// {
//     Console.Write(update);
// }
// Console.WriteLine();

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 4. Invoke the kernel with a Handlebar prompt template and execution settings");
// // Create handlebars template for intent
// var getIntent = kernel.CreateFunctionFromPrompt(
//     new()
//     {
//         Template = @"
//             <message role=""system"">You are a helpful assistant who can detect the intent from the user's request. Use one of the follwing choices to respond.
//             If you don't know the intent, don't guess; instead respond with ""Unknown"".
//             Choices: SendEmail, SendMessage, CompleteTask, CreateDocument. 
//             </message>
//             <message role=""user"">{{request}}</message>
//             <message role=""system"">Intent:</message>
//             ",
//         TemplateFormat = "handlebars"
//     },
//     new HandlebarsPromptTemplateFactory()
// );
// KernelArguments chatIntentArgs = new() { { "request", "Can you send a very quick approval to the marketing team?" } };
// Console.WriteLine(await kernel.InvokeAsync(getIntent, chatIntentArgs));

#endregion

#region plugins
var kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(
                        endpoint: connector.Endpoint,
                        apiKey: connector.ApiKey,
                        deploymentName: connector.DeploymentOrModelId
                    )
                    .AddAzureOpenAITextGeneration(
                        endpoint: connector.Endpoint,
                        apiKey: connector.ApiKey,
                        deploymentName: connector.DeploymentOrModelId
                    )
                    .Build();
Console.WriteLine("-----------------------------------------------------------------------------------");
Console.WriteLine("Example 6. Load Plugins");
var intentPlugin = kernel.ImportPluginFromPromptDirectory("Plugins/IntentPlugin", "GetIntent");


#endregion