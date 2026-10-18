-- =============================================================
--  MyStore - SQL Server script (Gift Shop: Van phong pham & Qua luu niem)
--  Chay trong SSMS: File > Open > MyStore.sql > Execute (F5)
-- =============================================================
USE master;
GO
IF DB_ID(N'MyStore') IS NOT NULL
BEGIN
    ALTER DATABASE MyStore SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MyStore;
END
GO
CREATE DATABASE MyStore;
GO
USE MyStore;
GO

CREATE TABLE Category(
    CategoryID   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(300) NULL
);

CREATE TABLE Product(
    ProductID          INT IDENTITY(1,1) PRIMARY KEY,
    ProductName        NVARCHAR(200) NOT NULL,
    ProductDescription NVARCHAR(MAX) NULL,
    ProductImage       NVARCHAR(200) NULL,
    Price              DECIMAL(18,0) NOT NULL,
    CategoryID         INT NOT NULL REFERENCES Category(CategoryID),
    SoldCount          INT NOT NULL DEFAULT 0
);

CREATE TABLE [User](
    Username NVARCHAR(50)  NOT NULL PRIMARY KEY,
    Password NVARCHAR(100) NOT NULL,   -- luu SHA256 (hex, chu hoa)
    UserRole NVARCHAR(20)  NOT NULL DEFAULT N'Customer'
);

CREATE TABLE Customer(
    CustomerID      INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName    NVARCHAR(100) NOT NULL,
    CustomerPhone   NVARCHAR(20)  NOT NULL,
    CustomerEmail   NVARCHAR(100) NOT NULL,
    CustomerAddress NVARCHAR(300) NOT NULL,
    Username        NVARCHAR(50)  NOT NULL REFERENCES [User](Username)
);

CREATE TABLE [Order](
    OrderID         INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID      INT NOT NULL REFERENCES Customer(CustomerID),
    OrderDate       DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount     DECIMAL(18,0) NOT NULL,
    PaymentStatus   NVARCHAR(30)  NOT NULL,
    PaymentMethod   NVARCHAR(50)  NOT NULL,
    DeliveryMethod  NVARCHAR(50)  NOT NULL,
    ShippingAddress NVARCHAR(300) NOT NULL,
    OrderStatus     NVARCHAR(30)  NOT NULL
);

CREATE TABLE OrderDetail(
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID       INT NOT NULL REFERENCES [Order](OrderID),
    ProductID     INT NOT NULL REFERENCES Product(ProductID),
    Quantity      INT NOT NULL,
    UnitPrice     DECIMAL(18,0) NOT NULL
);
ALTER TABLE OrderDetail ADD TotalPrice AS (Quantity * UnitPrice) PERSISTED;
GO

-- ================= DU LIEU MAU =================

INSERT INTO Category(CategoryName,Description) VALUES
(N'Sổ tay',N'Sổ tay, notebook, planner các loại'),
(N'Bút viết',N'Bút bi, bút gel, bút highlight'),
(N'Dụng cụ học tập',N'Thước, tẩy, hộp bút, kẹp giấy'),
(N'Quà lưu niệm',N'Móc khóa, cốc, gối, đồ trang trí'),
(N'Combo quà tặng',N'Bộ quà tặng tiết kiệm theo combo');

INSERT INTO Product(ProductName,ProductDescription,ProductImage,Price,CategoryID,SoldCount) VALUES
(N'Sổ tay bìa da A5',N'Sổ tay bìa da A5 - sản phẩm chất lượng cao thuộc nhóm Sổ tay. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p1.svg',85000,1,41),
(N'Sổ lò xo kẻ ngang B5',N'Sổ lò xo kẻ ngang B5 - sản phẩm chất lượng cao thuộc nhóm Sổ tay. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p2.svg',45000,1,19),
(N'Planner 2026 theo tuần',N'Planner 2026 theo tuần - sản phẩm chất lượng cao thuộc nhóm Sổ tay. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p3.svg',120000,1,50),
(N'Sổ chấm dot grid A5',N'Sổ chấm dot grid A5 - sản phẩm chất lượng cao thuộc nhóm Sổ tay. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p4.svg',68000,1,83),
(N'Sổ tay mini bỏ túi',N'Sổ tay mini bỏ túi - sản phẩm chất lượng cao thuộc nhóm Sổ tay. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p5.svg',25000,1,6),
(N'Sổ vẽ sketchbook A4',N'Sổ vẽ sketchbook A4 - sản phẩm chất lượng cao thuộc nhóm Sổ tay. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p6.svg',95000,1,9),
(N'Bút bi xanh Thiên Long (hộp 20)',N'Bút bi xanh Thiên Long (hộp 20) - sản phẩm chất lượng cao thuộc nhóm Bút viết. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p7.svg',70000,2,105),
(N'Bút gel đen 0.5mm (hộp 12)',N'Bút gel đen 0.5mm (hộp 12) - sản phẩm chất lượng cao thuộc nhóm Bút viết. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p8.svg',54000,2,68),
(N'Bút highlight pastel (bộ 6)',N'Bút highlight pastel (bộ 6) - sản phẩm chất lượng cao thuộc nhóm Bút viết. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p9.svg',58000,2,12),
(N'Bút lông kim 0.3mm',N'Bút lông kim 0.3mm - sản phẩm chất lượng cao thuộc nhóm Bút viết. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p10.svg',18000,2,46),
(N'Bút máy học sinh',N'Bút máy học sinh - sản phẩm chất lượng cao thuộc nhóm Bút viết. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p11.svg',65000,2,74),
(N'Bút chì gỗ 2B (hộp 12)',N'Bút chì gỗ 2B (hộp 12) - sản phẩm chất lượng cao thuộc nhóm Bút viết. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p12.svg',36000,2,7),
(N'Hộp bút vải canvas',N'Hộp bút vải canvas - sản phẩm chất lượng cao thuộc nhóm Dụng cụ học tập. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p13.svg',42000,3,116),
(N'Thước kẻ trong suốt 30cm',N'Thước kẻ trong suốt 30cm - sản phẩm chất lượng cao thuộc nhóm Dụng cụ học tập. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p14.svg',9000,3,64),
(N'Tẩy trắng không bụi',N'Tẩy trắng không bụi - sản phẩm chất lượng cao thuộc nhóm Dụng cụ học tập. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p15.svg',6000,3,27),
(N'Kẹp giấy màu (hộp 100)',N'Kẹp giấy màu (hộp 100) - sản phẩm chất lượng cao thuộc nhóm Dụng cụ học tập. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p16.svg',15000,3,4),
(N'Máy tính cầm tay học sinh',N'Máy tính cầm tay học sinh - sản phẩm chất lượng cao thuộc nhóm Dụng cụ học tập. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p17.svg',189000,3,11),
(N'Băng keo trang trí washi (bộ 10)',N'Băng keo trang trí washi (bộ 10) - sản phẩm chất lượng cao thuộc nhóm Dụng cụ học tập. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p18.svg',49000,3,55),
(N'Móc khóa gấu bông mini',N'Móc khóa gấu bông mini - sản phẩm chất lượng cao thuộc nhóm Quà lưu niệm. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p19.svg',32000,4,53),
(N'Cốc sứ in hình mèo',N'Cốc sứ in hình mèo - sản phẩm chất lượng cao thuộc nhóm Quà lưu niệm. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p20.svg',89000,4,8),
(N'Gối ôm hình thú',N'Gối ôm hình thú - sản phẩm chất lượng cao thuộc nhóm Quà lưu niệm. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p21.svg',145000,4,30),
(N'Hộp nhạc gỗ cổ điển',N'Hộp nhạc gỗ cổ điển - sản phẩm chất lượng cao thuộc nhóm Quà lưu niệm. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p22.svg',175000,4,11),
(N'Khung ảnh để bàn gỗ',N'Khung ảnh để bàn gỗ - sản phẩm chất lượng cao thuộc nhóm Quà lưu niệm. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p23.svg',78000,4,70),
(N'Đèn ngủ mini hình mây',N'Đèn ngủ mini hình mây - sản phẩm chất lượng cao thuộc nhóm Quà lưu niệm. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p24.svg',129000,4,54),
(N'Combo Sổ + Bút học đường',N'Combo Sổ + Bút học đường - sản phẩm chất lượng cao thuộc nhóm Combo quà tặng. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p25.svg',110000,5,7),
(N'Combo Văn phòng công sở',N'Combo Văn phòng công sở - sản phẩm chất lượng cao thuộc nhóm Combo quà tặng. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p26.svg',235000,5,105),
(N'Combo Quà sinh nhật',N'Combo Quà sinh nhật - sản phẩm chất lượng cao thuộc nhóm Combo quà tặng. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p27.svg',199000,5,72),
(N'Combo Back to School',N'Combo Back to School - sản phẩm chất lượng cao thuộc nhóm Combo quà tặng. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p28.svg',260000,5,15),
(N'Combo Planner & Washi tape',N'Combo Planner & Washi tape - sản phẩm chất lượng cao thuộc nhóm Combo quà tặng. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p29.svg',165000,5,28),
(N'Combo Quà tặng thầy cô',N'Combo Quà tặng thầy cô - sản phẩm chất lượng cao thuộc nhóm Combo quà tặng. Thiết kế đẹp, bền, phù hợp làm quà tặng hoặc dùng hằng ngày. Giao hàng toàn quốc, đổi trả trong 7 ngày.',N'p30.svg',220000,5,80);

-- Tai khoan mau: demo / 123456
INSERT INTO [User](Username,Password,UserRole) VALUES (N'demo',N'8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92',N'Customer');
INSERT INTO Customer(CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username)
VALUES (N'Nguyễn Văn Demo',N'0901234567',N'demo@mystore.vn',N'123 Nguyễn Huệ, Quận 1, TP.HCM',N'demo');

INSERT INTO [Order](CustomerID,OrderDate,TotalAmount,PaymentStatus,PaymentMethod,DeliveryMethod,ShippingAddress,OrderStatus) VALUES
(1,DATEADD(day,-10,GETDATE()),155000,N'Paid',N'Chuyển khoản',N'Giao nhanh',N'123 Nguyễn Huệ, Quận 1, TP.HCM',N'Đã giao'),
(1,DATEADD(day,-3,GETDATE()),89000,N'Pending',N'COD',N'Giao tiết kiệm',N'123 Nguyễn Huệ, Quận 1, TP.HCM',N'Đang vận chuyển'),
(1,DATEADD(day,-1,GETDATE()),110000,N'Pending',N'COD',N'Giao nhanh',N'123 Nguyễn Huệ, Quận 1, TP.HCM',N'Đang xử lý');
INSERT INTO OrderDetail(OrderID,ProductID,Quantity,UnitPrice) VALUES
(1,1,1,85000),(1,2,1,45000),(1,5,1,25000),
(2,20,1,89000),
(3,25,1,110000);
GO
SELECT N'Tao CSDL MyStore thanh cong' AS KetQua;
