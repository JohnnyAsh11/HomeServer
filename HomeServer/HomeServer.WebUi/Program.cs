using MudBlazor.Services;
using HomeServer.WebUi.Components;
using HomeServer.Client;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

namespace HomeServer.WebUi;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add MudBlazor services
        builder.Services.AddMudServices();

        builder.WebHost.UseStaticWebAssets();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddHttpClient();

        // Adding the Client to the services collection.
        string? apiUrl = builder.Configuration["ApiUrl"];
        builder.Services.AddSingleton(
            services => new TodoListClient(
                services.GetRequiredService<HttpClient>(),
                apiUrl ?? throw new ArgumentException($"The API URL was not provided.")
            ));

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.UseStaticFiles();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}

/// <summary>
/// Determines the return value of a dialog.
/// </summary>
public enum DialogResultValue
{
    Confirm,
    Cancel
}
