using System.Collections.Generic;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Infrastructure.Repositories
{
    public class CounterTypeRepository : BaseRepository, IGetAllRepository<CounterTypeModel>
    {
        public CounterTypeRepository(string connectionString) : base(connectionString) { }

        public IEnumerable<CounterTypeModel> GetAll()
        {
            string query = @"
                SELECT TypeID, TypeName
                FROM CounterTypes;";

            List<CounterTypeModel> counterTypes = new List<CounterTypeModel>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int typeIdOrd = reader.GetOrdinal("TypeID");
                        int typeNameOrd = reader.GetOrdinal("TypeName");
                        while (reader.Read())
                        {
                            counterTypes.Add(new CounterTypeModel
                            {
                                TypeID = reader.GetInt32(typeIdOrd),
                                TypeName = reader.GetString(typeNameOrd)

                            });
                        }
                    }

                }

            }
            return counterTypes;
        }
    }
}
