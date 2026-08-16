using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Infrastructure.Repositories
{
    public class CounterRepository : BaseRepository,
                                       IFetchableRepository<CounterModel>,
                                       IListableRepository<CounterModel>,
                                       IAddableRepository<CounterModel>,
                                       IUpdateableRepository<CounterModel>,
                                       IDeleteableRepository<CounterModel>
    {
        public CounterRepository(string connectionString) : base(connectionString) { }

        public CounterModel GetById(int counterId)
        {
            string query = @"
                SELECT CounterNameEN, CounterNameAR, IsActive, ModifiedAt, BranchID, co.TypeID, TypeName, CounterID
                FROM Counters co INNER JOIN CounterTypes ct
                ON co.TypeID = ct.TypeID
                WHERE CounterID=@CounterID;";


            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@CounterID", SqlDbType.Int).Value = counterId;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new CounterModel
                    {
                        CounterId = reader.GetInt32(reader.GetOrdinal("CounterID")),
                        CounterNameEN = reader.GetString(reader.GetOrdinal("CounterNameEN")),
                        CounterNameAR = reader.GetString(reader.GetOrdinal("CounterNameAR")),
                        CounterStatus = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        ModifiedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ModifiedAt")),
                        TypeID = reader.GetInt32(reader.GetOrdinal("TypeID")),
                        BranchId = reader.GetInt32(reader.GetOrdinal("BranchID")),
                        TypeName = reader.GetString(reader.GetOrdinal("TypeName")),
                    };
                }
            }
        }

        public IEnumerable<CounterModel> GetAll(int branchId)
        {
            string query = @"
                SELECT CounterNameEN, CounterNameAR, IsActive, ModifiedAt, BranchID, co.TypeID, TypeName, CounterID
                FROM Counters co INNER JOIN CounterTypes ct
                ON co.TypeID = ct.TypeID
                WHERE BranchID=@BranchID;";
            List<CounterModel> counters = new List<CounterModel>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {

                cmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int counterIdOrd = reader.GetOrdinal("CounterID");
                        int typeIdOrd = reader.GetOrdinal("TypeID");
                        int branchIdOrd = reader.GetOrdinal("BranchID");
                        int counterNameENOrd = reader.GetOrdinal("CounterNameEN");
                        int counterNameAROrd = reader.GetOrdinal("CounterNameAR");
                        int counterStatusOrd = reader.GetOrdinal("IsActive");
                        int modifiedAtOrd = reader.GetOrdinal("ModifiedAt");
                        int typeNameOrd = reader.GetOrdinal("TypeName");
                        while (reader.Read())
                        {
                            counters.Add(new CounterModel
                            {
                                CounterId = reader.GetInt32(counterIdOrd),
                                CounterNameEN = reader.GetString(counterNameENOrd),
                                CounterNameAR = reader.GetString(counterNameAROrd),
                                CounterStatus = reader.GetBoolean(counterStatusOrd),
                                ModifiedAt = reader.GetDateTimeOffset(modifiedAtOrd),
                                TypeID = reader.GetInt32(typeIdOrd),
                                BranchId = reader.GetInt32(branchIdOrd),
                                TypeName = reader.GetString(typeNameOrd),
                            });
                        }
                    }

                }

            }
            return counters;
        }


        public int Add(CounterModel counterModel)
        {
            string query = @"
                INSERT INTO Counters (CounterNameEN, CounterNameAR, IsActive, TypeID, BranchID)
                VALUES(@CounterNameEN, @CounterNameAR, @IsActive, @TypeID, @BranchID);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@CounterNameEN", SqlDbType.NVarChar, 100).Value = counterModel.CounterNameEN;
                cmd.Parameters.Add("@CounterNameAR", SqlDbType.NVarChar, 100).Value = counterModel.CounterNameAR;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = counterModel.CounterStatus;
                cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = counterModel.TypeID;
                cmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = counterModel.BranchId;
                conn.Open();
                if (cmd.ExecuteScalar() is int newId)
                {
                    return newId;
                }
                throw new InvalidOperationException("Database failed to return a valid identity ID.");

            }
        }
        public bool Update(CounterModel counterModel)
        {
            string query = @"
                UPDATE Counters 
                SET CounterNameEN=@CounterNameEN, CounterNameAR=@CounterNameAR, IsActive=@IsActive, TypeID=@TypeID
                WHERE CounterID=@CounterID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@CounterID", SqlDbType.Int).Value = counterModel.CounterId;
                cmd.Parameters.Add("@CounterNameEN", SqlDbType.NVarChar, 100).Value = counterModel.CounterNameEN;
                cmd.Parameters.Add("@CounterNameAR", SqlDbType.NVarChar, 100).Value = counterModel.CounterNameAR;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = counterModel.CounterStatus;
                cmd.Parameters.Add("@TypeID", SqlDbType.Int).Value = counterModel.TypeID;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int counterId)
        {
            string query = @"
                DELETE 
                FROM Counters 
                WHERE CounterID = @CounterID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@CounterID", SqlDbType.Int).Value = counterId;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


    }
}
