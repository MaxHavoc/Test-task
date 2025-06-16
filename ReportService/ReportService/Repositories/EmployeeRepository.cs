using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ReportService.Models;

namespace ReportService.Repositories;

public class EmployeeRepository(IConfiguration config) : IEmployeeRepository
{
    private readonly string _connStr = config.GetConnectionString("Default");

    public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct)
    {
        await using var conn = new NpgsqlConnection(_connStr);
        const string sql = @"
            SELECT 
                e.name, 
                e.inn, 
                d.name AS department
            FROM emps e
            LEFT JOIN deps d ON e.departmentid = d.id
            WHERE d.active = true OR d.id IS NULL
            ORDER BY d.name NULLS LAST, e.name";
        
        return await conn.QueryAsync<Employee>(sql, ct);
    }
}