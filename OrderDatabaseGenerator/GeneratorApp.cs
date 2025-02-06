using System;
namespace OrderDatabaseGenerator;
public class GeneratorApp
{
    private readonly List<Product> products;
    private readonly List<Branch> branches;
    private readonly List<User> customers;
    private readonly List<User> employees;
    private readonly List<PaymentType> payments;
    private readonly List<DeliveryType> deliveries;
    private Random random = new Random();
    private DateTime startDate = new DateTime(2018, 1, 1);

    public GeneratorApp()
    {
        var baseDate = new DateTime(2018, 1, 1); // Nejstarší datum
        var random = new Random(); // Pro generování náhodných rozestupů

        products = File.ReadAllLines("product.data")
            .AdvSelect()
            .Select(x =>
            new Product(
                Id: int.Parse(x[0]),
                Name: x[1],
                Rarity: x[2],
                Season: int.Parse(x[3]),
                Price: decimal.Parse(x[4].Replace('.', ',')),
                StockQuantity: int.Parse(x[5])))
            .ToList();

        branches = File.ReadAllLines("branch.data")
            .AdvSelect()
            .Select(x =>
            new Branch(
                Id: int.Parse(x[0]),
                Name: x[1],
                ManagerId: int.Parse(x[2]),
                CreatedDate: x[3]))
            .ToList();

        var users = File.ReadAllLines("user.data")
            .AdvSelect()
            .Select((x, index) =>
            {
                var randomDays = random.Next(1, 90);
                baseDate = baseDate.AddDays(randomDays); // Zvýšení základního data o náhodný počet dnů

                // Přidání náhodného času
                var randomHours = random.Next(0, 24); // Náhodná hodina (0-23)
                var randomMinutes = random.Next(0, 60); // Náhodná minuta (0-59)
                var randomSeconds = random.Next(0, 60); // Náhodná sekunda (0-59)
                var createdDateTime = baseDate.AddHours(randomHours)
                                              .AddMinutes(randomMinutes)
                                              .AddSeconds(randomSeconds);

                return new User(
                    Id: int.Parse(x[0]),
                    FirstName: x[1],
                    LastName: x[2],
                    Username: x[3],
                    Password: x[4],
                    Email: x[5],
                    Role: x[6],
                    Sex: x[7],
                    BranchId: x[8].Trim() != "null" ? int.Parse(x[8]) : null,
                    CreatedDate: createdDateTime.ToString("yyyy-MM-dd HH:mm:ss")); // Datum i čas
            })
            .ToList();

        employees = users.Where(x => x.Role == "2").ToList();
        customers = users.Where(x => x.Role == "1").ToList();

        payments = File.ReadAllLines("payment.data")
            .AdvSelect()
            .Select(x =>
            new PaymentType(
                Id: int.Parse(x[0]),
                Name: x[1]))
            .ToList();

        deliveries = File.ReadAllLines("delivery.data")
            .AdvSelect()
            .Select(x =>
            new DeliveryType(
                Id: int.Parse(x[0]),
                Name: x[1]))
            .ToList();
    }

    public void Run()
    {
        var orders = new List<Order>();
        var orderProducts = new List<OrderProduct>();
        GenerateUsers(customers, employees);
        GenerateData(orders, orderProducts);
        MapToInsert(orders, orderProducts);
        GenerateEmployees(); // Added method call for generating employee inserts

    }

    private void MapToInsert(List<Order> orders, List<OrderProduct> orderProducts)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            Console.Write($"({order.Id},{order.UserId},{order.EmployeeId},{order.BranchId},'{order.CreatedDate}',{order.TotalAmount.ToString().Replace(',', '.')},{order.TaxRate.ToString().Replace(',', '.')},{order.PaymentTypeId},{order.DeliveryTypeId})");
            if (i < orders.Count - 1)
                Console.WriteLine(",");
            else
                Console.WriteLine();
        }
        Console.WriteLine("OrderProduct");
        for (int i = 0; i < orderProducts.Count; i++)
        {
            var op = orderProducts[i];
            Console.Write($"({op.Id},{op.OrderId},{op.ProductId},{op.Quantity},{op.UnitPrice.ToString().Replace(',', '.')})");
            if (i < orderProducts.Count - 1)
                Console.WriteLine(",");
            else
                Console.WriteLine();
        }
    }
    private void GenerateUsers(List<User> customers, List<User> employees)
    {
        foreach (var user in customers.Concat(employees))
        {
            Console.WriteLine($"({user.Id},'{user.FirstName}','{user.LastName}','{user.Username}','{user.Password}','{user.Email}',{user.Role},'{user.Sex}',{(user.BranchId.HasValue ? user.BranchId.Value.ToString() : "NULL")},'{user.CreatedDate}'),");
        }
        Console.WriteLine();
    }

    private void GenerateData(List<Order> orders, List<OrderProduct> orderProducts)
    {
        var orderProductCounter = 0;
        var currentOrderDate = new DateTime(2024, 1, 1);
        var r = new Random(123456);
        for (int i = 1; i < 134; i++)
        {
            var excepts = new List<Product>();
            var total = 0m;
            for (int j = 0; j < r.Next(1, 9); j++)
            {
                var tmpProducts = products.Except(excepts).ToList();
                var quantity = r.Next(1, 12);
                var currentProduct = tmpProducts[r.Next(0, tmpProducts.Count)];
                orderProducts.Add(new OrderProduct(
                    Id: ++orderProductCounter,
                    OrderId: i,
                    ProductId: currentProduct.Id,
                    Quantity: quantity,
                    UnitPrice: total = quantity * currentProduct.Price
                    ));
            }

            // Náhodné hodiny, minuty a sekundy
            var randomHours = r.Next(0, 24);
            var randomMinutes = r.Next(0, 60);
            var randomSeconds = r.Next(0, 60);

            var empl = employees[r.Next(0, employees.Count)];
            var item = new Order(
                i,
                customers[r.Next(0, customers.Count)].Id,
                empl.Id,
                empl.BranchId!.Value,
                (currentOrderDate = currentOrderDate.AddDays(r.Next(0, 4)))
                    .AddHours(randomHours)
                    .AddMinutes(randomMinutes)
                    .AddSeconds(randomSeconds)
                    .ToString("yyyy-MM-dd HH:mm:ss"), // Přidání času k datu
                total,
                0.21m,
                payments[r.Next(0, payments.Count)].Id,
                deliveries[r.Next(0, deliveries.Count)].Id
                );

            orders.Add(item);
        }
    }
    private void GenerateEmployees()
    {
        var employeeData = File.ReadAllLines("employees.data")
            .Select(x => x.Split(','))
            .Select(x => new
            {
                Id = int.Parse(x[0]),
                Salary = decimal.Parse(x[1]),
                Position = x[2]
            })
            .ToList();
        Console.WriteLine("Employees");
        foreach (var emp in employees)
        {
            var data = employeeData.FirstOrDefault(e => e.Id == emp.Id);
            if (data != null)
            {
                Console.WriteLine($"({data.Id},{data.Salary},{emp.BranchId},'{data.Position}'),");
            }
        }
        Console.WriteLine();
    }
}

public static class Extensions
{
    public static IEnumerable<string[]> AdvSelect(this string[] query)
        => query
        .Select(x => x.TrimStart('(').TrimEnd([')', ',']).Split(','));
}
