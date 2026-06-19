-- ============================================================
-- PARCEL TRACKING AND DELIVERY MANAGEMENT SYSTEM v2
-- Schema (3NF) | MySQL Workbench
-- Group 8 | Information Management Final Activity
-- ============================================================

CREATE DATABASE IF NOT EXISTS parcel_tracking_db;
USE parcel_tracking_db;

-- ============================================================
-- TABLE: users
-- ============================================================
CREATE TABLE IF NOT EXISTS users (
    user_id    INT AUTO_INCREMENT PRIMARY KEY,
    username   VARCHAR(50)  NOT NULL UNIQUE,
    password   VARCHAR(255) NOT NULL,
    full_name  VARCHAR(100) NOT NULL,
    role       ENUM('admin') NOT NULL DEFAULT 'admin',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- ============================================================
-- TABLE: sender
-- ============================================================
CREATE TABLE IF NOT EXISTS sender (
    sender_id    VARCHAR(10)  PRIMARY KEY,
    full_name    VARCHAR(100) NOT NULL,
    phone_number VARCHAR(15),
    email        VARCHAR(100),
    address      VARCHAR(255)
);

-- ============================================================
-- TABLE: recipient
-- ============================================================
CREATE TABLE IF NOT EXISTS recipient (
    recipient_id VARCHAR(10)  PRIMARY KEY,
    full_name    VARCHAR(100) NOT NULL,
    phone_number VARCHAR(15),
    email        VARCHAR(100),
    address      VARCHAR(255)
);

-- ============================================================
-- TABLE: rider
-- ============================================================
CREATE TABLE IF NOT EXISTS rider (
    rider_id      VARCHAR(10)  PRIMARY KEY,
    full_name     VARCHAR(100) NOT NULL,
    phone_number  VARCHAR(15),
    email         VARCHAR(100),
    license_plate VARCHAR(20),
    vehicle_type  VARCHAR(50),
    status        ENUM('active','inactive') NOT NULL DEFAULT 'active'
);

-- ============================================================
-- TABLE: route
-- ============================================================
CREATE TABLE IF NOT EXISTS route (
    route_id          VARCHAR(10)  PRIMARY KEY,
    origin_province   VARCHAR(100) NOT NULL,
    origin_municipality VARCHAR(100) NOT NULL,
    dest_province     VARCHAR(100) NOT NULL,
    dest_municipality VARCHAR(100) NOT NULL
);

-- ============================================================
-- TABLE: shipment
-- NOTE: One shipment can have MANY parcels (matches ERD)
-- ============================================================
CREATE TABLE IF NOT EXISTS shipment (
    shipment_id       VARCHAR(20)  PRIMARY KEY,
    rider_id          VARCHAR(10)  NOT NULL,
    route_id          VARCHAR(10)  NOT NULL,
    shipment_date     DATE         NOT NULL,
    estimated_delivery DATE        NOT NULL,
    status            ENUM('pending','in transit','completed','cancelled') NOT NULL DEFAULT 'pending',
    created_at        DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rider_id)  REFERENCES rider(rider_id),
    FOREIGN KEY (route_id)  REFERENCES route(route_id)
);

-- ============================================================
-- TABLE: parcel
-- FK to shipment (many parcels → one shipment)
-- ============================================================
CREATE TABLE IF NOT EXISTS parcel (
    parcel_id    VARCHAR(20)  PRIMARY KEY,
    shipment_id  VARCHAR(20)  NOT NULL,
    sender_id    VARCHAR(10)  NOT NULL,
    recipient_id VARCHAR(10)  NOT NULL,
    weight_kg    DECIMAL(8,2),
    description  VARCHAR(255),
    parcel_type  ENUM('document','package','fragile') DEFAULT 'package',
    created_at   DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (shipment_id)  REFERENCES shipment(shipment_id),
    FOREIGN KEY (sender_id)    REFERENCES sender(sender_id),
    FOREIGN KEY (recipient_id) REFERENCES recipient(recipient_id)
);

-- ============================================================
-- TABLE: tracking_status
-- ============================================================
CREATE TABLE IF NOT EXISTS tracking_status (
    tracking_id  VARCHAR(20)  PRIMARY KEY,
    parcel_id    VARCHAR(20)  NOT NULL,
    status_type  ENUM('preparing to ship','in transit','out for delivery','delivered','failed delivery') NOT NULL,
    location     VARCHAR(100),
    timestamp    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    notes        VARCHAR(255),
    FOREIGN KEY (parcel_id) REFERENCES parcel(parcel_id)
);

-- ============================================================
-- TABLE: payment
-- ============================================================
CREATE TABLE IF NOT EXISTS payment (
    payment_id     VARCHAR(20)  PRIMARY KEY,
    parcel_id      VARCHAR(20)  NOT NULL,
    amount         DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    method         ENUM('Cash on Delivery','GCash','PayPal','Credit Card','Bank Transfer') NOT NULL,
    payment_date   DATE,
    payment_status ENUM('pending','paid','failed') NOT NULL DEFAULT 'pending',
    FOREIGN KEY (parcel_id) REFERENCES parcel(parcel_id)
);

-- ============================================================
-- TABLE: audit_log
-- ============================================================
CREATE TABLE IF NOT EXISTS audit_log (
    log_id      INT AUTO_INCREMENT PRIMARY KEY,
    table_name  VARCHAR(50),
    action      VARCHAR(10),
    record_id   VARCHAR(50),
    change_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    details     TEXT
);

-- ============================================================
-- TABLE: ph_province (PSGC)
-- ============================================================
CREATE TABLE IF NOT EXISTS ph_province (
    province_id   VARCHAR(10) PRIMARY KEY,
    province_name VARCHAR(100) NOT NULL,
    region        VARCHAR(100) NOT NULL
);

-- ============================================================
-- TABLE: ph_municipality (PSGC)
-- ============================================================
CREATE TABLE IF NOT EXISTS ph_municipality (
    municipality_id   VARCHAR(10) PRIMARY KEY,
    municipality_name VARCHAR(100) NOT NULL,
    province_id       VARCHAR(10) NOT NULL,
    FOREIGN KEY (province_id) REFERENCES ph_province(province_id)
);
