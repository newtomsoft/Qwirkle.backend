### db migrations how-to
in repository directory :  
use command ```dotnet ef Database update --connection <connectionString>```  
exemple :  ```dotnet ef Database update --connection "Server=localhost;Database=qwirkle.dev;Trusted_Connection=True;"```



### Serilog SQL table
```
CREATE TABLE [Log]
(
   [Id] int IDENTITY(1,1) NOT NULL,
   [TimeStamp] datetimeoffset(7) NOT NULL,
   [Message] nvarchar(max) NULL,
   [MessageTemplate] nvarchar(max) NULL,
   [Level] nvarchar(128) NULL,
   [Exception] nvarchar(max) NULL,
   [Properties] xml NULL,
   [LogEvent] nvarchar(max) NULL

   CONSTRAINT [PK_Log]
     PRIMARY KEY CLUSTERED ([Id] ASC)
)
```