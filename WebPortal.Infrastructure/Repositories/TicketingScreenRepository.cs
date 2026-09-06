using App.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebPortal.Domain.Interfaces;

namespace WebPortal.Infrastructure.Repositories
{
    public class TicketingScreenRepository : BaseRepository, ITicketingScreenRepository
    {
        public TicketingScreenRepository(string connectionString) : base(connectionString)
        {
        }

        public ScreenModel GetActiveScreen(int branchId, int bankId)
        {
            string query = @"
                SET NOCOUNT ON;

                DECLARE @ScreenID INT;

                SELECT @ScreenID = SCR.ScreenID
                FROM Branches BR
                INNER JOIN Screens SCR
                    ON SCR.BankID = BR.BankID
                    AND SCR.IsActive = 1
                WHERE BR.BranchID = @BranchID
                    AND BR.BankID = @BankID
                    AND BR.IsActive = 1;

                SELECT
                    SCR.ScreenID,
                    SCR.ScreenName,
                    SCR.IsActive,
                    SCR.ModifiedAt,
                    SCR.BankID
                FROM Screens SCR
                WHERE SCR.ScreenID = @ScreenID;

                SELECT
                    B.ButtonID,
                    B.ButtonNameEN,
                    B.ButtonNameAR,
                    B.ButtonType,
                    B.ScreenID,
                    B.ModifiedAt AS ButtonModifiedAt,
                    BT.TypeName,
                    T.TicketID,
                    SVC.ServiceID,
                    SVC.ServiceNameEN,
                    SVC.ServiceNameAR,
                    SVC.MaxTicketsPerDay,
                    SVC.IsActive AS ServiceIsActive,
                    SVC.ModifiedAt AS ServiceModifiedAt,
                    SVC.BankID AS ServiceBankID,
                    SVC.MinimumServiceTime,
                    SVC.MaximumServiceTime
                FROM Buttons B
                INNER JOIN ButtonTypes BT
                    ON BT.TypeID = B.ButtonType
                INNER JOIN Tickets T
                    ON T.ButtonID = B.ButtonID
                INNER JOIN Services SVC
                    ON SVC.ServiceID = T.ServiceID
                WHERE B.ScreenID = @ScreenID
                    AND BT.TypeName = 'Issue Ticket'
                    AND SVC.IsActive = 1
                    AND SVC.BankID = @BankID
                    AND EXISTS
                    (
                        SELECT 1
                        FROM service_allocations SA
                        INNER JOIN Counters C
                            ON C.CounterID = SA.CounterID
                        WHERE SA.ServiceID = SVC.ServiceID
                            AND C.BranchID = @BranchID
                            AND C.IsActive = 1
                    )
                ORDER BY B.ButtonID;

                SELECT
                    B.ButtonID,
                    B.ButtonNameEN,
                    B.ButtonNameAR,
                    B.ButtonType,
                    B.ScreenID,
                    B.ModifiedAt AS ButtonModifiedAt,
                    BT.TypeName,
                    MSG.MessageID,
                    MSG.MessageEN,
                    MSG.MessageAR
                FROM Buttons B
                INNER JOIN ButtonTypes BT
                    ON BT.TypeID = B.ButtonType
                INNER JOIN Messages MSG
                    ON MSG.ButtonID = B.ButtonID
                WHERE B.ScreenID = @ScreenID
                    AND BT.TypeName = 'Show Message'
                ORDER BY B.ButtonID;";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;
                command.Parameters.Add("@BankID", SqlDbType.Int).Value = bankId;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    ScreenModel screen = ReadScreen(reader);

                    if (screen == null)
                    {
                        return null;
                    }

                    if (reader.NextResult())
                    {
                        ReadTicketButtons(reader, screen);
                    }

                    if (reader.NextResult())
                    {
                        ReadMessageButtons(reader, screen);
                    }

                    return screen;
                }
            }
        }

        private static ScreenModel ReadScreen(SqlDataReader reader)
        {
            if (!reader.Read())
            {
                return null;
            }

            return new ScreenModel
            {
                ScreenId = reader.GetInt32(reader.GetOrdinal("ScreenID")),
                ScreenName = reader.GetString(reader.GetOrdinal("ScreenName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                ModifiedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ModifiedAt")),
                BankId = reader.GetInt32(reader.GetOrdinal("BankID")),
                Buttons = new List<ButtonModel>()
            };
        }

        private static void ReadTicketButtons(SqlDataReader reader, ScreenModel screen)
        {
            while (reader.Read())
            {
                ServiceModel service = new ServiceModel
                {
                    ServiceId = reader.GetInt32(reader.GetOrdinal("ServiceID")),
                    ServiceNameEN = reader.GetString(reader.GetOrdinal("ServiceNameEN")),
                    ServiceNameAR = reader.GetString(reader.GetOrdinal("ServiceNameAR")),
                    MaxTicketsPerDay = reader.GetInt32(reader.GetOrdinal("MaxTicketsPerDay")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("ServiceIsActive")),
                    ModifiedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ServiceModifiedAt")),
                    BankId = reader.GetInt32(reader.GetOrdinal("ServiceBankID")),
                    MinimumServiceTime = reader.GetInt32(reader.GetOrdinal("MinimumServiceTime")),
                    MaximumServiceTime = reader.GetInt32(reader.GetOrdinal("MaximumServiceTime"))
                };

                TicketModel ticketButton = new TicketModel
                {
                    TicketId = reader.GetInt32(reader.GetOrdinal("TicketID")),
                    ServiceId = service.ServiceId,
                    Service = service
                };

                MapButton(reader, ticketButton);
                screen.Buttons.Add(ticketButton);
            }
        }

        private static void ReadMessageButtons(SqlDataReader reader, ScreenModel screen)
        {
            while (reader.Read())
            {
                MessageModel messageButton = new MessageModel
                {
                    MessageId = reader.GetInt32(reader.GetOrdinal("MessageID")),
                    MessageEN = reader.GetString(reader.GetOrdinal("MessageEN")),
                    MessageAR = reader.GetString(reader.GetOrdinal("MessageAR"))
                };

                MapButton(reader, messageButton);
                screen.Buttons.Add(messageButton);
            }
        }

        private static void MapButton(SqlDataReader reader, ButtonModel button)
        {
            button.ButtonId = reader.GetInt32(reader.GetOrdinal("ButtonID"));
            button.ButtonNameEN = reader.GetString(reader.GetOrdinal("ButtonNameEN"));
            button.ButtonNameAR = reader.GetString(reader.GetOrdinal("ButtonNameAR"));
            button.ButtonType = reader.GetInt32(reader.GetOrdinal("ButtonType"));
            button.ScreenId = reader.GetInt32(reader.GetOrdinal("ScreenID"));
            button.ModifiedAt = reader.GetDateTimeOffset(reader.GetOrdinal("ButtonModifiedAt"));
            button.TypeName = reader.GetString(reader.GetOrdinal("TypeName"));
        }
    }
}