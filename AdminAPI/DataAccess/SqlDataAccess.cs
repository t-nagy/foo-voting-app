using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminAPI.DataAccess
{
    public class SqlDataAccess : IDisposable
    {
        private readonly ConfigHelper _config;
        public IDbConnection? Connection { get; private set; }
        public IDbTransaction? Transaction { get; private set; }
        private bool _isClosed = false;

        public SqlDataAccess(ConfigHelper config)
        {
            _config = config;
        }

        public void StartTransaction(string connectionStringName)
        {
            Connection = new SqlConnection(_config.GetConnectionString(connectionStringName));
            Connection.Open();

            Transaction = Connection.BeginTransaction();

            _isClosed = false;
        }

        public void CommitTransaction()
        {
            Transaction?.Commit();
            Connection?.Close();
            _isClosed = true;
        }

        public void RollbackTransaction()
        {
            Transaction?.Rollback();
            Connection?.Close();
            _isClosed = true;
        }

        public void Dispose()
        {
            if (!_isClosed)
            {
                try
                {
                    CommitTransaction();
                }
                catch
                {
                }
            }

            Transaction = null;
            Connection = null;
        }
    }
}
