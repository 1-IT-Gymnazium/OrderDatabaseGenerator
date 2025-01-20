
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
    "Sex" VARCHAR(10) NULL,
    "BranchId" INT,
    "CreatedDate" TIMESTAMP NOT NULL
);

CREATE TABLE "Branch" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "ManagerId" INT NOT NULL,
    "CreatedDate" TIMESTAMP NOT NULL
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
    "UserId" INT NOT NULL,
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
{{insert_roles}};

INSERT INTO "Branch" ("Id", "Name", "ManagerId", "CreatedDate") VALUES
{{insert_branches}};

INSERT INTO "User" ("Id", "FirstName", "LastName", "Username", "Password", "Email", "RoleId", "Sex", "BranchId", "CreatedDate") VALUES
{{insert_users}};

INSERT INTO "PaymentType" ("Id", "Name") VALUES
{{insert_payment_types}};

INSERT INTO "DeliveryType" ("Id", "Name") VALUES
{{insert_delivery_types}};

INSERT INTO "Product" ("Id", "Name", "Rarity", "Season", "Price", "StockQuantity") VALUES
{{insert_products}};

ALTER TABLE "User"
ADD CONSTRAINT "fk_user_role" FOREIGN KEY ("RoleId") REFERENCES "Role"("Id"),
ADD CONSTRAINT "fk_user_branch" FOREIGN KEY ("BranchId") REFERENCES "Branch"("Id");

ALTER TABLE "Branch"
ADD CONSTRAINT "fk_branch_manager" FOREIGN KEY ("ManagerId") REFERENCES "User"("Id");

ALTER TABLE "Order"
ADD CONSTRAINT "fk_order_user" FOREIGN KEY ("UserId") REFERENCES "User"("Id"),
ADD CONSTRAINT "fk_order_employee" FOREIGN KEY ("EmployeeId") REFERENCES "User"("Id"),
ADD CONSTRAINT "fk_order_branch" FOREIGN KEY ("BranchId") REFERENCES "Branch"("Id"),
ADD CONSTRAINT "fk_order_payment" FOREIGN KEY ("PaymentTypeId") REFERENCES "PaymentType"("Id"),
ADD CONSTRAINT "fk_order_delivery" FOREIGN KEY ("DeliveryTypeId") REFERENCES "DeliveryType"("Id");

ALTER TABLE "OrderProduct"
ADD CONSTRAINT "fk_orderproduct_order" FOREIGN KEY ("OrderId") REFERENCES "Order"("Id"),
ADD CONSTRAINT "fk_orderproduct_product" FOREIGN KEY ("ProductId") REFERENCES "Product"("Id");

INSERT INTO "Order" ("Id", "UserId", "EmployeeId", "BranchId", "CreatedDate", "TotalAmount", "TaxRate", "PaymentTypeId", "DeliveryTypeId") VALUES
{{insert_orders}};

INSERT INTO "OrderProduct" ("Id", "OrderId", "ProductId", "Quantity", "UnitPrice") VALUES
{{insert_order_products}};
