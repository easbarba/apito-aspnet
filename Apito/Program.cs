/*
* apito-aspnet is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* apito-aspnet is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with apito-aspnet. If not, see <https://www.gnu.org/licenses/>.
*/

using Apito.DTOs;
using Apito.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    options.AddPolicy(
        name: "AnyOrigin",
        cfg =>
        {
            cfg.AllowAnyOrigin();
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
        }
    );
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "Apito - ASP.NET",
            Description = "Evaluate soccer referees' performance.",
            License = new OpenApiLicense
            {
                Name = "GNU GENERAL PUBLIC LICENSE Version 3",
                Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.en.html")
            }
        }
    );
});
builder.Services.AddDbContext<DataContext>();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapGet("/", [EnableCors("AnyOrigin")] () => new ResponseDTO<string>("Welcome to Apito!")); //.WithOpenApi();

// app.UsePathBase(new PathString("/api/v1"));
// app.UseRouting();
app.MapControllers().RequireCors("AnyOrigin");
app.Run();

public partial class Program { }
