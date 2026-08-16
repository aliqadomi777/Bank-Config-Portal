using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Infrastructure.Repositories
{
    public class ServiceRepository : BaseRepository,
       IFetchableRepository<ServiceModel>,
       IListableRepository<ServiceModel>,
       IAddableRepository<ServiceModel>,
       IUpdateableRepository<ServiceModel>,
       IDeleteableRepository<ServiceModel>
    {
        public ServiceRepository(string connectionString) : base(connectionString) { }

        public ServiceModel GetById(int serviceId)
        {
            string query = @"
                SELECT ServiceID, ServiceNameEN, ServiceNameAR, MaxTicketsPerDay, IsActive, ModifiedAt, BankID
                FROM Services
                WHERE ServiceID=@ServiceID;";


            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = serviceId;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new ServiceModel
                    {
                        ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceID")),
                        BankId = reader.GetInt32(reader.GetOrdinal("BankID")),
                        ServiceNameEN = reader.GetString(reader.GetOrdinal("ServiceNameEN")),
                        ServiceNameAR = reader.GetString(reader.GetOrdinal("ServiceNameAR")),
                        ServiceStatus = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        MaxTicketsPerDay = reader.GetInt32(reader.GetOrdinal("MaxTicketsPerDay")),
                        ModifiedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ModifiedAt")),
                    };
                }
            }
        }

        public IEnumerable<ServiceModel> GetAll(int bankId)
        {
            string query = @"
                SELECT ServiceID, ServiceNameEN, ServiceNameAR, MaxTicketsPerDay, IsActive, ModifiedAt, BankID
                FROM Services
                WHERE BankID=@BankID;";
            List<ServiceModel> services = new List<ServiceModel>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {

                cmd.Parameters.Add("@BankID", SqlDbType.Int).Value = bankId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int serviceIdOrd = reader.GetOrdinal("ServiceID");
                        int bankIdOrd = reader.GetOrdinal("BankID");
                        int serviceNameENOrd = reader.GetOrdinal("ServiceNameEN");
                        int serviceNameAROrd = reader.GetOrdinal("ServiceNameAR");
                        int serviceStatusOrd = reader.GetOrdinal("IsActive");
                        int modifiedAtOrd = reader.GetOrdinal("ModifiedAt");
                        int maxTicketsPerDayOrd = reader.GetOrdinal("MaxTicketsPerDay");
                        while (reader.Read())
                        {
                            services.Add(new ServiceModel
                            {
                                ServiceId = reader.GetInt32(serviceIdOrd),
                                ServiceNameEN = reader.GetString(serviceNameENOrd),
                                ServiceNameAR = reader.GetString(serviceNameAROrd),
                                ServiceStatus = reader.GetBoolean(serviceStatusOrd),
                                ModifiedAt = reader.GetDateTimeOffset(modifiedAtOrd),
                                BankId = reader.GetInt32(bankIdOrd),
                                MaxTicketsPerDay = reader.GetInt32(maxTicketsPerDayOrd)
                            });
                        }
                    }

                }

            }
            return services;
        }

        public int Add(ServiceModel serviceModel)
        {
            string query = @"
                INSERT INTO Services (ServiceNameEN, ServiceNameAR, IsActive, MaxTicketsPerDay, BankID)
                VALUES(@ServiceNameEN, @ServiceNameAR, @IsActive, @MaxTicketsPerDay,@BankID);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ServiceNameEN", SqlDbType.NVarChar, 100).Value = serviceModel.ServiceNameEN;
                cmd.Parameters.Add("@ServiceNameAR", SqlDbType.NVarChar, 100).Value = serviceModel.ServiceNameAR;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = serviceModel.ServiceStatus;
                cmd.Parameters.Add("@MaxTicketsPerDay", SqlDbType.Int).Value = serviceModel.MaxTicketsPerDay;
                cmd.Parameters.Add("@BankID", SqlDbType.Int).Value = serviceModel.BankId;
                conn.Open();
                if (cmd.ExecuteScalar() is int newId)
                {
                    return newId;
                }
                throw new InvalidOperationException("Database failed to return a valid identity ID.");

            }
        }


        public bool Update(ServiceModel serviceModel)
        {
            string query = @"
                UPDATE Services 
                SET ServiceNameEN=@ServiceNameEN, ServiceNameAR=@ServiceNameAR, IsActive=@IsActive, MaxTicketsPerDay=@MaxTicketsPerDay
                WHERE ServiceID=@ServiceID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = serviceModel.ServiceId;
                cmd.Parameters.Add("@ServiceNameEN", SqlDbType.NVarChar, 100).Value = serviceModel.ServiceNameEN;
                cmd.Parameters.Add("@ServiceNameAR", SqlDbType.NVarChar, 100).Value = serviceModel.ServiceNameAR;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = serviceModel.ServiceStatus;
                cmd.Parameters.Add("@MaxTicketsPerDay", SqlDbType.Int).Value = serviceModel.MaxTicketsPerDay;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public bool Delete(int serviceId)
        {
            string query = @"
                DELETE 
                FROM Services 
                WHERE ServiceID = @ServiceID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = serviceId;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


    }
}