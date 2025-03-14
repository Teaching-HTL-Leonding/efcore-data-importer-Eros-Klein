namespace EFCoreWriting;

public class Parser
{
    public static async Task<List<Customer>> Parse(string[] lines, DataContext context)
    {
        var customers = new List<Customer>();

        Customer? currentCustomer = null;
        OrderHeader? currentOrderHeader = null;

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if(parts[0] == "CUS")
            {
                if(currentCustomer != null)
                {
                    try{
                        context.Customers.Add(currentCustomer);
                        await context.SaveChangesAsync();
                        customers.Add(currentCustomer);
                    } catch{
                        context.Customers.Remove(currentCustomer);
                    }
                }
                
                currentCustomer = new Customer(0, parts[1], parts[2], parts[3]);
            }
            else if(parts[0] == "OH" && currentCustomer != null)
            {
                var orderDate = DateOnly.TryParse(parts[1], out var date) ? date : new DateOnly();

                currentOrderHeader = new OrderHeader(0, 0, currentCustomer, orderDate, parts[2], parts[3], parts[4]);
                currentCustomer.Orders.Add(currentOrderHeader);
            }
            else if(parts[0] == "OL" && currentOrderHeader != null)
            {
                currentOrderHeader.OrderLines.Add(new OrderLine(0, 0, currentOrderHeader, parts[1], int.Parse(parts[2]), decimal.Parse(parts[3])));
            }
        }

        if(currentCustomer != null)
        {
            try{
                context.Customers.Add(currentCustomer);
                await context.SaveChangesAsync();
                customers.Add(currentCustomer);
            }
            catch {}
        }

        return customers;
    }
}