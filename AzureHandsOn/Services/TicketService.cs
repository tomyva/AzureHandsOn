using AzureHandsOn.Models;
using Microsoft.Data.SqlClient;

namespace AzureHandsOn.Services;

public class TicketService
{
    private readonly string _connectionString;

    public TicketService(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("AzureSql")
            ?? throw new InvalidOperationException(
                "AzureSql connection string not found.");
    }

    public async Task AddTicketAsync(Ticket ticket)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        const string sql = """
        INSERT INTO Tickets
        (
            CustomerName,
            Subject,
            Description,
            Priority,
            Status,
            AttachmentFileName,
            AttachmentBlobName
        )
        VALUES
        (
            @CustomerName,
            @Subject,
            @Description,
            @Priority,
            @Status,
            @AttachmentFileName,
            @AttachmentBlobName
        )
        """;

        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@CustomerName", ticket.CustomerName);

        command.Parameters.AddWithValue(
            "@Subject", ticket.Subject);

        command.Parameters.AddWithValue(
            "@Description",
            (object?)ticket.Description ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@Priority", ticket.Priority);

        command.Parameters.AddWithValue(
            "@Status", ticket.Status);

        command.Parameters.AddWithValue(
            "@AttachmentFileName",
            (object?)ticket.AttachmentFileName ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "@AttachmentBlobName",
            (object?)ticket.AttachmentBlobName ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Ticket>> GetTicketsAsync()
    {
        var tickets = new List<Ticket>();

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync();

        var sql = """
            SELECT TicketId,
                   CustomerName,
                   Subject,
                   Description,
                   Priority,
                   Status,
                   CreatedDate
            FROM Tickets
            ORDER BY TicketId
            """;

        await using var command =
            new SqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tickets.Add(new Ticket
            {
                TicketId = reader.GetInt32(0),
                CustomerName = reader.GetString(1),
                Subject = reader.GetString(2),

                Description = reader.IsDBNull(3)
                    ? null
                    : reader.GetString(3),

                Priority = reader.GetString(4),
                Status = reader.GetString(5),
                CreatedDate = reader.GetDateTime(6)
            });
        }

        return tickets;
    }
}