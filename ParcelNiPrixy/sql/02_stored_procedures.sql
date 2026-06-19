-- ============================================================
-- STORED PROCEDURES v3
-- 1 Parcel = 1 Shipment
-- ============================================================

USE parcel_tracking_db;
DELIMITER $$

-- ── AUTH ─────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_login$$
CREATE PROCEDURE sp_login(
    IN  p_username VARCHAR(50),
    IN  p_password VARCHAR(255),
    OUT p_success  TINYINT,
    OUT p_fullname VARCHAR(100)
)
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*), MAX(full_name) INTO v_count, p_fullname
    FROM users WHERE username=p_username AND password=p_password;
    SET p_success = IF(v_count>0,1,0);
    IF p_success=0 THEN SET p_fullname=NULL; END IF;
END$$

-- ── PSGC ─────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_provinces$$
CREATE PROCEDURE sp_get_provinces()
BEGIN
    SELECT province_id, province_name, region
    FROM ph_province ORDER BY region, province_name;
END$$

DROP PROCEDURE IF EXISTS sp_get_municipalities$$
CREATE PROCEDURE sp_get_municipalities(IN p_province_id VARCHAR(10))
BEGIN
    SELECT municipality_id, municipality_name
    FROM ph_municipality
    WHERE province_id=p_province_id ORDER BY municipality_name;
END$$

-- ── SENDER ───────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_all_senders$$
CREATE PROCEDURE sp_get_all_senders()
BEGIN SELECT * FROM sender ORDER BY full_name; END$$

DROP PROCEDURE IF EXISTS sp_search_senders$$
CREATE PROCEDURE sp_search_senders(IN p_query VARCHAR(100))
BEGIN
    SELECT * FROM sender
    WHERE full_name LIKE CONCAT('%',p_query,'%')
       OR phone_number LIKE CONCAT('%',p_query,'%')
       OR email LIKE CONCAT('%',p_query,'%')
    ORDER BY full_name LIMIT 10;
END$$

DROP PROCEDURE IF EXISTS sp_get_sender_by_id$$
CREATE PROCEDURE sp_get_sender_by_id(IN p_id VARCHAR(20))
BEGIN SELECT * FROM sender WHERE sender_id=p_id; END$$

DROP PROCEDURE IF EXISTS sp_create_sender$$
CREATE PROCEDURE sp_create_sender(
    IN p_id VARCHAR(20), IN p_name VARCHAR(100),
    IN p_phone VARCHAR(15), IN p_email VARCHAR(100), IN p_address VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        INSERT INTO sender(sender_id,full_name,phone_number,email,address)
        VALUES(p_id,p_name,p_phone,p_email,p_address);
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_update_sender$$
CREATE PROCEDURE sp_update_sender(
    IN p_id VARCHAR(20), IN p_name VARCHAR(100),
    IN p_phone VARCHAR(15), IN p_email VARCHAR(100), IN p_address VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        UPDATE sender SET full_name=p_name,phone_number=p_phone,
            email=p_email,address=p_address WHERE sender_id=p_id;
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_delete_sender$$
CREATE PROCEDURE sp_delete_sender(IN p_id VARCHAR(20))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION; DELETE FROM sender WHERE sender_id=p_id; COMMIT;
END$$

-- ── RECIPIENT ─────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_all_recipients$$
CREATE PROCEDURE sp_get_all_recipients()
BEGIN SELECT * FROM recipient ORDER BY full_name; END$$

DROP PROCEDURE IF EXISTS sp_search_recipients$$
CREATE PROCEDURE sp_search_recipients(IN p_query VARCHAR(100))
BEGIN
    SELECT * FROM recipient
    WHERE full_name LIKE CONCAT('%',p_query,'%')
       OR phone_number LIKE CONCAT('%',p_query,'%')
       OR email LIKE CONCAT('%',p_query,'%')
    ORDER BY full_name LIMIT 10;
END$$

DROP PROCEDURE IF EXISTS sp_get_recipient_by_id$$
CREATE PROCEDURE sp_get_recipient_by_id(IN p_id VARCHAR(20))
BEGIN SELECT * FROM recipient WHERE recipient_id=p_id; END$$

DROP PROCEDURE IF EXISTS sp_create_recipient$$
CREATE PROCEDURE sp_create_recipient(
    IN p_id VARCHAR(20), IN p_name VARCHAR(100),
    IN p_phone VARCHAR(15), IN p_email VARCHAR(100), IN p_address VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        INSERT INTO recipient(recipient_id,full_name,phone_number,email,address)
        VALUES(p_id,p_name,p_phone,p_email,p_address);
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_update_recipient$$
CREATE PROCEDURE sp_update_recipient(
    IN p_id VARCHAR(20), IN p_name VARCHAR(100),
    IN p_phone VARCHAR(15), IN p_email VARCHAR(100), IN p_address VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        UPDATE recipient SET full_name=p_name,phone_number=p_phone,
            email=p_email,address=p_address WHERE recipient_id=p_id;
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_delete_recipient$$
CREATE PROCEDURE sp_delete_recipient(IN p_id VARCHAR(20))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION; DELETE FROM recipient WHERE recipient_id=p_id; COMMIT;
END$$

-- ── RIDER ─────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_all_riders$$
CREATE PROCEDURE sp_get_all_riders()
BEGIN SELECT * FROM rider ORDER BY full_name; END$$

DROP PROCEDURE IF EXISTS sp_get_active_riders$$
CREATE PROCEDURE sp_get_active_riders()
BEGIN SELECT * FROM rider WHERE status='active' ORDER BY full_name; END$$

DROP PROCEDURE IF EXISTS sp_create_rider$$
CREATE PROCEDURE sp_create_rider(
    IN p_id VARCHAR(20), IN p_name VARCHAR(100), IN p_phone VARCHAR(15),
    IN p_email VARCHAR(100), IN p_license VARCHAR(20),
    IN p_vehicle VARCHAR(50), IN p_status VARCHAR(10))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        INSERT INTO rider(rider_id,full_name,phone_number,email,license_plate,vehicle_type,status)
        VALUES(p_id,p_name,p_phone,p_email,p_license,p_vehicle,p_status);
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_update_rider$$
CREATE PROCEDURE sp_update_rider(
    IN p_id VARCHAR(20), IN p_name VARCHAR(100), IN p_phone VARCHAR(15),
    IN p_email VARCHAR(100), IN p_license VARCHAR(20),
    IN p_vehicle VARCHAR(50), IN p_status VARCHAR(10))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        UPDATE rider SET full_name=p_name,phone_number=p_phone,email=p_email,
            license_plate=p_license,vehicle_type=p_vehicle,status=p_status
        WHERE rider_id=p_id;
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_delete_rider$$
CREATE PROCEDURE sp_delete_rider(IN p_id VARCHAR(20))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION; DELETE FROM rider WHERE rider_id=p_id; COMMIT;
END$$

-- ── ROUTE ─────────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_all_routes$$
CREATE PROCEDURE sp_get_all_routes()
BEGIN
    SELECT *,
        CONCAT(origin_municipality,', ',origin_province) AS origin_display,
        CONCAT(dest_municipality,', ',dest_province) AS dest_display
    FROM route ORDER BY origin_province;
END$$

-- ── PARCEL (master transaction: creates everything at once) ───
DROP PROCEDURE IF EXISTS sp_create_parcel_full$$
CREATE PROCEDURE sp_create_parcel_full(
    IN p_parcel_id           VARCHAR(20),
    IN p_weight_kg           DECIMAL(8,2),
    IN p_description         VARCHAR(255),
    IN p_parcel_type         VARCHAR(20),
    IN p_sender_id           VARCHAR(20),
    IN p_sender_name         VARCHAR(100),
    IN p_sender_phone        VARCHAR(15),
    IN p_sender_email        VARCHAR(100),
    IN p_sender_address      VARCHAR(255),
    IN p_sender_is_new       TINYINT,
    IN p_recipient_id        VARCHAR(20),
    IN p_recipient_name      VARCHAR(100),
    IN p_recipient_phone     VARCHAR(15),
    IN p_recipient_email     VARCHAR(100),
    IN p_recipient_address   VARCHAR(255),
    IN p_recipient_is_new    TINYINT,
    IN p_origin_province     VARCHAR(100),
    IN p_origin_municipality VARCHAR(100),
    IN p_dest_province       VARCHAR(100),
    IN p_dest_municipality   VARCHAR(100),
    IN p_rider_id            VARCHAR(20),
    IN p_ship_date           DATE,
    IN p_est_delivery        DATE,
    IN p_amount              DECIMAL(10,2),
    IN p_method              VARCHAR(30),
    OUT p_shipment_id        VARCHAR(20),
    OUT p_route_id           VARCHAR(20)
)
BEGIN
    DECLARE v_route_id    VARCHAR(20);
    DECLARE v_shipment_id VARCHAR(20);
    DECLARE v_pay_id      VARCHAR(20);
    DECLARE v_track_id    VARCHAR(20);

    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;

        -- 1. Create sender if new
        IF p_sender_is_new=1 THEN
            INSERT INTO sender(sender_id,full_name,phone_number,email,address)
            VALUES(p_sender_id,p_sender_name,p_sender_phone,p_sender_email,p_sender_address);
        END IF;

        -- 2. Create recipient if new
        IF p_recipient_is_new=1 THEN
            INSERT INTO recipient(recipient_id,full_name,phone_number,email,address)
            VALUES(p_recipient_id,p_recipient_name,p_recipient_phone,p_recipient_email,p_recipient_address);
        END IF;

        -- 3. Find or create route
        SELECT route_id INTO v_route_id FROM route
        WHERE origin_province=p_origin_province
          AND origin_municipality=p_origin_municipality
          AND dest_province=p_dest_province
          AND dest_municipality=p_dest_municipality
        LIMIT 1;

        IF v_route_id IS NULL THEN
            SET v_route_id = CONCAT('RT',DATE_FORMAT(NOW(),'%Y%m%d%H%i%s'));
            INSERT INTO route(route_id,origin_province,origin_municipality,dest_province,dest_municipality)
            VALUES(v_route_id,p_origin_province,p_origin_municipality,p_dest_province,p_dest_municipality);
        END IF;
        SET p_route_id = v_route_id;

        -- 4. Create shipment (1 per parcel)
        SET v_shipment_id = CONCAT('SH',DATE_FORMAT(NOW(),'%Y%m%d%H%i%s'));
        INSERT INTO shipment(shipment_id,rider_id,route_id,shipment_date,estimated_delivery)
        VALUES(v_shipment_id,p_rider_id,v_route_id,p_ship_date,p_est_delivery);
        SET p_shipment_id = v_shipment_id;

        -- 5. Create parcel
        INSERT INTO parcel(parcel_id,shipment_id,sender_id,recipient_id,weight_kg,description,parcel_type)
        VALUES(p_parcel_id,v_shipment_id,p_sender_id,p_recipient_id,p_weight_kg,p_description,p_parcel_type);

        -- 6. Create payment
        SET v_pay_id = CONCAT('PAY',DATE_FORMAT(NOW(),'%Y%m%d%H%i%s'));
        INSERT INTO payment(payment_id,parcel_id,amount,method,payment_status)
        VALUES(v_pay_id,p_parcel_id,p_amount,p_method,'pending');

        -- 7. Initial tracking status
        SET v_track_id = CONCAT('TR',DATE_FORMAT(NOW(),'%Y%m%d%H%i%s'));
        INSERT INTO tracking_status(tracking_id,parcel_id,status_type,timestamp,notes)
        VALUES(v_track_id,p_parcel_id,'preparing to ship',NOW(),'Parcel received and logged');

    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_get_all_parcels$$
CREATE PROCEDURE sp_get_all_parcels()
BEGIN
    SELECT
        p.parcel_id, p.weight_kg, p.description, p.parcel_type, p.created_at,
        s.full_name   AS sender_name,   s.phone_number AS sender_phone,
        r.full_name   AS recipient_name, r.phone_number AS recipient_phone,
        r.address     AS recipient_address,
        rd.full_name  AS rider_name,
        CONCAT(rt.origin_municipality,', ',rt.origin_province) AS origin,
        CONCAT(rt.dest_municipality,', ',rt.dest_province)     AS destination,
        sh.shipment_id, sh.shipment_date, sh.estimated_delivery,
        py.payment_id, py.amount, py.method, py.payment_status,
        (SELECT ts.status_type FROM tracking_status ts
         WHERE ts.parcel_id=p.parcel_id
         ORDER BY ts.timestamp DESC LIMIT 1) AS current_status
    FROM parcel p
    JOIN sender    s  ON p.sender_id    = s.sender_id
    JOIN recipient r  ON p.recipient_id = r.recipient_id
    JOIN shipment  sh ON p.shipment_id  = sh.shipment_id
    JOIN rider     rd ON sh.rider_id    = rd.rider_id
    JOIN route     rt ON sh.route_id    = rt.route_id
    LEFT JOIN payment py ON py.parcel_id = p.parcel_id
    ORDER BY p.created_at DESC;
END$$

DROP PROCEDURE IF EXISTS sp_get_parcel_by_id$$
CREATE PROCEDURE sp_get_parcel_by_id(IN p_id VARCHAR(20))
BEGIN
    SELECT p.*, s.full_name AS sender_name, s.phone_number AS sender_phone,
        s.email AS sender_email, s.address AS sender_address,
        r.full_name AS recipient_name, r.phone_number AS recipient_phone,
        r.email AS recipient_email, r.address AS recipient_address
    FROM parcel p
    JOIN sender s ON p.sender_id=s.sender_id
    JOIN recipient r ON p.recipient_id=r.recipient_id
    WHERE p.parcel_id=p_id;
END$$

DROP PROCEDURE IF EXISTS sp_update_parcel$$
CREATE PROCEDURE sp_update_parcel(
    IN p_parcel_id   VARCHAR(20),
    IN p_weight_kg   DECIMAL(8,2),
    IN p_description VARCHAR(255),
    IN p_parcel_type VARCHAR(20))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        UPDATE parcel SET weight_kg=p_weight_kg,description=p_description,
            parcel_type=p_parcel_type WHERE parcel_id=p_parcel_id;
    COMMIT;
END$$

DROP PROCEDURE IF EXISTS sp_delete_parcel$$
CREATE PROCEDURE sp_delete_parcel(IN p_parcel_id VARCHAR(20))
BEGIN
    DECLARE v_shipment_id VARCHAR(20);
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        SELECT shipment_id INTO v_shipment_id FROM parcel WHERE parcel_id=p_parcel_id;
        DELETE FROM tracking_status WHERE parcel_id=p_parcel_id;
        DELETE FROM payment WHERE parcel_id=p_parcel_id;
        DELETE FROM parcel WHERE parcel_id=p_parcel_id;
        DELETE FROM shipment WHERE shipment_id=v_shipment_id;
    COMMIT;
END$$

-- ── TRACKING ──────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_tracking_by_parcel$$
CREATE PROCEDURE sp_get_tracking_by_parcel(IN p_parcel_id VARCHAR(20))
BEGIN
    SELECT ts.*,
        s.full_name  AS sender_name,
        r.full_name  AS recipient_name,
        rd.full_name AS rider_name,
        CONCAT(rt.origin_municipality,', ',rt.origin_province) AS origin,
        CONCAT(rt.dest_municipality,', ',rt.dest_province)     AS destination
    FROM tracking_status ts
    JOIN parcel    p  ON ts.parcel_id   = p.parcel_id
    JOIN sender    s  ON p.sender_id    = s.sender_id
    JOIN recipient r  ON p.recipient_id = r.recipient_id
    JOIN shipment  sh ON p.shipment_id  = sh.shipment_id
    JOIN rider     rd ON sh.rider_id    = rd.rider_id
    JOIN route     rt ON sh.route_id    = rt.route_id
    WHERE ts.parcel_id=p_parcel_id
    ORDER BY ts.timestamp ASC;
END$$

DROP PROCEDURE IF EXISTS sp_add_tracking_update$$
CREATE PROCEDURE sp_add_tracking_update(
    IN p_parcel_id VARCHAR(20),
    IN p_status    VARCHAR(30),
    IN p_location  VARCHAR(100),
    IN p_notes     VARCHAR(255))
BEGIN
    DECLARE v_track_id VARCHAR(20);
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        SET v_track_id=CONCAT('TR',DATE_FORMAT(NOW(),'%Y%m%d%H%i%s%f'));
        INSERT INTO tracking_status(tracking_id,parcel_id,status_type,location,timestamp,notes)
        VALUES(v_track_id,p_parcel_id,p_status,p_location,NOW(),p_notes);
        IF p_status='delivered' THEN
            UPDATE payment SET payment_status='paid',payment_date=CURDATE()
            WHERE parcel_id=p_parcel_id AND payment_status='pending';
        END IF;
    COMMIT;
END$$

-- ── PAYMENT ───────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_all_payments$$
CREATE PROCEDURE sp_get_all_payments()
BEGIN
    SELECT py.*, s.full_name AS sender_name, r.full_name AS recipient_name
    FROM payment py
    JOIN parcel    p ON py.parcel_id   = p.parcel_id
    JOIN sender    s ON p.sender_id    = s.sender_id
    JOIN recipient r ON p.recipient_id = r.recipient_id
    ORDER BY py.payment_id DESC;
END$$

DROP PROCEDURE IF EXISTS sp_update_payment$$
CREATE PROCEDURE sp_update_payment(
    IN p_payment_id VARCHAR(20),
    IN p_amount     DECIMAL(10,2),
    IN p_method     VARCHAR(30),
    IN p_status     VARCHAR(10),
    IN p_date       DATE)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    START TRANSACTION;
        UPDATE payment SET amount=p_amount,method=p_method,
            payment_status=p_status,payment_date=p_date
        WHERE payment_id=p_payment_id;
    COMMIT;
END$$

-- ── DASHBOARD ─────────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_get_dashboard_summary$$
CREATE PROCEDURE sp_get_dashboard_summary()
BEGIN
    SELECT
        (SELECT COUNT(*) FROM parcel) AS total_parcels,
        (SELECT COUNT(*) FROM rider WHERE status='active') AS active_riders,
        (SELECT COUNT(*) FROM tracking_status
            WHERE status_type='delivered' AND DATE(timestamp)=CURDATE()) AS delivered_today,
        (SELECT COUNT(*) FROM tracking_status ts
            INNER JOIN (SELECT parcel_id, MAX(timestamp) AS mx FROM tracking_status GROUP BY parcel_id) latest
            ON ts.parcel_id=latest.parcel_id AND ts.timestamp=latest.mx
            WHERE ts.status_type='in transit') AS in_transit,
        (SELECT IFNULL(SUM(amount),0) FROM payment WHERE payment_status='paid') AS total_revenue;
END$$

DELIMITER ;
