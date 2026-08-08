using System.Data;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;
namespace WebPortal.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository, IFetchableByBankUserRepository<UserModel>
    {
        public UserRepository(string connectionString) : base(connectionString) { }
        public UserModel GetByName(string bankName, string userName)
        {
            string query = @"
                SELECT b.BankName, b.BankID, u.Password, u.UserName
                FROM Users u 
                INNER JOIN Banks b ON u.BankID = b.BankID
                WHERE b.BankName=@BankName AND u.UserName=@UserName;";


            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = bankName;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new UserModel
                    {
                        BankId = reader.GetInt32(reader.GetOrdinal("BankID")),
                        BankName = reader.GetString(reader.GetOrdinal("BankName")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    };
                }
            }
        }

    }
}
