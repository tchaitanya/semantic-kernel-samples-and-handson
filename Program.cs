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

#endregion

#region Plugins - Get Intent of the user request

// Console.WriteLine("----------------------------Plugins-------------------------------------------------------");
// Console.WriteLine("Example 1. Import and add Plugin to the Kernel");
// // Import the Math plugin
// // kernel.ImportPluginFromPromptDirectory("Plugins","IntentPlugins");

// //Get the function from the Intent plugin
// KernelFunction getIntentFunction = kernel.Plugins.GetFunction("IntentPlugins", "GetIntent");

// // Invoke Intent plugin function
// var output = await kernel.InvokeAsync(getIntentFunction,
//                                         new KernelArguments() { 
//                                             { "request", "Can you send a very quick approval to the marketing team?" }
//                                         });
// Console.WriteLine($"User Intent : {output}");
#endregion

#region Plugins - Get Time Information
// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Native Function using Time Information Plugin");

// Console.WriteLine("Example 1: Invoke the kernel with a prompt that asks the AI for information it cannot provide and may hallucinate");
// KernelArguments kernelArguments1 = new() { { "event", "India Republic Day" } };
// Console.WriteLine(await kernel.InvokePromptAsync("How many days until {{$event}}?", kernelArguments1));


// Console.WriteLine("Example 2: Invoke the kernel with a templated prompt that invokes a plugin and display the result");

// //Use TimeInformation plugin
// Console.WriteLine(await kernel.InvokePromptAsync("The current time is {{TimeInformationPlugin.GetCurrentUtcTime}}. How many days until {{$event}}?", kernelArguments1));

// Console.WriteLine("Example 3: Invoke the kernel with a prompt and allow the AI to automatically invoke functions");

// // Create a new instance of OpenAIPromptExecutionSettings
// OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new ()
// {
//     // Set the ToolCallBehavior to AutoInvokeKernelFunctions
//     ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
// };

// Console.WriteLine(await kernel.InvokePromptAsync("The current time is {{TimeInformationPlugin.GetCurrentUtcTime}}. How many days until {{$event}}? Explain your thinking.", new(openAIPromptExecutionSettings)));

#endregion

#region Native Functions using Math Plugin

// Console.WriteLine("---------------------------Math Plugin--------------------------------------------------------");
// Console.WriteLine("Example 1: Native Function using Math Plugin");

// // // Import the Math plugin
// //kernel.ImportPluginFromPromptDirectory("Plugins","MathPlugin");

// //Get the function from the Math plugin
// var mathKernelFunction = kernel.Plugins.GetFunction("MathPlugin", "Sqrt");

// //Set the Arguments
// var kernelArguments = new KernelArguments() { { "number1", 12 } };

// // Test the math plugin

// // double functionResults = await kernel.InvokeAsync<double>("Math", "Sqrt", kernelArguments);

// double functionResults = await kernel.InvokeAsync<double>(mathKernelFunction, kernelArguments);

// Console.WriteLine($"The square root of 12 is {functionResults}.");

#endregion

#region Invoke Math Plugin using AI 
// Console.WriteLine("-----------------------------------------------------------------------------------");
// Console.WriteLine("Example 2: Invoke Math Plugin using AI");

// // Create chat history
// ChatHistory chatHistory= [];

// // Get chat completion service
// var chatCompletionService1 = kernel.GetRequiredService<IChatCompletionService>();

// // Start the conversation
// while (true)
// {
//     // Get user input
//     Console.Write("User > ");
//     chatHistory.AddUserMessage(Console.ReadLine()!);

//     // Enable auto function calling
//     OpenAIPromptExecutionSettings openAIPromptExecutionSettings1 = new()
//     {
//         ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
//     };

//     // Get the response from the AI
//     var result = chatCompletionService1.GetStreamingChatMessageContentsAsync(
//         chatHistory,
//         executionSettings: openAIPromptExecutionSettings1,
//         kernel: kernel);

//     // Stream the results
//     string fullMessage = "";
//     var first = true;
//     await foreach (var content in result)
//     {
//         if (content.Role.HasValue && first)
//         {
//             Console.Write("Assistant > ");
//             first = false;
//         }
//         Console.Write(content.Content);
//         fullMessage += content.Content;
//     }
//     Console.WriteLine();

//     // Add the message from the agent to the chat history
//     chatHistory.AddAssistantMessage(fullMessage);
// }
#endregion