using System.Text;

namespace shared;
public class DockerBuilder
{
    private readonly TempFolder _folder;
    private readonly StringBuilder _builder = new ();

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

    public void Run(string command)
    {
        _builder.AppendLine($"RUN {command}");
    }

    /// <summary>
    /// This sets up the entrypoint for running a script saved as a file
    /// </summary>
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
        Copy(scriptLocation, finalLocation);
        
        // get exec permissions
        Run($"chmod +x {finalLocation}");
        
        // set the entrypoint to our new script
        Entrypoint(finalLocation);
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
}
