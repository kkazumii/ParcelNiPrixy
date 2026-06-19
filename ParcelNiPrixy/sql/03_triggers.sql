-- ============================================================
-- TRIGGERS v3
-- ============================================================

USE parcel_tracking_db;
DELIMITER $$

DROP TRIGGER IF EXISTS trg_before_insert_parcel$$
CREATE TRIGGER trg_before_insert_parcel
BEFORE INSERT ON parcel FOR EACH ROW
BEGIN
    IF NEW.parcel_id IS NULL OR TRIM(NEW.parcel_id)='' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Parcel ID cannot be empty.';
    END IF;
END$$

DROP TRIGGER IF EXISTS trg_after_insert_parcel$$
CREATE TRIGGER trg_after_insert_parcel
AFTER INSERT ON parcel FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name,action,record_id,details)
    VALUES('parcel','INSERT',NEW.parcel_id,
        CONCAT('New parcel | Shipment: ',NEW.shipment_id,
               ' | Sender: ',NEW.sender_id,
               ' | Recipient: ',NEW.recipient_id,
               ' | Type: ',NEW.parcel_type));
END$$

DROP TRIGGER IF EXISTS trg_after_update_parcel$$
CREATE TRIGGER trg_after_update_parcel
AFTER UPDATE ON parcel FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name,action,record_id,details)
    VALUES('parcel','UPDATE',NEW.parcel_id,
        CONCAT('Updated | Weight: ',IFNULL(NEW.weight_kg,'N/A'),
               ' | Type: ',NEW.parcel_type));
END$$

DROP TRIGGER IF EXISTS trg_after_delete_parcel$$
CREATE TRIGGER trg_after_delete_parcel
AFTER DELETE ON parcel FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name,action,record_id,details)
    VALUES('parcel','DELETE',OLD.parcel_id,
        CONCAT('Deleted | Recipient: ',OLD.recipient_id));
END$$

DROP TRIGGER IF EXISTS trg_before_insert_shipment$$
CREATE TRIGGER trg_before_insert_shipment
BEFORE INSERT ON shipment FOR EACH ROW
BEGIN
    IF NEW.estimated_delivery < NEW.shipment_date THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT='Estimated delivery cannot be before shipment date.';
    END IF;
END$$

DROP TRIGGER IF EXISTS trg_after_insert_shipment$$
CREATE TRIGGER trg_after_insert_shipment
AFTER INSERT ON shipment FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name,action,record_id,details)
    VALUES('shipment','INSERT',NEW.shipment_id,
        CONCAT('New shipment | Rider: ',NEW.rider_id,
               ' | Route: ',NEW.route_id,
               ' | Date: ',NEW.shipment_date));
END$$

DROP TRIGGER IF EXISTS trg_before_insert_tracking$$
CREATE TRIGGER trg_before_insert_tracking
BEFORE INSERT ON tracking_status FOR EACH ROW
BEGIN
    DECLARE v_last VARCHAR(30);
    SELECT status_type INTO v_last FROM tracking_status
    WHERE parcel_id=NEW.parcel_id ORDER BY timestamp DESC LIMIT 1;

    IF v_last='delivered' THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT='Cannot update: parcel already delivered.';
    END IF;

    IF v_last='failed delivery' AND NEW.status_type NOT IN ('in transit','out for delivery') THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT='After failed delivery, status must be in transit or out for delivery.';
    END IF;
END$$

DROP TRIGGER IF EXISTS trg_after_insert_tracking$$
CREATE TRIGGER trg_after_insert_tracking
AFTER INSERT ON tracking_status FOR EACH ROW
BEGIN
    INSERT INTO audit_log(table_name,action,record_id,details)
    VALUES('tracking_status','INSERT',NEW.tracking_id,
        CONCAT('Parcel ',NEW.parcel_id,' → ',NEW.status_type,
               IFNULL(CONCAT(' | Location: ',NEW.location),'')));
END$$

DROP TRIGGER IF EXISTS trg_after_update_payment$$
CREATE TRIGGER trg_after_update_payment
AFTER UPDATE ON payment FOR EACH ROW
BEGIN
    IF OLD.payment_status<>NEW.payment_status THEN
        INSERT INTO audit_log(table_name,action,record_id,details)
        VALUES('payment','UPDATE',NEW.payment_id,
            CONCAT('Payment ',OLD.payment_status,' → ',NEW.payment_status,
                   ' | Parcel: ',NEW.parcel_id));
    END IF;
END$$

DELIMITER ;
