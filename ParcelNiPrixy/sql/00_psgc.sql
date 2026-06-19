-- PSGC: Philippine Standard Geographic Code
-- Provinces and Municipalities (abbreviated for practical use)
-- Source: PSGC 2024

USE parcel_tracking_db;

CREATE TABLE IF NOT EXISTS ph_province (
    province_id   VARCHAR(10) PRIMARY KEY,
    province_name VARCHAR(100) NOT NULL,
    region        VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS ph_municipality (
    municipality_id   VARCHAR(10) PRIMARY KEY,
    municipality_name VARCHAR(100) NOT NULL,
    province_id       VARCHAR(10) NOT NULL,
    FOREIGN KEY (province_id) REFERENCES ph_province(province_id)
);

INSERT IGNORE INTO ph_province VALUES
('PH-ABR','Abra','CAR'),('PH-AGN','Agusan del Norte','Caraga'),('PH-AGS','Agusan del Sur','Caraga'),
('PH-AKL','Aklan','Region VI'),('PH-ALB','Albay','Region V'),('PH-ANT','Antique','Region VI'),
('PH-APY','Apayao','CAR'),('PH-AUR','Aurora','Region III'),('PH-BAS','Basilan','BARMM'),
('PH-BAN','Bataan','Region III'),('PH-BTN','Batanes','Region II'),('PH-BTG','Batangas','Region IV-A'),
('PH-BEN','Benguet','CAR'),('PH-BIL','Biliran','Region VIII'),('PH-BOH','Bohol','Region VII'),
('PH-BUK','Bukidnon','Region X'),('PH-BUL','Bulacan','Region III'),('PH-CAG','Cagayan','Region II'),
('PH-CAN','Camarines Norte','Region V'),('PH-CAS','Camarines Sur','Region V'),
('PH-CAM','Camiguin','Region X'),('PH-CAP','Capiz','Region VI'),('PH-CAT','Catanduanes','Region V'),
('PH-CAV','Cavite','Region IV-A'),('PH-CEB','Cebu','Region VII'),('PH-COM','Compostela Valley','Region XI'),
('PH-NCO','Cotabato','Region XII'),('PH-DAV','Davao del Norte','Region XI'),
('PH-DAS','Davao del Sur','Region XI'),('PH-DAO','Davao Occidental','Region XI'),
('PH-DAC','Davao de Oro','Region XI'),('PH-DAO2','Davao Oriental','Region XI'),
('PH-DIN','Dinagat Islands','Caraga'),('PH-EAS','Eastern Samar','Region VIII'),
('PH-GUI','Guimaras','Region VI'),('PH-IFU','Ifugao','CAR'),('PH-ILN','Ilocos Norte','Region I'),
('PH-ILS','Ilocos Sur','Region I'),('PH-ILI','Iloilo','Region VI'),('PH-ISA','Isabela','Region II'),
('PH-KAL','Kalinga','CAR'),('PH-LAN','La Union','Region I'),('PH-LAG','Laguna','Region IV-A'),
('PH-LAN2','Lanao del Norte','Region X'),('PH-LAS','Lanao del Sur','BARMM'),
('PH-LEY','Leyte','Region VIII'),('PH-MAG','Maguindanao','BARMM'),('PH-MAR','Marinduque','Region IV-B'),
('PH-MAS','Masbate','Region V'),('PH-MDN','Misamis Occidental','Region X'),
('PH-MDS','Misamis Oriental','Region X'),('PH-MNT','Mountain Province','CAR'),
('PH-NCR','Metro Manila','NCR'),('PH-NEC','Negros Occidental','Region VI'),
('PH-NOR','Negros Oriental','Region VII'),('PH-NSA','Northern Samar','Region VIII'),
('PH-NUE','Nueva Ecija','Region III'),('PH-NUV','Nueva Vizcaya','Region II'),
('PH-MDO','Occidental Mindoro','Region IV-B'),('PH-MOR','Oriental Mindoro','Region IV-B'),
('PH-PAL','Palawan','Region IV-B'),('PH-PAM','Pampanga','Region III'),('PH-PAN','Pangasinan','Region I'),
('PH-QUE','Quezon','Region IV-A'),('PH-QUI','Quirino','Region II'),('PH-RIZ','Rizal','Region IV-A'),
('PH-ROM','Romblon','Region IV-B'),('PH-WSA','Samar','Region VIII'),('PH-SAR','Sarangani','Region XII'),
('PH-SIQ','Siquijor','Region VII'),('PH-SOR','Sorsogon','Region V'),
('PH-SCO','South Cotabato','Region XII'),('PH-SLE','Southern Leyte','Region VIII'),
('PH-SUK','Sultan Kudarat','Region XII'),('PH-SLU','Sulu','BARMM'),
('PH-SUR','Surigao del Norte','Caraga'),('PH-SUS','Surigao del Sur','Caraga'),
('PH-TAR','Tarlac','Region III'),('PH-TAW','Tawi-Tawi','BARMM'),('PH-ZMB','Zambales','Region III'),
('PH-ZAN','Zamboanga del Norte','Region IX'),('PH-ZAS','Zamboanga del Sur','Region IX'),
('PH-ZSI','Zamboanga Sibugay','Region IX');

INSERT IGNORE INTO ph_municipality VALUES
-- Metro Manila (NCR)
('MNL','City of Manila','PH-NCR'),('QZN','Quezon City','PH-NCR'),('MKT','Makati','PH-NCR'),
('PSG','Pasig','PH-NCR'),('TGG','Taguig','PH-NCR'),('MND','Mandaluyong','PH-NCR'),
('MSK','Marikina','PH-NCR'),('PQE','Parañaque','PH-NCR'),('LPA','Las Piñas','PH-NCR'),
('MNT2','Muntinlupa','PH-NCR'),('VLZ','Valenzuela','PH-NCR'),('KLO','Kalookan','PH-NCR'),
('MLA','Malabon','PH-NCR'),('NAV','Navotas','PH-NCR'),('PSY','Pasay','PH-NCR'),
('PTR','Pateros','PH-NCR'),('SJN','San Juan','PH-NCR'),
-- Bulacan
('MYC','Meycauayan','PH-BUL'),('SMR','Santa Maria','PH-BUL'),('MLS','Malolos','PH-BUL'),
('OBD','Obando','PH-BUL'),('PLR','Pulilan','PH-BUL'),('PND','Pandi','PH-BUL'),
('BST','Bustos','PH-BUL'),('BLN','Balagtas','PH-BUL'),('GPC','Guiguinto','PH-BUL'),
('MBC','Marilao','PH-BUL'),('PAR','Paombong','PH-BUL'),('PLJ','Plaridel','PH-BUL'),
('SRI','San Rafael','PH-BUL'),('SIL','San Ildefonso','PH-BUL'),('SMG','San Miguel','PH-BUL'),
('SPS','San Jose del Monte','PH-BUL'),('NRZ','Norzagaray','PH-BUL'),('DNG','Doña Remedios Trinidad','PH-BUL'),
('ANG','Angat','PH-BUL'),('BKW','Bokod','PH-BUL'),('CAL','Calumpit','PH-BUL'),
('HGN','Hagonoy','PH-BUL'),('RCL','Bocaue','PH-BUL'),
-- Pampanga
('SFN','San Fernando','PH-PAM'),('ANG2','Angeles','PH-PAM'),('MBL','Mabalacat','PH-PAM'),
('CLC','Calumpit','PH-PAM'),('GPN','Guagua','PH-PAM'),('LBW','Lubao','PH-PAM'),
('MCB','Macabebe','PH-PAM'),('MCL','Magalang','PH-PAM'),('MKB','Masantol','PH-PAM'),
('MXC','Mexico','PH-PAM'),('PMP','Porac','PH-PAM'),('SNS','San Luis','PH-PAM'),
('STR','Sta. Rita','PH-PAM'),('SBL','Sasmuan','PH-PAM'),('FLR','Floridablanca','PH-PAM'),
('APC','Apalit','PH-PAM'),('BRG','Bacolor','PH-PAM'),('CND','Candaba','PH-PAM'),
-- Cavite
('BCA','Bacoor','PH-CAV'),('DSD','Dasmariñas','PH-CAV'),('GEN','General Trias','PH-CAV'),
('IMS','Imus','PH-CAV'),('KWT','Kawit','PH-CAV'),('TRZ','Trece Martires','PH-CAV'),
('TAG2','Tagaytay','PH-CAV'),('ALF','Alfonso','PH-CAV'),('AMD','Amadeo','PH-CAV'),
('CVT','Cavite City','PH-CAV'),('GNS','General Mariano Alvarez','PH-CAV'),
('IND','Indang','PH-CAV'),('MGR','Magallanes','PH-CAV'),('MRG','Maragondon','PH-CAV'),
('NSB','Naic','PH-CAV'),('NVL','Noveleta','PH-CAV'),('RSD','Rosario','PH-CAV'),
('SLN','Silang','PH-CAV'),('TNZ','Tanza','PH-CAV'),('TRN','Ternate','PH-CAV'),
-- Laguna
('SPC','San Pedro','PH-LAG'),('BIN','Biñan','PH-LAG'),('STA','Santa Rosa','PH-LAG'),
('CAB','Cabuyao','PH-LAG'),('CLM','Calamba','PH-LAG'),('SPN','San Pablo','PH-LAG'),
('LOS','Los Baños','PH-LAG'),('BTN2','Bay','PH-LAG'),('CRN','Calauan','PH-LAG'),
('LLN','Liliw','PH-LAG'),('LUM','Lumban','PH-LAG'),('MJD','Majayjay','PH-LAG'),
('MGL','Magdalena','PH-LAG'),('MNB','Mabitac','PH-LAG'),('MTC','Mtc','PH-LAG'),
('PAG','Pagsanjan','PH-LAG'),('PKL','Pakil','PH-LAG'),('PSN','Pangil','PH-LAG'),
('RZL','Rizal','PH-LAG'),('SCS','Santa Cruz','PH-LAG'),('SMP','Siniloan','PH-LAG'),
('VCR','Victoria','PH-LAG'),('FZN','Famy','PH-LAG'),('KLY','Kalayaan','PH-LAG'),
-- Batangas
('BTG2','Batangas City','PH-BTG'),('LPA2','Lipa','PH-BTG'),('TAN','Tanauan','PH-BTG'),
('STR2','Santo Tomas','PH-BTG'),('MRZ','Malvar','PH-BTG'),('NAS','Nasugbu','PH-BTG'),
('LMR','Lemery','PH-BTG'),('ROS','Rosario','PH-BTG'),('BAU','Bauan','PH-BTG'),
('CLN','Cuenca','PH-BTG'),('AGO','Agoncillo','PH-BTG'),('ALI','Alitagtag','PH-BTG'),
('BLT','Balayan','PH-BTG'),('BNT','Bantay','PH-BTG'),('CAL2','Calauan','PH-BTG'),
('CLD','Calaca','PH-BTG'),('CLS','Calatagan','PH-BTG'),('CRP','Cuenca','PH-BTG'),
('LAN3','Laurel','PH-BTG'),('LYA','Lobo','PH-BTG'),('MBT','Mabini','PH-BTG'),
('MTC2','Mataasnakahoy','PH-BTG'),('PDS','Padre Garcia','PH-BTG'),('SAN','San Juan','PH-BTG'),
('SLS','San Luis','PH-BTG'),('SNP','San Nicolas','PH-BTG'),('SPS2','San Pascual','PH-BTG'),
('TUY','Tuy','PH-BTG'),
-- Rizal
('ANT2','Antipolo','PH-RIZ'),('CAI','Cainta','PH-RIZ'),('TAY','Taytay','PH-RIZ'),
('SML','San Mateo','PH-RIZ'),('RDZ','Rodriguez','PH-RIZ'),('MRK','Morong','PH-RIZ'),
('BAR','Baras','PH-RIZ'),('BJR','Binangonan','PH-RIZ'),('CPT','Cardona','PH-RIZ'),
('JAL','Jala-Jala','PH-RIZ'),('PIL','Pililla','PH-RIZ'),('TNG','Tanay','PH-RIZ'),
('TRS','Teresa','PH-RIZ'),
-- Quezon Province
('LCD','Lucena','PH-QUE'),('TYB','Tayabas','PH-QUE'),('LBN','Lucban','PH-QUE'),
('MUL','Mauban','PH-QUE'),('SRY','Sariaya','PH-QUE'),('CGN','Candelaria','PH-QUE'),
('LBY','Calauag','PH-QUE'),('GMY','Gumaca','PH-QUE'),('IPL','Infanta','PH-QUE'),
('PDN','Padre Burgos','PH-QUE'),('RLB','Real','PH-QUE'),
-- Nueva Ecija
('PLY','Palayan','PH-NUE'),('CBC','Cabanatuan','PH-NUE'),('MBY','Muñoz','PH-NUE'),
('SNJ','San Jose','PH-NUE'),('GPN2','Gapan','PH-NUE'),('ALG','Aliaga','PH-NUE'),
('BNG','Bongabon','PH-NUE'),('CBN','Cabiao','PH-NUE'),('CYB','Cuyapo','PH-NUE'),
('GNZ','Guimba','PH-NUE'),('LLG','Laur','PH-NUE'),('LNT','Licab','PH-NUE'),
('LRP','Llanera','PH-NUE'),('MRC','Nampicuan','PH-NUE'),('PPN','Peñaranda','PH-NUE'),
('RZD','Rizal','PH-NUE'),('SNN','San Antonio','PH-NUE'),('SNI','San Isidro','PH-NUE'),
('SNL','San Leonardo','PH-NUE'),('STA2','Santa Rosa','PH-NUE'),('SGT','Sto. Domingo','PH-NUE'),
('TLV','Talavera','PH-NUE'),('TYG','Talugtug','PH-NUE'),('ZRG','Zaragoza','PH-NUE'),
-- Tarlac
('TRL','Tarlac City','PH-TAR'),('CPP','Capas','PH-TAR'),('CGC','Concepcion','PH-TAR'),
('GER','Gerona','PH-TAR'),('LPZ','La Paz','PH-TAR'),('MBN','Mabini','PH-TAR'),
('MGT','Mayantoc','PH-TAR'),('MND2','Moncada','PH-TAR'),('PNB','Paniqui','PH-TAR'),
('PNT','Panique','PH-TAR'),('RMO','Ramos','PH-TAR'),('SNM','San Clemente','PH-TAR'),
('SMN','San Manuel','PH-TAR'),('SNX','Santa Ignacia','PH-TAR'),('VMC','Victoria','PH-TAR'),
-- Zambales
('OLN','Olongapo','PH-ZMB'),('IBA','Iba','PH-ZMB'),('SNT','San Antonio','PH-ZMB'),
('SBL2','Subic','PH-ZMB'),('CSTl','Castillejos','PH-ZMB'),('SFN2','San Felipe','PH-ZMB'),
('SNN2','San Narciso','PH-ZMB'),('SMA','Santa Cruz','PH-ZMB'),('CNP','Candelaria','PH-ZMB'),
('MSN','Masinloc','PH-ZMB'),('PLC','Palauig','PH-ZMB'),('RMB','Romblon','PH-ZMB'),
-- Bataan
('BLC','Balanga','PH-BAN'),('ABU','Abucay','PH-BAN'),('BAG','Bagac','PH-BAN'),
('DNG2','Dinalupihan','PH-BAN'),('HRN','Hermosa','PH-BAN'),('LML','Limay','PH-BAN'),
('MRV','Mariveles','PH-BAN'),('MRL','Morong','PH-BAN'),('ORC','Orion','PH-BAN'),
('PIL2','Pilar','PH-BAN'),('SNF','Samal','PH-BAN'),
-- Ilocos Norte
('LGG','Laoag','PH-ILN'),('BTC','Batac','PH-ILN'),('PGN','Pagudpud','PH-ILN'),
('VGN','Vintar','PH-ILN'),('BDG','Badoc','PH-ILN'),('BNN','Bangui','PH-ILN'),
('BRT','Barataria','PH-ILN'),('CVR','Carasi','PH-ILN'),('DNG3','Dingras','PH-ILN'),
('ELB','Piddig','PH-ILN'),('PGS','Pasuquin','PH-ILN'),('SDN','Solsona','PH-ILN'),
('SRN','Sarrat','PH-ILN'),('TNB','Tagudin','PH-ILN'),
-- Cebu
('CBU','Cebu City','PH-CEB'),('MDW','Mandaue','PH-CEB'),('LPN','Lapu-Lapu','PH-CEB'),
('TLB','Toledo','PH-CEB'),('DNG4','Danao','PH-CEB'),('CRN2','Carcar','PH-CEB'),
('TGN','Talisay','PH-CEB'),('NGA','Naga','PH-CEB'),('BGO','Bogo','PH-CEB'),
('BRN','Balamban','PH-CEB'),('ARG','Argao','PH-CEB'),('ALG2','Alcoy','PH-CEB'),
('AST','Asturias','PH-CEB'),('BDJ','Barili','PH-CEB'),('BLG','Borbon','PH-CEB'),
('CBG','Catmon','PH-CEB'),('CNS','Consolacion','PH-CEB'),('CRD','Cordova','PH-CEB'),
('DLG','Dalaguete','PH-CEB'),('GBT','Ginatilan','PH-CEB'),('LLL','Liloan','PH-CEB'),
('LMC','Madridejos','PH-CEB'),('MGN','Malabuyoc','PH-CEB'),('MGL2','Minglanilla','PH-CEB'),
('MBL2','Moalboal','PH-CEB'),('OSB','Oslob','PH-CEB'),('PDR','Pilar','PH-CEB'),
('PSN2','Pinamungajan','PH-CEB'),('RMS','Ronda','PH-CEB'),('SMB','Samboan','PH-CEB'),
('SFC','San Fernando','PH-CEB'),('SFN3','San Francisco','PH-CEB'),('SRF','San Remigio','PH-CEB'),
('SNT2','Santa Fe','PH-CEB'),('SGR','Santander','PH-CEB'),('SBN','Sibonga','PH-CEB'),
('SGD','Sogod','PH-CEB'),('TPZ','Tabogon','PH-CEB'),('TBL','Tabuelan','PH-CEB'),
('TGS','Tuburan','PH-CEB'),('TDB','Tudela','PH-CEB'),
-- Davao del Sur
('DVO','Davao City','PH-DAS'),('DGS','Digos','PH-DAS'),('MTI','Matanao','PH-DAS'),
('MLP','Magsaysay','PH-DAS'),('PDP','Padada','PH-DAS'),('STA3','Santa Cruz','PH-DAS'),
('TBL2','Sulop','PH-DAS'),('BNS','Bansalan','PH-DAS'),('HGO','Hagonoy','PH-DAS'),
('KDN','Kiblawan','PH-DAS'),('MLG','Malalag','PH-DAS'),
-- Iloilo
('ILO','Iloilo City','PH-ILI'),('PSI','Passi','PH-ILI'),('SGJ','San Joaquin','PH-ILI'),
('CAL3','Calinog','PH-ILI'),('CNC','Concepcion','PH-ILI'),('DGS2','Duenas','PH-ILI'),
('DNG5','Dumangas','PH-ILI'),('JNI','Janiuay','PH-ILI'),('LGN','Leganes','PH-ILI'),
('LMB','Lambunao','PH-ILI'),('MBT2','Maasin','PH-ILI'),('MLT','Miagao','PH-ILI'),
('MNA','Mina','PH-ILI'),('NVL2','New Lucena','PH-ILI'),('OTN','Oton','PH-ILI'),
('PVN','Pavia','PH-ILI'),('STB','Santa Barbara','PH-ILI'),('SRB','Sara','PH-ILI'),
('TGN2','Tigbauan','PH-ILI'),('TUB','Tubungan','PH-ILI'),('ZRG2','Zarraga','PH-ILI');

-- Stored procedures to load provinces and municipalities
DROP PROCEDURE IF EXISTS sp_get_provinces;
DELIMITER $$
CREATE PROCEDURE sp_get_provinces()
BEGIN
    SELECT province_id, province_name, region
    FROM ph_province
    ORDER BY region, province_name;
END$$

DROP PROCEDURE IF EXISTS sp_get_municipalities;
CREATE PROCEDURE sp_get_municipalities(IN p_province_id VARCHAR(10))
BEGIN
    SELECT municipality_id, municipality_name
    FROM ph_municipality
    WHERE province_id = p_province_id
    ORDER BY municipality_name;
END$$
DELIMITER ;
