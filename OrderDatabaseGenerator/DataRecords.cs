namespace OrderDatabaseGenerator;
public record Product(
    int Id,
    string Name,
    string Rarity,
    int Season,
    decimal Price,
    int StockQuantity
);

public record User(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password,
    string Role,
    string? Sex, // "Male" or "Female"
    int? BranchId,
    string CreatedDate
);

public record Role(
    int Id,
    string Name
);

public record Branch(
    int Id,
    string Name,
    int ManagerId,
    string CreatedDate
);

public record PaymentType(
    int Id,
    string Name
);

public record DeliveryType(
    int Id,
    string Name
);

public record Order(
    int Id,
    int UserId,
    int EmployeeId,
    int BranchId,
    string CreatedDate,
    decimal TotalAmount,
    decimal TaxRate,
    int PaymentTypeId,
    int DeliveryTypeId
);

public record OrderProduct(
    int Id,
    int OrderId,
    int ProductId,
    int Quantity,
    decimal UnitPrice
);
