# Sistema de Inmobiliaria  
[![built with Codeium](https://codeium.com/badges/main)](https://codeium.com)

## Tecnologías  
Basado en .Net Core 7. Trabaja con MSSqlLocalDB pero puede cambiarse de motor de BD fácilmente.

## Herramientas  
- [Framework .Net Core](https://dotnet.microsoft.com/download). Indispensable.
- [Visual Studio Code](https://code.visualstudio.com/download). Para el trabajo día a día.
- [aspnet-codegenerator](https://learn.microsoft.com/es-mx/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator). Motor de scaffolding de ASP.NET Core, para generar código.
- [Github Desktop](https://desktop.github.com/). Opcional, se puede integrar Github en VS Code.
- [Postman](https://www.postman.com/downloads/). Para hacer peticiones a la Web API.
- [Diagramas](https://app.diagrams.net/). Para modelar diagramas.
- [Figma](https://www.figma.com/). Para modelar pantallas.

## Extensiones  
- [C# para VS Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp). Extensión de C# para VS Code.
- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit). Kit de desarrollo de C# para VS Code.
- [Postman](https://marketplace.visualstudio.com/items?itemName=Postman.postman-for-vscode). Usar Postman dentro de VS Code.
- [Conveyor by Keyoti](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1448185.ConveyorbyKeyoti). Para poder conectarse remotamente al IIS Express que genera Visual Studio (no VS Code).

## Paquetes  
- [Microsoft.VisualStudio.Web.CodeGeneration.Design](https://www.nuget.org/packages/Microsoft.VisualStudio.Web.CodeGeneration.Design). Necesaria para usar aspnet-codegenerator.
- [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore). ORM de .Net Core.
- [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql). Extensión de EntityFrameworkCore para MySql.
- [JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer). Uso de JWT para autenticar.
- [MailKit](nuget.org/packages/MailKit). Envío de correos electrónicos.
- [QRCoder](nuget.org/packages/QRCoder). Generador de códigos QR.
- [LazZiya.ImageResize](https://www.nuget.org/packages/LazZiya.ImageResize). Manipular imágenes.

## Comandos
- dotnet new mvc [doc](https://learn.microsoft.com/es-es/dotnet/core/tools/dotnet-new-sdk-templates#web-options)
- dotnet new webapi --use-controllers [doc](https://learn.microsoft.com/es-es/dotnet/core/tools/dotnet-new-sdk-templates#webapi)
- dotnet build
- dotnet run
- dotnet clean
- dotnet dev-certs https --trust [doc](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs)
- dotnet user-secrets init [doc](https://learn.microsoft.com/es-mx/aspnet/core/security/app-secrets)
- dotnet add *location of your test csproj file* reference *location of the csproj file for project to be tested* [doc](https://code.visualstudio.com/docs/csharp/testing)
- sqllocaldb info [doc](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/start-stop-pause-resume-restart-sql-server-services)
- sqllocaldb versions [doc](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/start-stop-pause-resume-restart-sql-server-services)
- sqllocaldb stop MSSQLLocalDB
- sqllocaldb start MSSQLLocalDB

## Plantilla para Proyectos
[Plantilla basada en Markdown](https://hackmd.io/@nttUoarcRQOCiYt3qgy_SQ/plantilla_proyecto)
