namespace HomeServer.CompileFormula1Data
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // The URI to connect to OpenF1
            string uri = "https://api.openf1.org/v1/";

            // Configuring and building the web app.
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureServices(ConfigureServices);

            WebApplication app = builder.Build();

            // Configuring the application.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Start();

            HttpClient client = app.Services
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient("OpenF1");

            HttpResponseMessage resp = await client.GetAsync("session_result?session_key=11227");
            Console.WriteLine(await resp.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Configures the settings for the TodoList Api application service.
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddOpenApi();
            services.AddControllers();

            string uri = "https://api.openf1.org/v1/";
            services.AddHttpClient("OpenF1", client =>
            {
                client.BaseAddress = new Uri(uri);
            });
        }
    }
}
