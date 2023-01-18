# Push

Push is a governance focused devops tool designed to solve the complicated needs regulation applies to organizations.

## Note on auth

In development I am directly using auth0 for simplicity. Once the project is closer to completion I will figure something else out.
Probably using a standalone service to handle auth and federating with whatever is folks want to use.

## How pipelines work

So pipelines are made up of two primary components: stages and commands. Stages are lanes that contain commands. Commands actually run things.
Stages run sequentially and commands within them run sequentially (for now). This makes scheduling and UX way easier.

Commands will have parameters. You can define parameters at the pipeline level if you you wish to consolidate inputs across commands.
If you do not supply a value for a parameter it will ask for the value in the UI when configuring the app.

Pipelines will be defined as JSON however the CLI will also accept YAML. This is primarily because there's no first party support for
YAML in dotnet and the primary library that does support it isn't maintained to a point I would be comfortable using. My suggestion
would be to manage pipelines with a pipeline. A basic pipeline for this purpose will be provided in JSON format (for copy/paste).
