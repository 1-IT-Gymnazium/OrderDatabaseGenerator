using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        var baseDate = new DateTime(2018, 1, 1);
        var random = new Random();

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
                baseDate = baseDate.AddDays(randomDays);
                var createdDateTime = baseDate
                                      .AddHours(random.Next(0, 24))
                                      .AddMinutes(random.Next(0, 60))
                                      .AddSeconds(random.Next(0, 60));

                return new User(
                    Id: int.Parse(x[0]),
                    FirstName: x[1],
                    LastName: x[2],
                    Username: x[3],
                    Password: x[4],
                    Email: x[5],
                    Role: x[6],
                    Sex: x[7],
                    CreatedDate: createdDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
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

        var usersSql = GenerateUsers(customers, employees);
        var employeesSql = GenerateEmployees();
        var (ordersSql, orderProductsSql) = GenerateData(orders, orderProducts);

        ReplaceAndSaveSqlTemplate(usersSql, employeesSql, ordersSql, orderProductsSql);
    }

    private string GenerateUsers(List<User> customers, List<User> employees)
    {
        return string.Join(",\n", customers.Concat(employees)
            .Select(user => $"({user.Id},'{user.FirstName}','{user.LastName}','{user.Username}','{user.Password}','{user.Email}',{user.Role},'{user.Sex}','{user.CreatedDate}')"));
    }

    private (string, string) GenerateData(List<Order> orders, List<OrderProduct> orderProducts)
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

            var randomHours = r.Next(0, 24);
            var randomMinutes = r.Next(0, 60);
            var randomSeconds = r.Next(0, 60);

            var empl = employees[r.Next(0, employees.Count)];
            int branchId = branches[(empl.Id - 1) % branches.Count].Id;

            orders.Add(new Order(
                i,
                customers[r.Next(0, customers.Count)].Id,
                empl.Id,
                branchId,
                (currentOrderDate = currentOrderDate.AddDays(r.Next(0, 4)))
                    .AddHours(randomHours)
                    .AddMinutes(randomMinutes)
                    .AddSeconds(randomSeconds)
                    .ToString("yyyy-MM-dd HH:mm:ss"),
                total,
                0.21m,
                payments[r.Next(0, payments.Count)].Id,
                deliveries[r.Next(0, deliveries.Count)].Id
            ));
        }

        var ordersSql = string.Join(",\n", orders.Select(o => $"({o.Id},{o.UserId},{o.EmployeeId},{o.BranchId},'{o.CreatedDate}',{o.TotalAmount},{o.TaxRate},{o.PaymentTypeId},{o.DeliveryTypeId})"));
        var orderProductsSql = string.Join(",\n", orderProducts.Select(op => $"({op.Id},{op.OrderId},{op.ProductId},{op.Quantity},{op.UnitPrice})"));

        return (ordersSql, orderProductsSql);
    }

    private string GenerateEmployees()
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

        int branchCount = branches.Count;
        int employeesPerBranch = (int)Math.Ceiling((double)employees.Count / branchCount);

        return string.Join(",\n", employees.Select((emp, i) =>
        {
            var data = employeeData.FirstOrDefault(e => e.Id == emp.Id);
            if (data != null)
            {
                int branchId = branches[(i / employeesPerBranch) % branchCount].Id;
                return $"({data.Id},{data.Salary},{branchId},'{data.Position}')";
            }
            return null;
        }).Where(x => x != null));
    }

    private void ReplaceAndSaveSqlTemplate(string usersSql, string employeesSql, string ordersSql, string orderProductsSql)
    {
        string sqlTemplate = File.ReadAllText("sql_template.sql");
        sqlTemplate = sqlTemplate.Replace("{{insert_users}}", usersSql)
                                 .Replace("{{insert_employees}}", employeesSql)
                                 .Replace("{{insert_orders}}", ordersSql)
                                 .Replace("{{insert_order_products}}", orderProductsSql);

        File.WriteAllText("db_Orders_version.sql", sqlTemplate);
        Console.WriteLine("Ulozeno do db_Orders_version.sql");
    }
}

public static class Extensions
{
    public static IEnumerable<string[]> AdvSelect(this string[] query)
        => query.Select(x => x.TrimStart('(').TrimEnd([')', ',']).Split(','));
}
