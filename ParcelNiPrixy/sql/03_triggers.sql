-- ============================================================
-- TRIGGERS v2
-- Parcel Tracking and Delivery Management System
-- ============================================================

USE parcel_tracking_db;
DELIMITER $$

-- ============================================================
-- TRIGGER 1: Validate parcel_id not empty
-- ============================================================
DROP TRIGGER IF EXISTS trg_before_insert_parcel$$
CREATE TRIGGER trg_before_insert_parcel
BEFORE INSERT ON parcel FOR EACH ROW
BEGIN
    IF NEW.parcel_id IS NULL OR TRIM(NEW.parcel_id) = '' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Parcel ID cannot be empty.';
    END IF;
END$$

-- ============================================================
-- TRIGGER 2: Audit log on parcel INSERT
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_insert_parcel$$
CREATE TRIGGER trg_after_insert_parcel
AFTER INSERT ON parcel FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name, action, record_id, details)
    VALUES('parcel','INSERT', NEW.parcel_id,
        CONCAT('New parcel created | Shipment: ', NEW.shipment_id,
               ' | Sender: ', NEW.sender_id,
               ' | Recipient: ', NEW.recipient_id,
               ' | Type: ', NEW.parcel_type));
END$$

-- ============================================================
-- TRIGGER 3: Audit log on parcel UPDATE
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_update_parcel$$
CREATE TRIGGER trg_after_update_parcel
AFTER UPDATE ON parcel FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name, action, record_id, details)
    VALUES('parcel','UPDATE', NEW.parcel_id,
        CONCAT('Parcel updated | Weight: ', IFNULL(NEW.weight_kg,'N/A'),
               ' | Type: ', NEW.parcel_type,
               ' | Description: ', IFNULL(NEW.description,'N/A')));
END$$

-- ============================================================
-- TRIGGER 4: Audit log on parcel DELETE
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_delete_parcel$$
CREATE TRIGGER trg_after_delete_parcel
AFTER DELETE ON parcel FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name, action, record_id, details)
    VALUES('parcel','DELETE', OLD.parcel_id,
        CONCAT('Parcel deleted | Was in shipment: ', OLD.shipment_id,
               ' | Recipient: ', OLD.recipient_id));
END$$

-- ============================================================
-- TRIGGER 5: Prevent estimated_delivery before shipment_date
-- ============================================================
DROP TRIGGER IF EXISTS trg_before_insert_shipment$$
CREATE TRIGGER trg_before_insert_shipment
BEFORE INSERT ON shipment FOR EACH ROW
BEGIN
    IF NEW.estimated_delivery < NEW.shipment_date THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Estimated delivery date cannot be before shipment date.';
    END IF;
END$$

-- ============================================================
-- TRIGGER 6: Prevent same rider double-booking on same date
-- ============================================================
DROP TRIGGER IF EXISTS trg_before_insert_shipment_rider$$
CREATE TRIGGER trg_before_insert_shipment_rider
BEFORE INSERT ON shipment FOR EACH ROW
BEGIN
    DECLARE v_count INT DEFAULT 0;
    SELECT COUNT(*) INTO v_count FROM shipment
    WHERE rider_id = NEW.rider_id
      AND shipment_date = NEW.shipment_date
      AND status IN ('pending','in transit');

    IF v_count > 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'This rider already has an active shipment on the selected date. Please choose a different rider or date.';
    END IF;
END$$

-- ============================================================
-- TRIGGER 7: Audit log on shipment INSERT
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_insert_shipment$$
CREATE TRIGGER trg_after_insert_shipment
AFTER INSERT ON shipment FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name, action, record_id, details)
    VALUES('shipment','INSERT', NEW.shipment_id,
        CONCAT('New shipment created | Rider: ', NEW.rider_id,
               ' | Route: ', NEW.route_id,
               ' | Date: ', NEW.shipment_date));
END$$

-- ============================================================
-- TRIGGER 8: Audit log on shipment status UPDATE
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_update_shipment$$
CREATE TRIGGER trg_after_update_shipment
AFTER UPDATE ON shipment FOR EACH ROW
BEGIN
    IF OLD.status <> NEW.status THEN
        INSERT INTO audit_log(table_name, action, record_id, details)
        VALUES('shipment','UPDATE', NEW.shipment_id,
            CONCAT('Shipment status changed: ', OLD.status, ' → ', NEW.status));
    END IF;
END$$

-- ============================================================
-- TRIGGER 9: Prevent status reversal on delivered parcels
-- ============================================================
DROP TRIGGER IF EXISTS trg_before_insert_tracking$$
CREATE TRIGGER trg_before_insert_tracking
BEFORE INSERT ON tracking_status FOR EACH ROW
BEGIN
    DECLARE v_last_status VARCHAR(30);
    SELECT status_type INTO v_last_status
    FROM tracking_status
    WHERE parcel_id = NEW.parcel_id
    ORDER BY timestamp DESC LIMIT 1;

    IF v_last_status = 'delivered' THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Cannot update: parcel is already marked as delivered.';
    END IF;

    IF v_last_status = 'failed delivery'
       AND NEW.status_type NOT IN ('in transit','out for delivery') THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'After a failed delivery, status must be set to in transit or out for delivery.';
    END IF;
END$$

-- ============================================================
-- TRIGGER 10: Audit log on tracking INSERT
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_insert_tracking$$
CREATE TRIGGER trg_after_insert_tracking
AFTER INSERT ON tracking_status FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name, action, record_id, details)
    VALUES('tracking_status','INSERT', NEW.tracking_id,
        CONCAT('Parcel ', NEW.parcel_id, ' → status: ', NEW.status_type,
               IFNULL(CONCAT(' | Location: ', NEW.location), '')));
END$$

-- ============================================================
-- TRIGGER 11: Audit log on payment UPDATE
-- ============================================================
DROP TRIGGER IF EXISTS trg_after_update_payment$$
CREATE TRIGGER trg_after_update_payment
AFTER UPDATE ON payment FOR EACH ROW
BEGIN
    IF OLD.payment_status <> NEW.payment_status THEN
        INSERT INTO audit_log(table_name, action, record_id, details)
        VALUES('payment','UPDATE', NEW.payment_id,
            CONCAT('Payment ', NEW.payment_id, ' status: ',
                   OLD.payment_status, ' → ', NEW.payment_status,
                   ' | Parcel: ', NEW.parcel_id));
    END IF;
END$$

DELIMITER ;
