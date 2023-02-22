# Push

Push is a governance focused devops tool designed to solve the complicated needs regulation applies to organizations.

## How pipelines work

So pipelines are made up of two primary components: stages and steps. Stages are lanes that contain steps. Steps actually run things.
Stages run sequentially and steps within them run sequentially (for now). This makes scheduling and UX way easier.

Steps will have parameters. You can define parameters at the pipeline level if you you wish to consolidate inputs across steps.
If you do not supply a value for a parameter it will ask for the value in the UI when configuring the app.

Pipelines will be defined as JSON. This is primarily because there's no first party support for
YAML in dotnet and the primary library that does support it isn't maintained to a point I would be comfortable using. My suggestion
would be to manage pipelines with a pipeline. A basic pipeline for this purpose will be provided in JSON format (for copy/paste).
