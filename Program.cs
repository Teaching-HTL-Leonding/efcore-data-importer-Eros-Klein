using EFCoreWriting;
using Microsoft.EntityFrameworkCore;


var factory = new DataContextFactory();
var context = factory.CreateDbContext(args: []);

await context.OrderHeaders.ExecuteDeleteAsync();
await context.OrderLines.ExecuteDeleteAsync();
await context.Customers.ExecuteDeleteAsync();

await Parser.Parse(await File.ReadAllLinesAsync("data/data.txt"), context);