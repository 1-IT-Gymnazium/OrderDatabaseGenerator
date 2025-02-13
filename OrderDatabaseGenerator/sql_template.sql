-- Create tables without foreign key constraints
CREATE TABLE "Product" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Rarity" VARCHAR(50) NOT NULL,
    "Season" INT NOT NULL,
    "Price" DECIMAL(10, 2) NOT NULL,
    "StockQuantity" INT NOT NULL
);

CREATE TABLE "Role" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL
);

CREATE TABLE "User" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(255) NOT NULL,
    "LastName" VARCHAR(255) NOT NULL,
    "Username" VARCHAR(255) NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "RoleId" INT NOT NULL,
    "Sex" VARCHAR(10) NULL, -- M/F
    "CreatedDate" TIMESTAMP NOT NULL
);

CREATE TABLE "Branch" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "ManagerId" INT NOT NULL,
    "CreatedDate" TIMESTAMP NOT NULL
);

CREATE TABLE "Employee" (
    "Id" INT PRIMARY KEY, -- Matches the "User"."Id"
    "Salary" DECIMAL(10, 2) NOT NULL,
    "BranchId" INT NOT NULL,
    "Position" VARCHAR(255) NULL,
    FOREIGN KEY ("Id") REFERENCES "User"("Id"),
    FOREIGN KEY ("BranchId") REFERENCES "Branch"("Id")
);

CREATE TABLE "PaymentType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL
);

CREATE TABLE "DeliveryType" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL
);

CREATE TABLE "Order" (
    "Id" SERIAL PRIMARY KEY,
    "CustomerId" INT NOT NULL,
    "EmployeeId" INT NOT NULL,
    "BranchId" INT NOT NULL,
    "CreatedDate" TIMESTAMP NOT NULL,
    "TotalAmount" DECIMAL(10, 2) NOT NULL,
    "TaxRate" DECIMAL(5, 2) NOT NULL,
    "PaymentTypeId" INT NOT NULL,
    "DeliveryTypeId" INT NOT NULL
);

CREATE TABLE "OrderProduct" (
    "Id" SERIAL PRIMARY KEY,
    "OrderId" INT NOT NULL,
    "ProductId" INT NOT NULL,
    "Quantity" INT NOT NULL,
    "UnitPrice" DECIMAL(10, 2) NOT NULL
);

-- Placeholder for inserting data
INSERT INTO "Role" ("Id", "Name") VALUES
(1, 'Customer'),
(2, 'Employee'),
(3, 'Manager');

INSERT INTO "Branch" ("Id", "Name", "ManagerId", "CreatedDate") VALUES
(1, 'Prague', 1, '2018.1.1'),
(2, 'New York', 5, '2020-01-01'),
(3, 'Tokyo', 9, '2023-01-01');

INSERT INTO "User" ("Id", "FirstName", "LastName", "Username", "Password", "Email", "RoleId", "Sex", "CreatedDate") VALUES
{{insert_users}};

INSERT INTO "Employee" ("Id", "Salary", "BranchId", "Position") VALUES
{{insert_employees}};

INSERT INTO "PaymentType" ("Id", "Name") VALUES
(1, 'Credit Card'),
(2, 'PayPal'),
(3, 'Bank Transfer'),
(4, 'Cash on Delivery');

INSERT INTO "DeliveryType" ("Id", "Name") VALUES
(1, 'Standard Shipping'),
(2, 'Express Shipping'),
(3, 'In-Store Pickup');

INSERT INTO "Product" ("Id", "Name", "Rarity", "Season", "Price", "StockQuantity") VALUES
(1, 'Starter Box (S1)', 'Common', 1, 9.99, 200),
(2, 'Adventure Box (S1)', 'Uncommon', 1, 14.99, 180),
(3, 'Mystic Box (S1)', 'Rare', 1, 19.99, 150),
(4, 'Epic Box (S1)', 'Epic', 1, 49.99, 75),
(5, 'Legendary Box (S1)', 'Legendary', 1, 99.99, 25),
(6, 'Explorer Box (S2)', 'Common', 2, 12.99, 160),
(7, 'Battle Box (S2)', 'Uncommon', 2, 19.99, 140),
(8, 'Champion Box (S2)', 'Rare', 2, 29.99, 120),
(9, 'Supreme Box (S2)', 'Epic', 2, 59.99, 60),
(10, 'Ultimate Box (S2)', 'Legendary', 2, 109.99, 20),
(11, 'Rookie Box (S3)', 'Common', 3, 11.99, 170),
(12, 'Soldier Box (S3)', 'Uncommon', 3, 17.99, 150),
(13, 'Warrior Box (S3)', 'Rare', 3, 24.99, 100),
(14, 'Guardian Box (S3)', 'Epic', 3, 54.99, 50),
(15, 'Mythical Box (S3)', 'Legendary', 3, 119.99, 15),
(16, 'Initiate Box (S4)', 'Common', 4, 10.99, 190),
(17, 'Explorer’s Cache (S4)', 'Uncommon', 4, 15.99, 170),
(18, 'Ranger Box (S4)', 'Rare', 4, 22.99, 130),
(19, 'Heroic Box (S4)', 'Epic', 4, 51.99, 55),
(20, 'Phoenix Box (S4)', 'Legendary', 4, 105.99, 18),
(21, 'Beginner Box (S5)', 'Common', 5, 13.99, 180),
(22, 'Trekker Box (S5)', 'Uncommon', 5, 18.99, 160),
(23, 'Conqueror Box (S5)', 'Rare', 5, 27.99, 110),
(24, 'Vanquisher Box (S5)', 'Epic', 5, 58.99, 45),
(25, 'Ethereal Box (S5)', 'Legendary', 5, 121.99, 12);

ALTER TABLE "User"
ADD CONSTRAINT "fk_user_role" FOREIGN KEY ("RoleId") REFERENCES "Role"("Id");

ALTER TABLE "Employee"
ADD CONSTRAINT "fk_employee_branch" FOREIGN KEY ("BranchId") REFERENCES "Branch"("Id");

ALTER TABLE "Branch"
ADD CONSTRAINT "fk_branch_manager" FOREIGN KEY ("ManagerId") REFERENCES "Employee"("Id");

ALTER TABLE "Order"
ADD CONSTRAINT "fk_order_user" FOREIGN KEY ("CustomerId") REFERENCES "User"("Id"),
ADD CONSTRAINT "fk_order_employee" FOREIGN KEY ("EmployeeId") REFERENCES "User"("Id"),
ADD CONSTRAINT "fk_order_branch" FOREIGN KEY ("BranchId") REFERENCES "Branch"("Id"),
ADD CONSTRAINT "fk_order_payment" FOREIGN KEY ("PaymentTypeId") REFERENCES "PaymentType"("Id"),
ADD CONSTRAINT "fk_order_delivery" FOREIGN KEY ("DeliveryTypeId") REFERENCES "DeliveryType"("Id");

ALTER TABLE "OrderProduct"
ADD CONSTRAINT "fk_orderproduct_order" FOREIGN KEY ("OrderId") REFERENCES "Order"("Id"),
ADD CONSTRAINT "fk_orderproduct_product" FOREIGN KEY ("ProductId") REFERENCES "Product"("Id");

INSERT INTO "Order" ("Id", "CustomerId", "EmployeeId", "BranchId", "CreatedDate", "TotalAmount", "TaxRate", "PaymentTypeId", "DeliveryTypeId") VALUES
{{insert_orders}};

INSERT INTO "OrderProduct" ("Id", "OrderId", "ProductId", "Quantity", "UnitPrice") VALUES
{{insert_order_products}};