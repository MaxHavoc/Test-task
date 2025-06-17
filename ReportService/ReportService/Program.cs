using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportService.Middleware;
using ReportService.Repositories;
using ReportService.Services.EmployeeCode;
using ReportService.Services.ReportFormatter;
using ReportService.Services.Salary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IReportFormatter, ReportFormatter>();
builder.Services.AddHttpClient<IEmployeeCodeService, EmployeeCodeService>();
builder.Services.AddHttpClient<ISalaryService, SalaryService>();

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