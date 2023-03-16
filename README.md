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

## Dev

Copy `example-appsettings.json` to `appsettings.Local.json` and fill it out. Then run the below commands.

```bash
cp appsettings.Local.json api
cp appsettings.Local.json scheduler
cp appsettings.Local.json runner
```

Run a dev nomad agent. with `nomad agent -dev run`

Make a bash script in your `$PATH` called runner that references the runner project. Mine looks like this.

```bash
#!/usr/bin/env bash

cd $PUSH_DIR/runner

dotnet run $@
```

Run `mongod` with basic settings. If you are mac you will need to specify a data directory and with admin perms.
The command I used was `sudo mongod --dbpath ~/mon`

While you don't really need indexes for dev, depending on your environment you may or may not need them.
The only ones I suggest are two indexes on the `jobs` collection.

```
{ Created: -1 }
{ Status: 1 }
```

This will help the scheduler out. The jobs table will be the largest one and likely the most hit (once per second by default).
Feel free to add any other indexes based on your data. Atlas will recommend them if you use it (which I recommend in prod environments).

If you don't need the jobs to stick around, feel free to purge the table. There will be no side effects on the app running. 
