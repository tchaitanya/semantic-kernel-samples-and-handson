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
using System.Reflection;


Console.WriteLine("---------------------Initializing kernel with Connector settings--------------------");
Kernel kernel = KernelBuilder.InitializeKernel();
Console.WriteLine("---------------------Kernel initialized---------------------------------------------");

#region prompt_engineering

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 1. Invoke the kernel with a templated prompt and display the result");
// KernelArguments arguments = new() { { "request", "Send an approval email to the marketing team" } };
// Console.WriteLine(await kernel.InvokePromptAsync("What is the intent of this request {{$request}}?", arguments));
// Console.WriteLine();

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 2. Invoke the kernel with a templated prompt and stream the results to the display");
// KernelArguments arguments = new() { { "request", "Generate marketing idea for new product launch called 'Xbox'" } };
// await foreach (var update in kernel.InvokePromptStreamingAsync("What is the intent of this request {{$request}}? Explain the thought process behind the answer.", arguments))
// {
//     Console.Write(update);
// }
// Console.WriteLine();

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 3. Stream the results from chat completion service with execution settings");
// KernelArguments arguments = new() { { "request", "Send an approval email to the marketing team" } };
// var executionSettings = new OpenAIPromptExecutionSettings()
// {
//     MaxTokens = 256,
//     FrequencyPenalty = 0,
//     PresencePenalty = 0,
//     Temperature = 1,
//     TopP = 0.5
// };
// IChatCompletionService chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
// ChatHistory chatHistory = new() 
// {   
//     new ChatMessageContent(AuthorRole.System, @"You are a helpful assistant who can detect the intent from the user's request. 
//                                                 Use one of the follwing choices to respond and also generate sample email or message.
//                                                 If you don't know the intent, don't guess; instead respond with 'Unknown'.
//                                                 Choices: SendEmail, SendMessage "),
//     new ChatMessageContent(AuthorRole.User, "Send an approval email to the marketing team with notes on the new product launch 'Xbox'"),
//     new ChatMessageContent(AuthorRole.System, "Intent:"),
//     new ChatMessageContent(AuthorRole.System, "Sample:")
// };
// await foreach (var update in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, executionSettings))
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
// // Create hook
// kernel.PromptRendered += (sender, args) =>
// {
//     Console.WriteLine($"\nPrompt sent to LLM:\n {args.RenderedPrompt}\n");
// };
// // Create choices
// List<string> choices = ["SendEmail", "SendMessage", "CompleteTask", "CreateDocument"];

// // Create few-shot examples
// List<ChatHistory> fewShotExamples = [
//     [
//         new ChatMessageContent(AuthorRole.User, "Can you send a very quick approval to the marketing team?"),
//         new ChatMessageContent(AuthorRole.System, "Intent:"),
//         new ChatMessageContent(AuthorRole.Assistant, "SendMessage")
//     ],
//     [
//         new ChatMessageContent(AuthorRole.User, "Thanks, I'm done for now"),
//         new ChatMessageContent(AuthorRole.System, "Intent:"),
//         new ChatMessageContent(AuthorRole.Assistant, "Unknown")
//     ]
// ];
// // Create a sample chat history
// ChatHistory history = new();
// history.AddUserMessage("Create an excel sheet with the sales data for the last quarter");
// history.AddSystemMessage("Intent: CreateDocument");
// history.AddUserMessage("Generate a marketing idea to sell new product called 'Xbox'");
// history.AddSystemMessage("Intent: Unknown");

// var getIntent = kernel.CreateFunctionFromPrompt(
//     new()
//     {
//         Template = @"
//         <message role=""system"">Instructions: What is the intent of this request?
//         Do not explain the reasoning, just reply back with the intent. If you are unsure, reply with ""Unknown"".
//         Choices: {{choices}}.</message>

//         {{#each fewShotExamples}}
//             {{#each this}}
//                 <message role=""{{role}}"">{{content}}</message>
//             {{/each}}
//         {{/each}}

//         {{#each chatHistory}}
//             <message role=""{{role}}"">{{content}}</message>
//         {{/each}}

//         <message role=""user"">{{request}}</message>
//         <message role=""system"">Intent:</message>",
//         TemplateFormat = "handlebars"
//     },
//     new HandlebarsPromptTemplateFactory()
// );
// Console.WriteLine($"\nOutput: {await kernel.InvokeAsync(getIntent, 
//                 new KernelArguments() { 
//                         { "request", "Follow up with John for the quarter report" },
//                         { "history", history },
//                         { "choices", string.Join(", ", choices) },
//                         { "fewShotExamples", fewShotExamples }
//                     })}\n");


// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 6. Using YAML prompt template");
// // Load prompt from YAML
// using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("semantic_kernel_samples_and_handson.Prompts.getIntent.prompt.yaml")!);
// KernelFunction getIntent = kernel.CreateFunctionFromPromptYaml(
//     reader.ReadToEnd(),
//     promptTemplateFactory: new HandlebarsPromptTemplateFactory()
// );
// //Sample chat history
// var history = new ChatHistory();
// history.AddUserMessage("Create an excel sheet with the sales data for the last quarter");
// history.AddSystemMessage("Intent: CreateDocument");
// history.AddUserMessage("Generate a marketing idea to sell new product called 'Xbox'");
// history.AddSystemMessage("Intent: Unknown");

// // Sample few-shot examples
// var fewShotExamples = """
//     User: Can you send message to John to send me the report?
//     Intent: SendMessage
//     User: Can you generate a document on new product idea? 
//     Intent: CreateDocument
// """;
// // Take input
// Console.Write("Enter text > ");
// var request = Console.ReadLine();
// // Invoke prompt
// var intent = await kernel.InvokeAsync(
//     getIntent,
//     new() {
//         { "request", request },
//         { "choices", "SendEmail, SendMessage, CompleteTask, CreateDocument" },
//         { "history", history },
//         { "fewShotExamples", fewShotExamples }
//     }
// );
// Console.WriteLine($"Output: {intent}");
#endregion

#region plugins

// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 1. Import and add Plugin to the Kernel");
// var intentPlugin = kernel.ImportPluginFromPromptDirectory("Plugins/IntentPlugins");
// KernelFunction getIntentFunction = kernel.Plugins.GetFunction("IntentPlugins", "GetIntent");
// var output = await kernel.InvokeAsync(getIntentFunction,
//                                         new KernelArguments() { 
//                                             { "request", "Can you send a very quick approval to the marketing team?" }
//                                         });
// Console.WriteLine($"Output: {output}");


#endregion