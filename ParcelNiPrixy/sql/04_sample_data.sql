-- ============================================================
-- SAMPLE DATA v3
-- ============================================================

USE parcel_tracking_db;

INSERT IGNORE INTO users(username,password,full_name,role)
VALUES('admin','admin123','System Administrator','admin');

INSERT IGNORE INTO sender VALUES
('s001','Velvet Corner','09171234567','velvetcorner@email.com','Blk 3 Lot 5 Rizal St., Meycauayan, Bulacan'),
('s002','Luna Finds','09281234567','lunafinds@email.com','22 Maharlika Highway, Biñan, Laguna'),
('s003','Golden Crate','09391234567','goldencrate@email.com','Unit 4B Eastwood Ave., Quezon City, Metro Manila'),
('s004','Urban Nest','09451234567','urbannest@email.com','100 Katipunan Ave., Quezon City, Metro Manila');

INSERT IGNORE INTO recipient VALUES
('r001','Paul Emil Baclea-an','09121111111','paul.baclea@email.com','12 Sampaguita St., Pandi, Bulacan'),
('r002','Prixy Daniella Delos Santos','09122222222','prixy.delos@email.com','8 Mabini St., Santa Maria, Bulacan'),
('r003','Ma. Alyzza Yvette Policarpio','09123333333','alyzza.p@email.com','45 Bayan-Bayanan Ave., Quezon City, Metro Manila'),
('r004','Marquin Robes','09124444444','marquin.r@email.com','7 Purok 3 Maligaya St., Santa Maria, Bulacan'),
('r005','Mathew Vasquez','09125555555','mathew.v@email.com','99 Timog Ave., Quezon City, Metro Manila');

INSERT IGNORE INTO rider VALUES
('rd001','Jose Manalo','09181111111','jose.m@delivery.com','ABC-1234','Motorcycle','active'),
('rd002','Wally Bayola','09182222222','wally.b@delivery.com','XYZ-5678','Van','active'),
('rd003','Paulo Ballesteros','09183333333','paulo.b@delivery.com','DEF-9012','Motorcycle','active'),
('rd004','Maine Mendoza','09184444444','maine.m@delivery.com','GHI-3456','Bicycle','active');

INSERT IGNORE INTO route VALUES
('rt001','Bulacan','Meycauayan','Bulacan','Pandi'),
('rt002','Laguna','Biñan','Bulacan','Santa Maria'),
('rt003','Metro Manila','Quezon City','Bulacan','Santa Maria'),
('rt004','Metro Manila','Quezon City','Bulacan','Meycauayan');

-- Shipments (1 per parcel)
INSERT IGNORE INTO shipment VALUES
('SH001','rd001','rt001','2026-04-01','2026-04-03','2026-04-01 08:00:00'),
('SH002','rd002','rt002','2026-04-01','2026-04-04','2026-04-01 09:00:00'),
('SH003','rd003','rt003','2026-04-03','2026-04-07','2026-04-03 10:00:00'),
('SH004','rd001','rt004','2026-04-10','2026-04-13','2026-04-10 08:00:00'),
('SH005','rd004','rt003','2026-04-10','2026-04-14','2026-04-10 09:00:00');

-- Parcels (1 per shipment)
INSERT IGNORE INTO parcel VALUES
('PRC001','SH001','s001','r001',1.20,'Handmade candles set','package','2026-04-01 08:15:00'),
('PRC002','SH002','s002','r002',0.30,'Vintage earrings collection','fragile','2026-04-01 09:10:00'),
('PRC003','SH003','s003','r003',2.50,'Office supplies bundle','package','2026-04-03 10:54:00'),
('PRC004','SH004','s001','r004',0.50,'Scented soap set','package','2026-04-10 08:30:00'),
('PRC005','SH005','s004','r005',3.00,'Decorative throw pillows','package','2026-04-10 09:00:00');

INSERT IGNORE INTO payment VALUES
('PAY001','PRC001',40.00,'Cash on Delivery','2026-04-03','paid'),
('PAY002','PRC002',536.67,'GCash','2026-04-04','paid'),
('PAY003','PRC003',189.50,'PayPal',NULL,'pending'),
('PAY004','PRC004',25.00,'Cash on Delivery','2026-04-13','paid'),
('PAY005','PRC005',168.04,'Cash on Delivery',NULL,'pending');

INSERT IGNORE INTO tracking_status VALUES
('TR001','PRC001','preparing to ship',NULL,'2026-04-01 08:15:00','Parcel received at origin'),
('TR002','PRC001','in transit','Meycauayan Hub','2026-04-02 09:00:00',NULL),
('TR003','PRC001','out for delivery','Pandi Hub','2026-04-03 08:00:00',NULL),
('TR004','PRC001','delivered',NULL,'2026-04-03 14:30:00','Received by recipient'),
('TR005','PRC002','preparing to ship',NULL,'2026-04-01 09:10:00','Parcel received at origin'),
('TR006','PRC002','in transit','Biñan Sorting Center','2026-04-02 11:00:00',NULL),
('TR007','PRC002','out for delivery','Santa Maria Hub','2026-04-04 08:00:00',NULL),
('TR008','PRC002','delivered',NULL,'2026-04-04 16:00:00','Received by recipient'),
('TR009','PRC003','preparing to ship',NULL,'2026-04-03 10:54:00','Parcel received at origin'),
('TR010','PRC003','in transit','QC Sorting Center','2026-04-05 09:00:00',NULL),
('TR011','PRC004','preparing to ship',NULL,'2026-04-10 08:30:00','Parcel received at origin'),
('TR012','PRC004','in transit','Meycauayan Hub','2026-04-11 09:00:00',NULL),
('TR013','PRC004','out for delivery','Santa Maria Hub','2026-04-13 08:00:00',NULL),
('TR014','PRC004','delivered',NULL,'2026-04-13 15:00:00','Received by recipient'),
('TR015','PRC005','preparing to ship',NULL,'2026-04-10 09:00:00','Parcel received at origin');
