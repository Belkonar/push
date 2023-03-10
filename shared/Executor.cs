using System.Diagnostics;
using System.Text;

namespace shared;

public static class Executor
{
    /// <summary>
    /// This function allows you to execute a local command and listen to the output. Super simple but also kind
    /// of really annoying.
    /// </summary>
    /// <remarks>
    /// So this is super weird but the handlers are passed into the function so that I don't create a memory leak.
    /// The shared reciever is also throttled to only send once per timespan
    /// </remarks>
    /// <param name="config"></param>
    /// <param name="sharedReceiver">A combination of STDOUT and STDERR, currently the only one throttled</param>
    /// <param name="outputReceiver"></param>
    /// <param name="errorReceiver"></param>
    /// <returns>The output, errors and exit code in one package.</returns>
    public static ExecutorResponse Execute(ExecutorConfig config, Func<string?, Task>? sharedReceiver = null, Func<string?, Task>? outputReceiver = null, Func<string?, Task>? errorReceiver = null)
    {
        Console.WriteLine($"[{config}]");
        
        using Process process = new Process();

        process.StartInfo.FileName = config.Command;

        if (config.Arguments.Any())
        {
            process.StartInfo.Arguments = string.Join(" ", config.Arguments);
        }

        if (!string.IsNullOrWhiteSpace(config.WorkingDirectory))
        {
            process.StartInfo.WorkingDirectory = config.WorkingDirectory;
        }
        
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        
        // This will be used eventually for "raw" executions
        process.StartInfo.EnvironmentVariables.Add("CI", "true");
        
        foreach (var env in config.EnvironmentVariables)
        {
            process.StartInfo.EnvironmentVariables.Add(env.Key, env.Value);
        }
        
        var output = new StringBuilder();
        var error = new StringBuilder();
        var shared = new StringBuilder();

        var sharedThrottle = new Throttle(async () =>
        {
            if (sharedReceiver != null)
            {
                await sharedReceiver(shared.ToString());
            }
        });

        process.OutputDataReceived += async (_, args) =>
        {
            output.AppendLine(args.Data);
            shared.AppendLine(args.Data);
            
            if (outputReceiver != null)
            {
                await outputReceiver(output.ToString());
            }

            if (sharedReceiver != null)
            {
                await sharedThrottle.Call();
            }
        };
            
        process.ErrorDataReceived += async (_, args) =>
        {
            error.AppendLine(args.Data);
            shared.AppendLine(args.Data);
            
            if (errorReceiver != null)
            {
                await errorReceiver(error.ToString());
            }
            
            if (sharedReceiver != null)
            {
                await sharedThrottle.Call();
            }
        };
            
        process.Start();
            
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return new ExecutorResponse()
        {
            Output = output.ToString(),
            Error = error.ToString(),
            Shared = shared.ToString(),
            ResponseCode = process.ExitCode
        };
    }
}

public class ExecutorResponse
{
    public string Output { get; init; }
    public string Error { get; init; }
    public string Shared { get; init; }
    public int ResponseCode { get; init; }
    

    public override string ToString()
    {
        var builder = new StringBuilder();
        
        builder.AppendLine("========== Status Code ==========");
        builder.AppendLine($"{ResponseCode}");
        builder.AppendLine("============ Output =============");
        builder.AppendLine($"{Output}");
        builder.AppendLine("============= Error =============");
        builder.AppendLine($"{Error}");

        return builder.ToString();
    }
}

public class ExecutorConfig
{
    public string Command { get; init; }
    public List<string> Arguments { get; init; } = new List<string>();

    public string WorkingDirectory { get; init; } = "";

    public Dictionary<string, string> EnvironmentVariables { get; init; } = new ();

    public override string ToString()
    {
        return $"{Command} {string.Join(" ", Arguments)}";
    }
}