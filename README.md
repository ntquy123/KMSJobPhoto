# FGInventoryManagement

## Overview
FGInventoryManagement is a .NET 8 Web API that powers finished-goods inventory operations. The solution retains the familiar `Startup` pattern while embracing the minimal hosting model introduced in modern .NET, allowing the team to keep existing middleware, centralized logging (NLog), and configuration conventions intact.

## Application Bootstrapping Flow
1. **Host initialization** – `Program.cs` creates the `WebApplicationBuilder`, removes the default logging providers, and registers NLog as the primary logging implementation.
2. **Service configuration** – `Program` instantiates the `Startup` class with the current `Configuration` and `Environment`, then calls `Startup.ConfigureServices` to register the DbContext, middleware, SignalR hubs, Swagger, and application services.
3. **Application build** – With services registered, the application is built into a `WebApplication`.
4. **Pipeline composition** – `Program` retrieves `ILoggerFactory` and `ILoggerManager` from the DI container and passes them to `Startup.Configure` to wire up the middleware pipeline (static files, CORS, authentication, Swagger UI, and custom middleware).
5. **Execution** – `app.Run()` starts the web host.

---

## Example Project Information
- **Server address:** `10.109.25.200`
- **Setup link:** [`http://10.109.25.200:8081/api/BuyerLabelUpload/GetComBoBoxForBuyer`](http://10.109.25.200:8081/api/BuyerLabelUpload/GetComBoBoxForBuyer)

---

## Deployment Instructions
1. Deploy the Git repository to the server directory: `/root/PungKook/AOMTOPS/FGInventory`.
2. Install Docker on the Linux server.
3. Check for running Docker containers to avoid conflicts with other Docker-based applications.
4. **Create a `Dockerfile`:** place it in the same directory as the .NET 8 project. It will be used to build the application image.
5. **Create a `docker-compose.yml`:** use it to build and run the .NET 8 Docker image and publish the API behind the Nginx reverse proxy.
6. Ensure the Nginx configuration files reside under `/root/PungKook/AOMTOPS/FGInventory/nginx`.

---

## Basic Docker Commands
> **Note:** Always `cd` into the project directory that contains the `Dockerfile` before running Docker commands.

| Task | Command |
|------|---------|
| View running Docker containers | `docker ps` |
| Build and start Docker containers | `docker compose up --build -d` |
| Stop and remove all containers | `docker compose down` |
| View .NET API container logs | `docker logs erp-dotnet-api` |
| View Nginx proxy container logs | `docker logs erp-nginx-proxy` |

---

## Additional Resources
- [.NET Documentation](https://learn.microsoft.com/dotnet/)
- [NLog Documentation](https://nlog-project.org/documentation.html)
- [Docker Documentation](https://docs.docker.com/)
