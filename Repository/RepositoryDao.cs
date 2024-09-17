using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace hoslog.signalr.api.Repository;

public class RepositoryDao
{
    private readonly IConfiguration _configuration;
    private const int _commandTimeOut = 120;

    public RepositoryDao(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString()
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
            return connectionString;
        throw new ArgumentException(nameof(connectionString));
    }

    public async Task<T> ExecuteAsync<T>(string proc, DynamicParameters param)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            var response = await connection.QueryAsync<T>(proc, param, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeOut);
            return response.SingleOrDefault();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<T>> ExecuteAsyncList<T>(string proc, DynamicParameters param)
    {
        try
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            var response = await connection.QueryAsync<T>(proc, param, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeOut);
            return response.ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }
}