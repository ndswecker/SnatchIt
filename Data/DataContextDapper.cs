using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace SnatchItAPI.Data
{
    class DataContextDapper
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
            _connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ?? "Your fallback connection string here";

        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);

            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);

            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Execute(sql);
        }

        public bool ExecuteSqlWithParameters(string sql, DynamicParameters parameters)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Execute(sql, parameters) > 0;
        }
        public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Query<T>(sql, parameters);
        }

        public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
        {
            // IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.QuerySingle<T>(sql, parameters);
        }
    }
}

// using System.Data;
// using Dapper;
// using Microsoft.Data.SqlClient;
// using System; // Make sure to add this for accessing Environment

// namespace SnatchItAPI.Data
// {
//     class DataContextDapper
//     {
//         private readonly string _connectionString;

//         public DataContextDapper()
//         {
//             // Load the connection string directly from the environment variable
//             _connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ?? "Your fallback connection string here";
//         }

//         public IEnumerable<T> LoadData<T>(string sql)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.Query<T>(sql);
//         }

//         public T LoadDataSingle<T>(string sql)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.QuerySingle<T>(sql);
//         }

//         public bool ExecuteSql(string sql)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.Execute(sql) > 0;
//         }

//         public int ExecuteSqlWithRowCount(string sql)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.Execute(sql);
//         }

//         public bool ExecuteSqlWithParameters(string sql, DynamicParameters parameters)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.Execute(sql, parameters) > 0;
//         }

//         public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.Query<T>(sql, parameters);
//         }

//         public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
//         {
//             using IDbConnection dbConnection = new SqlConnection(_connectionString);
//             return dbConnection.QuerySingle<T>(sql, parameters);
//         }
//     }
// }
