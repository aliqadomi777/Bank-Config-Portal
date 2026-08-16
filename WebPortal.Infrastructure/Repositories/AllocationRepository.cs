using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Infrastructure.Repositories
{
    public class AllocationRepository : BaseRepository,
                                        IFetchableRepository<AllocationModel>,
                                        IListableRepository<AllocationModel>,
                                        IAddableRepository<AllocationModel>,
                                        IUpdateableRepository<AllocationModel>,
                                        IDeleteableRepository<AllocationModel>,
                                        IAllocationRepository
    {
        public AllocationRepository(string connectionString) : base(connectionString) { }

        public AllocationModel GetById(int allocationId)
        {
            string query = @"
                SELECT allocationID, CounterID, s.ServiceID, ServiceNameEN, ServiceNameAR
                FROM service_allocations sa INNER JOIN Services s
                ON sa.ServiceID = s.ServiceID
                WHERE allocationID=@allocationID;";


            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@allocationID", SqlDbType.Int).Value = allocationId;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new AllocationModel
                    {
                        AllocationId = reader.GetInt32(reader.GetOrdinal("allocationID")),
                        CounterId = reader.GetInt32(reader.GetOrdinal("CounterID")),
                        ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceID")),
                        ServiceNameEN = reader.GetString(reader.GetOrdinal("ServiceNameEN")),
                        ServiceNameAR = reader.GetString(reader.GetOrdinal("ServiceNameAR")),
                    };
                }
            }
        }

        public IEnumerable<AllocationModel> GetAll(int counterId)
        {

            string query = @"
                SELECT allocationID, CounterID, s.ServiceID, ServiceNameEN, ServiceNameAR
                FROM service_allocations sa INNER JOIN Services s
                ON sa.ServiceID = s.ServiceID
                WHERE CounterID=@CounterID;";
            List<AllocationModel> allocations = new List<AllocationModel>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {

                cmd.Parameters.Add("@CounterID", SqlDbType.Int).Value = counterId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int allocationIdOrd = reader.GetOrdinal("allocationID");
                        int counterIdOrd = reader.GetOrdinal("CounterID");
                        int serviceIdOrd = reader.GetOrdinal("ServiceID");
                        int serviceNameENIdOrd = reader.GetOrdinal("ServiceNameEN");
                        int serviceNameARIdOrd = reader.GetOrdinal("ServiceNameAR");
                        while (reader.Read())
                        {
                            allocations.Add(new AllocationModel
                            {
                                AllocationId = reader.GetInt32(allocationIdOrd),
                                CounterId = reader.GetInt32(counterIdOrd),
                                ServiceId = reader.GetInt32(serviceIdOrd),
                                ServiceNameEN = reader.GetString(serviceNameENIdOrd),
                                ServiceNameAR = reader.GetString(serviceNameARIdOrd)
                            });
                        }
                    }

                }

            }
            return allocations;
        }

        public int Add(AllocationModel allocationModel)
        {
            string query = @"
                INSERT INTO service_allocations (CounterID, ServiceID)
                VALUES(@CounterID, @ServiceID);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@CounterID", SqlDbType.Int).Value = allocationModel.CounterId;
                cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = allocationModel.ServiceId;
                conn.Open();
                if (cmd.ExecuteScalar() is int newId)
                {
                    return newId;
                }
                throw new InvalidOperationException("Database failed to return a valid identity ID.");
            }
        }
        public bool Update(AllocationModel allocationModel)
        {
            string query = @"
                UPDATE service_allocations 
                SET ServiceID=@ServiceID
                WHERE allocationID=@allocationID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = allocationModel.ServiceId;
                cmd.Parameters.Add("@allocationID", SqlDbType.Int).Value = allocationModel.AllocationId;

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int allocationId)
        {
            string query = @"
                DELETE 
                FROM service_allocations 
                WHERE allocationID = @allocationID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@allocationID", SqlDbType.Int).Value = allocationId;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool DeleteAll(int counterId)
        {
            string query = @"
                DELETE 
                FROM service_allocations 
                WHERE CounterID = @CounterID;";

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                using (var cmd = new SqlCommand(query, conn, transaction))
                {
                    try
                    {
                        cmd.Parameters.Add("@CounterID", SqlDbType.Int).Value = counterId;
                        int rowsAffected = cmd.ExecuteNonQuery();

                        transaction.Commit();
                        return rowsAffected > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

    }
}
