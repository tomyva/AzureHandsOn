using AzureHandsOn.Components;
using AzureHandsOn.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<BlobStorageService>();

var connectionString =
    builder.Configuration.GetConnectionString("AzureSql");

await using var connection =
    new SqlConnection(connectionString);

await connection.OpenAsync();

Console.WriteLine("***** CONNECTED TO AZURE SQL *****");

var command = new SqlCommand(
    @"SELECT TicketId,
             CustomerName,
             Subject,
             Description,
             Priority,
             Status,
             CreatedDate
      FROM Tickets",
    connection);

await using var reader = await command.ExecuteReaderAsync();

while (await reader.ReadAsync())
{
    Console.WriteLine(
        $"{reader["TicketId"]} | " +
        $"{reader["CustomerName"]} | " +
        $"{reader["Subject"]} | " +
        $"{reader["Priority"]} | " +
        $"{reader["Status"]}");
}
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
