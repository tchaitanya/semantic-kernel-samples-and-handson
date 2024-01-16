using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

internal static class KernelBuilder
{
    
    /// <summary>
    /// Load the settings from the default config file or from user secrets.
    /// This will add the AI (LLM) or connector to the Kernel, which will process the requests.
    /// </summary>
    static readonly KernelSettings connector = KernelSettings.LoadSettings();

    /// <summary>
    /// Initialize the kernel based on the service type.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    internal static Kernel InitializeKernel()
    {
        var builder = Kernel.CreateBuilder();
        Console.WriteLine($"Initializing kernel with {connector.ServiceType} connector");
        if (connector.ServiceType == ServiceTypes.AzureOpenAI)
        {
            return InitializeAzureOpenAIKernel(builder);
        }
        else if (connector.ServiceType == ServiceTypes.OpenAI)
        {
            return InitializeOpenAIKernel(builder);
        }
        else
        {
            throw new ArgumentException($"Service type {connector.ServiceType} is not supported in this app");
        }
    }


    /// <summary>
    /// Initialize the kernel with the Azure OpenAI resource.
    /// </summary>
    /// <returns></returns>
    private static Kernel InitializeAzureOpenAIKernel(IKernelBuilder builder)
    {
        builder.Services.AddAzureOpenAIChatCompletion(
                        endpoint: connector.Endpoint,
                        apiKey: connector.ApiKey,
                        deploymentName: connector.DeploymentOrModelId
                    );

        // builder.Services.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Trace));    
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddFilter(level => false);
            loggingBuilder.AddConsole();
        });

        builder.Plugins.AddFromType<MathPlugin>();

        return builder.Build();
    }

    /// <summary>
    /// Initialize the kernel with the OpenAI.
    /// </summary>
    /// <returns></returns>
    private static Kernel InitializeOpenAIKernel(IKernelBuilder builder)
    {
        builder.Services
                    .AddOpenAIChatCompletion(
                        apiKey: connector.ApiKey,
                        modelId: connector.DeploymentOrModelId,
                        orgId: connector.OrgId,
                        serviceId: connector.ServiceId
                    );

        builder.Services.AddLogging(configure => configure.AddConsole());

        return builder.Build();
    }
}