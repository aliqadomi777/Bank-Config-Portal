using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;
using WebPortal.Domain.Model;

namespace WebPortal.Infrastructure.Repositories
{
    public class BranchRepository : BaseRepository,
        IFetchableRepository<BranchModel>,
        IListableRepository<BranchModel>,
        IAddableRepository<BranchModel>,
        IUpdateableRepository<BranchModel>,
        IDeleteableRepository<BranchModel>
    {
        public BranchRepository(string connectionString) : base(connectionString) { }

        public BranchModel GetById(int branchId)
        {
            string query = @"
                SELECT BranchID, BranchNameEN, BranchNameAR, IsActive, ModifiedAt, BankID
                FROM Branches
                WHERE BranchID=@BranchID;";


            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new BranchModel
                    {
                        BranchId = reader.GetInt32(reader.GetOrdinal("BranchID")),
                        BankId = reader.GetInt32(reader.GetOrdinal("BankID")),
                        BranchNameEN = reader.GetString(reader.GetOrdinal("BranchNameEN")),
                        BranchNameAR = reader.GetString(reader.GetOrdinal("BranchNameAR")),
                        BranchStatus = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        ModifiedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ModifiedAt")),
                    };
                }
            }
        }

        public IEnumerable<BranchModel> GetAll(int bankId)
        {
            string query = @"
                SELECT BranchID, BranchNameEN, BranchNameAR, IsActive, ModifiedAt, BankID
                FROM Branches
                WHERE BankID=@BankID;";
            List<BranchModel> branches = new List<BranchModel>();

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {

                cmd.Parameters.Add("@BankID", SqlDbType.Int).Value = bankId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int branchIdOrd = reader.GetOrdinal("BranchID");
                        int bankIdOrd = reader.GetOrdinal("BankID");
                        int branchNameENOrd = reader.GetOrdinal("BranchNameEN");
                        int branchNameAROrd = reader.GetOrdinal("BranchNameAR");
                        int branchStatusOrd = reader.GetOrdinal("IsActive");
                        int modifiedAtOrd = reader.GetOrdinal("ModifiedAt");
                        while (reader.Read())
                        {
                            branches.Add(new BranchModel
                            {
                                BranchId = reader.GetInt32(branchIdOrd),
                                BranchNameEN = reader.GetString(branchNameENOrd),
                                BranchNameAR = reader.GetString(branchNameAROrd),
                                BranchStatus = reader.GetBoolean(branchStatusOrd),
                                ModifiedAt = reader.GetDateTimeOffset(modifiedAtOrd),
                                BankId = reader.GetInt32(bankIdOrd),
                            });
                        }
                    }

                }

            }
            return branches;
        }

        public int Add(BranchModel branchModel)
        {
            string query = @"
                INSERT INTO Branches (BranchNameEN, BranchNameAR, IsActive, BankID)
                VALUES(@BranchNameEN, @BranchNameAR, @IsActive, @BankID);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@BranchNameEN", SqlDbType.NVarChar, 100).Value = branchModel.BranchNameEN;
                cmd.Parameters.Add("@BranchNameAR", SqlDbType.NVarChar, 100).Value = branchModel.BranchNameAR;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = branchModel.BranchStatus;
                cmd.Parameters.Add("@BankID", SqlDbType.Int).Value = branchModel.BankId;
                conn.Open();
                if (cmd.ExecuteScalar() is int newId)
                {
                    return newId;
                }
                throw new InvalidOperationException("Database failed to return a valid identity ID.");

            }
        }


        public bool Update(BranchModel branchModel)
        {
            string query = @"
                UPDATE Branches 
                SET BranchNameEN=@BranchNameEN, BranchNameAR=@BranchNameAR, IsActive=@IsActive
                WHERE BranchID=@BranchID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchModel.BranchId;
                cmd.Parameters.Add("@BranchNameEN", SqlDbType.NVarChar, 100).Value = branchModel.BranchNameEN;
                cmd.Parameters.Add("@BranchNameAR", SqlDbType.NVarChar, 100).Value = branchModel.BranchNameAR;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = branchModel.BranchStatus;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        public bool Delete(int branchId)
        {
            string query = @"
                DELETE 
                FROM Branches 
                WHERE BranchID = @BranchID;";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
