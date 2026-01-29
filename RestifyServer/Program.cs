// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using RestifyServer.Configuration;
using RestifyServer.ExceptionFilters;
using RestifyServer.Repository;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RestifyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

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
