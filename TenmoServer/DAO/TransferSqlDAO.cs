using Microsoft.AspNetCore.Razor.Language.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;


namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbconnectionString)
        {
            connectionString = dbconnectionString;
        }

        public Transfer GetTransfer(int id)
        {
            Transfer returnTransfer = null;
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT transfer_id, a.username as userfrom, b.username as userto, transfers.transfer_type_id, transfers.transfer_status_id, transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc, account_from, account_to, amount FROM transfers" +
                         " JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id" +
                         " JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id" +
                         " JOIN accounts AS f ON f.account_id = transfers.account_from" +
                         " JOIN accounts AS t ON t.account_id = transfers.account_to" +
                         " JOIN users AS a ON a.user_id = f.user_id" +
                         " JOIN users AS b ON b.user_id = t.user_id" +
                         " WHERE transfer_id = @transfer_id", sql);
                    sqlCommand.Parameters.AddWithValue("@transfer_id", id);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnTransfer = GetTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }

            return returnTransfer;
        }

        public List<Transfer> GetTransfers(int id)
        {
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT transfer_id, a.username as userfrom, b.username as userto, " +
                                                           "transfers.transfer_type_id, transfers.transfer_status_id, transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc, account_from, account_to, amount FROM transfers" +
                                                           " JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id" +
                         " JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id" +
                         " JOIN accounts AS f ON f.account_id = transfers.account_from" +
                         " JOIN accounts AS t ON t.account_id = transfers.account_to" +
                         " JOIN users AS a ON a.user_id = f.user_id" +
                         " JOIN users AS b ON b.user_id = t.user_id" +
                         " WHERE account_from = @user_id OR account_to = @user_id", sql);

                   /* SqlCommand sqlCommand = new SqlCommand("SELECT transfer_id, transfers.transfer_type_id, transfers.transfer_status_id, transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc, account_from, account_to, amount " +
                        "FROM transfers JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id" +
                         " JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id" +
                        " WHERE account_from = @user_id OR account_to = @user_id", sql);*/
                    sqlCommand.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while(reader.Read())
                    {
                        transfers.Add(GetTransferFromReader(reader));
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }

            return transfers;

        }

        public List<Transfer> GetPendingTransfers(int id)
        {
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT transfer_id, a.username as userfrom, b.username as userto, transfers.transfer_type_id, transfers.transfer_status_id, transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc, account_from, account_to, amount FROM transfers" +
                         " JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id" +
                         " JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id" +
                         " JOIN accounts AS f ON f.account_id = transfers.account_from" +
                         " JOIN accounts AS t ON t.account_id = transfers.account_to" +
                         " JOIN users AS a ON a.user_id = f.user_id" +
                         " JOIN users AS b ON b.user_id = t.user_id" +
                         " WHERE account_from = @user_id AND transfers.transfer_status_id = 1", sql);

                    /* SqlCommand sqlCommand = new SqlCommand("SELECT transfer_id, transfers.transfer_type_id, transfers.transfer_status_id, transfer_types.transfer_type_desc, transfer_statuses.transfer_status_desc, account_from, account_to, amount " +
                         "FROM transfers JOIN transfer_statuses ON transfer_statuses.transfer_status_id = transfers.transfer_status_id" +
                          " JOIN transfer_types ON transfer_types.transfer_type_id = transfers.transfer_type_id" +
                         " WHERE account_from = @user_id AND transfers.transfer_status_id = 1", sql);*/
                    sqlCommand.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(GetTransferFromReader(reader));
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }

            return transfers;

        }

        public Transfer SendTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount)" +
                        " VALUES (@transfer_type_id, @transfer_status_id, @account_from, @account_to, @amount)", sql);
                    sqlCommand.Parameters.AddWithValue("@transfer_type_id", transfer.TransferTypeID);
                    sqlCommand.Parameters.AddWithValue("@transfer_status_id", transfer.TransferStatusID);
                    sqlCommand.Parameters.AddWithValue("@account_from", transfer.AccountFrom);
                    sqlCommand.Parameters.AddWithValue("@account_to", transfer.AccountTo);
                    sqlCommand.Parameters.AddWithValue("@amount", transfer.Amount);

                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }

            return transfer;
        }

        public Transfer UpdateTransfer(Transfer transferToUpdate)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(connectionString))
                {
                    sql.Open();

                    SqlCommand sqlCommand = new SqlCommand("UPDATE transfers SET transfer_status_id = @transfer_status_id WHERE transfer_id = @transfer_id", sql);
                    sqlCommand.Parameters.AddWithValue("@transfer_status_id", transferToUpdate.TransferStatusID);
                    sqlCommand.Parameters.AddWithValue("@transfer_id", transferToUpdate.TransferID);
                    int numberOfRowsAffected = sqlCommand.ExecuteNonQuery();

                    if (numberOfRowsAffected > 0)
                    {
                        return transferToUpdate;
                    }
                }
            }
            catch (SqlException)
            {
                //Maybe change later
                throw;
            }
            return null;
        }

        public Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer()
            {
                TransferID = Convert.ToInt32(reader["transfer_id"]),
                TransferType = Convert.ToString(reader["transfer_type_desc"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"]),
                TransferStatusID = Convert.ToInt32(reader["transfer_status_id"]),
                TransferTypeID = Convert.ToInt32(reader["transfer_type_id"]),
                UserFromName = Convert.ToString(reader["userfrom"]),
                UserToName = Convert.ToString(reader["userto"]),
            };
            return transfer;
        }
    }
}
