﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportService.Middleware;
using ReportService.Repositories;
using ReportService.Services.EmployeeCodeClient;
using ReportService.Services.ReportFormatter;
using ReportService.Services.SalaryClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IReportFormatter, ReportFormatter>();
builder.Services.AddHttpClient<IEmployeeCodeClient, EmployeeCodeClient>();
builder.Services.AddHttpClient<ISalaryClient, SalaryClient>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();
app.UseExceptionHandler();

app.MapControllers();

app.Run();