// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using RestifyServer.Configuration;
using RestifyServer.Repository;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.Seq(AppConfiguration.GetSeqUrl(builder.Configuration)).CreateLogger();

builder.Services.AddDbContext<RestifyContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
    options.LogTo(Log.Information, LogLevel.Information);
    options.LogTo(Log.Error, LogLevel.Error);
    options.LogTo(Log.Warning, LogLevel.Warning);
    options.LogTo(Log.Fatal, LogLevel.Error);
});

builder.Services.AddRepositories();
builder.Services.AddMapper();
builder.Services.AddUtils();
builder.Services.AddServices();
builder.Services.AddControllers(options => ControllerConfigFactory.ConfigureControllers(options));
builder.Services.AddSwagger();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

Log.CloseAndFlush();
