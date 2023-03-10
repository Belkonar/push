using System.Text;
using Microsoft.VisualBasic;

namespace shared;
public class DockerBuilder
{
    private readonly TempFolder _folder;
    private readonly StringBuilder _builder = new ();
    private readonly string _containerName = Guid.NewGuid().ToString();

    private readonly Dictionary<string, string> _volumes = new Dictionary<string, string>();

    /// <param name="folder">This is a temp folder to drop any files needed to run this Dockerfile</param>
    public DockerBuilder(TempFolder folder)
    {
        _folder = folder;
    }
    
    public void From(string image, string? alias = null)
    {
        var postFix = alias != null ? $" as {alias}" : "";
        
        _builder.AppendLine($"FROM {image}{postFix}");
        
        // Env("HOME", "/home/u");
    }
    
    public void Entrypoint(string entrypoint, params string[] args)
    {
        _builder.Append($"ENTRYPOINT [ \"{entrypoint}\"");

        if (args.Length > 0)
        {
            var processedArgs = args.Select(x => $"\"{x}\"");
            _builder.AppendLine($", {string.Join(", ", processedArgs)} ]");
        }
        else
        {
            _builder.AppendLine(" ]");
        }
    }

    public void Copy(string from, string to)
    {
        _builder.AppendLine($"COPY {from} {to}");
    }
    
    public void Copy(string relativeTo, string from, string to)
    {
        var relativePath = Path.GetRelativePath(relativeTo, from);
        _builder.AppendLine($"COPY {relativePath} {to}");
    }

    public void Run(string command)
    {
        _builder.AppendLine($"RUN {command}");
    }

    /// <summary>
    /// This sets up the entrypoint for running a script saved as a file
    /// </summary>
    /// <remarks>
    /// This currently only supports linux. Windows will need to be supported differently (likely raw)
    /// Theoretically we can handle it the same way for windows containers but I've literally never used them.
    /// </remarks>
    /// <param name="command">the command run as part of the script</param>
    public void SetupScript(string command)
    {
        // the location in the container
        const string finalLocation = "/command";
        var scriptLocation = _folder.GetFile();

        // build the bash script
        var commandBuilder = new StringBuilder();
        commandBuilder.AppendLine("#!/usr/bin/env bash");
        commandBuilder.AppendLine(command);
        
        File.WriteAllText(scriptLocation, commandBuilder.ToString());
        
        // copy the file to the container
        Copy(_folder.Dir, scriptLocation, finalLocation);
        
        // get exec permissions
        Run($"chmod +x {finalLocation}");
        
        // set the entrypoint to our new scriptIf
        Entrypoint(finalLocation);
    }

    public void SetupScript(List<string> commands)
    {
        SetupScript(string.Join("\n", commands));
    }

    public void Env(string key, string value)
    {
        _builder.AppendLine($"ENV {key}=\"{value}\"");
    }

    /// <summary>
    /// Sets everything up for remote docker inside a stage.
    /// </summary>
    /// <param name="key">The PEM key for accessing the server</param>
    /// <param name="host">The host of the server (including port)</param>
    public void SetupRemoteDocker(byte[] key, string host)
    {
        var keyLocation = _folder.GetFile();
        var sshConfigLocation = _folder.GetFile();
        
        File.WriteAllBytes(keyLocation, key);
        
        const string finalKeyLocation = "/command";
        const string finalSshConfigLocation = "$HOME/.ssh/config";
        
        // TODO: pull my config from other computer
        
        Copy(keyLocation, finalKeyLocation);
        Copy(sshConfigLocation, finalSshConfigLocation);
        
        Env("DOCKER_HOST", host);
        
        throw new NotImplementedException();
    }

    public string GetDockerfile()
    {
        // TODO Remove this lol
        Console.WriteLine("---------- START DOCKERFILE ----------");
        Console.WriteLine(_builder.ToString());
        Console.WriteLine("----------- END DOCKERFILE -----------");
        return _builder.ToString();
    }

    public void CreateFile()
    {
        // some stuff needs this to not hang for user input
        Env("CI", "true");

        File.WriteAllText(_folder.GetFile("dockerfile"), GetDockerfile());
    }

    public ExecutorConfig GetBuildConfig()
    {
        return new ExecutorConfig()
        {
            Command = "docker",
            Arguments = new List<string>()
            {
                "build",
                "-t", _containerName,
                "."
            },
            WorkingDirectory = _folder.Dir
        };
    }
    
    public ExecutorConfig GetRunConfig()
    {
        var args = new List<string>()
        {
            "run",
        };

        foreach (var volume in _volumes)
        {
            args.Add("-v");
            args.Add($"{volume.Key}:{volume.Value}");
        }
        
        args.Add(_containerName);

        return new ExecutorConfig()
        {
            Command = "docker",
            Arguments = args,
            WorkingDirectory = _folder.Dir
        };
    }
    
    public void WorkDir(string dir)
    {
        _builder.AppendLine($"WORKDIR {dir}");
    }

    public void Volume(string src, string dest)
    {
        _volumes.Add(src, dest);
    }

    /// <summary>
    /// Creates a volume and uses the destination as the working directory
    /// </summary>
    /// <param name="src">The folder on the host to bind (has to be in the build context)</param>
    /// <param name="dest">The location on the container to bind to (and make working directory)</param>
    public void WorkDirVolume(string src, string dest)
    {
        Volume(src, dest);
        WorkDir(dest);
    }
}
