using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using TestClientsRequests.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(typeof(GoogleSheetsHelper));

builder.Services.AddHttpContextAccessor();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var localizationOptions = new RequestLocalizationOptions();

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ru-RU"),
    new CultureInfo("uz-UZ")
};

localizationOptions.SupportedCultures = supportedCultures;
localizationOptions.SupportedUICultures = supportedCultures;
localizationOptions.SetDefaultCulture("uz-UZ");
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration).WriteTo.Console());

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

var app = builder.Build();

app.UseRequestLocalization(localizationOptions);

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
