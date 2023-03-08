using System.Diagnostics;
using System.Text;

namespace shared;

public static class Executor
{
    /// <summary>
    /// This function allows you to execute a local command and listen to the output. Super simple but also kind
    /// of really annoying. It also automatically adds the CI ENV cause some apps need it.
    /// </summary>
    /// <remarks>
    /// So this is super weird but the handlers are passed into the function so that I don't create a memory leak.
    /// </remarks>
    /// <param name="config"></param>
    /// <param name="outputReceiver"></param>
    /// <param name="errorReceiver"></param>
    /// <returns>The output, errors and exit code in one package.</returns>
    public static ExecutorResponse Execute(ExecutorConfig config, Func<string?, Task>? outputReceiver = null, Func<string?, Task>? errorReceiver = null)
    {
        using Process process = new Process();

        process.StartInfo.FileName = config.Command;

        if (!string.IsNullOrWhiteSpace(config.Arguments))
        {
            process.StartInfo.Arguments = config.Arguments;
        }

        if (!string.IsNullOrWhiteSpace(config.WorkingDirectory))
        {
            process.StartInfo.WorkingDirectory = config.WorkingDirectory;
        }
        
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
            
        process.StartInfo.EnvironmentVariables.Add("CI", "true");
        
        foreach (var env in config.EnvironmentVariables)
        {
            process.StartInfo.EnvironmentVariables.Add(env.Key, env.Value);
        }

        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += async (_, args) =>
        {
            output.Append(args.Data);
            
            if (outputReceiver != null)
            {
                await outputReceiver(output.ToString());
            }
        };
            
        process.ErrorDataReceived += async (_, args) =>
        {
            error.Append(args.Data);
            
            if (errorReceiver != null)
            {
                await errorReceiver(error.ToString());
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
            ResponseCode = process.ExitCode
        };
    }
}

public class ExecutorResponse
{
    public string Output { get; set; }
    public string Error { get; set; }
    public int ResponseCode { get; set; }
}

public class ExecutorConfig
{
    public string Command { get; set; }
    public string Arguments { get; set; } = "";

    public string WorkingDirectory { get; set; } = "";

    public Dictionary<string, string> EnvironmentVariables { get; set; } = new ();
}