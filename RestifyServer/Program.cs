// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using RestifyServer.Configuration;
using RestifyServer.Repository;
using RestifyServer.Utils;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Enviroment", builder.Environment.EnvironmentName)
    .Enrich.WithProperty("ApplicationName", builder.Environment.ApplicationName)
    .WriteTo.Seq(AppConfiguration.GetSeqUrl(builder.Configuration))
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<RestifyContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(2);
    });

    options.UseLoggerFactory(new LoggerFactory());
});

builder.Services.AddRepositories();
builder.Services.AddMapper();
builder.Services.AddUtils();
builder.Services.AddServices();
builder.Services.AddControllers(options => ControllerConfigFactory.ConfigureControllers(options));
builder.Services.AddSwagger();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();
app.TestDbConnection();
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

Log.CloseAndFlush();
