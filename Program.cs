using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.Extensions.Azure;
using Microsoft.SemanticKernel.Plugins.Core;


#region prompt_engineering

Console.WriteLine("---------------------Initializing kernel with Connector settings--------------------");
Kernel kernel = KernelBuilder.InitializeKernel();
Console.WriteLine("---------------------Kernel initialized---------------------------");

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 1. Invoke the kernel with a zero-shot prompt and display the result");
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

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 5. Loops in Handlebars templates");
// // Add ConversationSummaryPlugin to the Kernel
// KernelPlugin conversationSummaryPlugin = kernel.ImportPluginFromType<ConversationSummaryPlugin>();
// // Create chat history
// var getIntent = kernel.CreateFunctionFromPrompt(
//     new()
//     {
//         Template = @"
//         <message role=""system"">Instructions: What is the intent of this request?
//         Do not explain the reasoning, just reply back with the intent. If you are unsure, reply with {{choices[0]}}.
//         Choices: {{choices}}.</message>

//         {{#each fewShotExamples}}
//             {{#each this}}
//                 <message role=""{{role}}"">{{content}}</message>
//             {{/each}}
//         {{/each}}

//         {{ConversationSummaryPlugin-SummarizeConversation history}}

//         <message role=""user"">{{request}}</message>
//         <message role=""system"">Intent:</message>",
//         TemplateFormat = "handlebars"
//     },
//     new HandlebarsPromptTemplateFactory()
// );
// Console.WriteLine($"\nOutput: {await kernel.InvokeAsync(getIntent, new KernelArguments() { { "request", "Can you send a very quick approval to the marketing team?" } })}\n");

// TODO Add 2-3 more examples 
#endregion

#region plugins

Console.WriteLine("-----------------------------------------------------------------------------------");
Console.WriteLine("Example 1. Import and add Plugin to the Kernel");
var intentPlugin = kernel.ImportPluginFromPromptDirectory("Plugins/IntentPlugins");
KernelFunction getIntentFunction = kernel.Plugins.GetFunction("IntentPlugins", "GetIntent");
var output = await kernel.InvokeAsync(getIntentFunction,
                                        new KernelArguments() { 
                                            { "request", "Can you send a very quick approval to the marketing team?" }
                                        });
Console.WriteLine($"Output: {output}");


#endregion