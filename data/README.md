# Data

So the thing about data. We are primarily using postgres as a nosql data store.
The only fields that should be on the table outside of the `content` which represents
the json structure should be the primary key and foreign key fields. Anything that
can be used for searching can also be there, though I might use a dedicated search database
for that.
