# Data

So the thing about data. We are primarily using postgres as a nosql data store.
The only fields that should be on the table outside of the `content` which represents
the json structure should be the primary key and foreign key fields. Anything that
can be used for searching can also be there, though I might use a dedicated search database
for that.

## Database

```sh
# Shut down api
dotnet ef database drop -f
dotnet ef migrations remove
dotnet ef migrations add Init
dotnet ef database update

# Start api back up
curl http://localhost:5251/Util/fill
```

Just reset database

```sh
# Shut down api
dotnet ef database drop -f
dotnet ef database update

# Start api back up
curl http://localhost:5251/Util/fill
```

