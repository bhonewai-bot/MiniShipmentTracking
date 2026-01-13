create table dbo.Tbl_User (
    UserId INT IDENTITY (1, 1) PRIMARY KEY ,
    Name VARCHAR(100) NOT NULL ,
    Email VARCHAR(100) UNIQUE ,
    Password VARCHAR(255) NOT NULL ,
    Role VARCHAR(20) CHECK (Role IN ('Staff', 'Admin')) ,
    CreatedAt DATETIME DEFAULT GETDATE(),
)

create table dbo.Tbl_Login (
    LoginId INT IDENTITY (1, 1) PRIMARY KEY ,
    UserId INT NOT NULL ,
    SessionId VARCHAR(255) NOT NULL ,
    SessionExpiredAt DATETIME NOT NULL ,
    CreatedAt DATETIME DEFAULT GETDATE(),
)

create table dbo.Tbl_Shipment (
    ShipmentId INT IDENTITY (1, 1) PRIMARY KEY ,
    TrackingNo VARCHAR(20) NOT NULL UNIQUE ,
    Origin VARCHAR(50) NOT NULL ,
    Destination VARCHAR(50) NOT NULL ,
    Status VARCHAR(20) CHECK (Status IN ('Created', 'PickedUp', 'InTransit', 'OutForDelivery', 'Delivered')) ,
    UserId INT NOT NULL ,
    CreatedAt DATETIME DEFAULT GETDATE() ,
    UpdatedAt DATETIME NULL
)

create table dbo.TrackingEvent (
    EventId INT IDENTITY (1, 1) PRIMARY KEY ,
    ShipmentId INT NOT NULL ,
    Status VARCHAR(20) CHECK (Status IN ('Created', 'PickedUp', 'InTransit', 'OutForDelivery', 'Delivered')) ,
    Location VARCHAR(100) NOT NULL ,
    Description VARCHAR(MAX) ,
    UpdatedByUserId INT NOT NULL ,
    CreatedAt DATETIME DEFAULT GETDATE() ,
    UpdatedAt DATETIME NULL
)
go