create database thulai;
go
use thulai;
go
/* Danh mục chức danh giảng dạy (base) */
IF OBJECT_ID('dbo.CHUCDANH','U') IS NULL
CREATE TABLE dbo.CHUCDANH (
  MaChucDanh  CHAR(10)  NOT NULL PRIMARY KEY,
  TenChucDanh NVARCHAR(100) NOT NULL
);

/* Giờ chuẩn theo chức danh (năm) */
IF OBJECT_ID('dbo.DINHMUCGIO','U') IS NULL
CREATE TABLE dbo.DINHMUCGIO (
  MaDinhMucGio CHAR(10)  NOT NULL PRIMARY KEY, -- trùng với MaChucDanh để đơn giản
  GioChuan     INT       NOT NULL              -- giờ/năm
);

/* Chức vụ quản lý (để giảm tải) */
IF OBJECT_ID('dbo.CHUCVU','U') IS NULL
CREATE TABLE dbo.CHUCVU (
  MaChucVu  CHAR(10) NOT NULL PRIMARY KEY,
  TenChucVu NVARCHAR(100) NOT NULL,
  HeSoGiam  DECIMAL(5,2)  NOT NULL DEFAULT(0) -- % giảm, ví dụ 25.00 = giảm 25%
);

/* Loại học viên */
IF OBJECT_ID('dbo.LOAIHOCVIEN','U') IS NULL
CREATE TABLE dbo.LOAIHOCVIEN (
  MaLoaiHocVien CHAR(10) NOT NULL PRIMARY KEY,
  TenLoaiHocVien NVARCHAR(100) NOT NULL
);

/* Ngôn ngữ giảng dạy */
IF OBJECT_ID('dbo.NGONNGU','U') IS NULL
CREATE TABLE dbo.NGONNGU (
  MaNgonNgu  CHAR(10) NOT NULL PRIMARY KEY,
  TenNgonNgu NVARCHAR(100) NOT NULL
);

/* Ca kíp */
IF OBJECT_ID('dbo.CAKIP','U') IS NULL
CREATE TABLE dbo.CAKIP (
  MaCaKip  CHAR(10) NOT NULL PRIMARY KEY,
  TenCaKip NVARCHAR(100) NOT NULL
);

/* Lớp học */
IF OBJECT_ID('dbo.LOP','U') IS NULL
CREATE TABLE dbo.LOP (
  MaLop   CHAR(10) NOT NULL PRIMARY KEY,
  TenLop  NVARCHAR(100) NOT NULL,
  QuanSo  INT NOT NULL DEFAULT(0)
);

/* Môn học */
IF OBJECT_ID('dbo.MONHOC','U') IS NULL
CREATE TABLE dbo.MONHOC (
  MaMonHoc CHAR(10) NOT NULL PRIMARY KEY,
  TenMonHoc NVARCHAR(200) NOT NULL
);

/* Giảng viên */
IF OBJECT_ID('dbo.GIANGVIEN','U') IS NULL
CREATE TABLE dbo.GIANGVIEN (
  MaGV       CHAR(10)  NOT NULL PRIMARY KEY,
  HoTen      NVARCHAR(150) NOT NULL,
  MaChucDanh CHAR(10)  NULL,
  MaChucVu   CHAR(10)  NULL,
  CONSTRAINT FK_GV_CD  FOREIGN KEY (MaChucDanh) REFERENCES dbo.CHUCDANH(MaChucDanh),
  CONSTRAINT FK_GV_CV  FOREIGN KEY (MaChucVu)   REFERENCES dbo.CHUCVU(MaChucVu)
);

/* Lịch sử chức danh theo kỳ (ưu tiên lấy ở đây khi tính) */
IF OBJECT_ID('dbo.CHUCDANH_GV_KY','U') IS NULL
CREATE TABLE dbo.CHUCDANH_GV_KY (
  MaGV       CHAR(10)     NOT NULL,
  NamHoc     NVARCHAR(10) NOT NULL,   -- ví dụ '2025-2026'
  HocKy      NVARCHAR(10) NOT NULL,   -- 'HK1' | 'HK2'
  MaChucDanh CHAR(10)     NOT NULL,
  PRIMARY KEY (MaGV, NamHoc, HocKy),
  CONSTRAINT FK_CDGK_GV FOREIGN KEY (MaGV)       REFERENCES dbo.GIANGVIEN(MaGV),
  CONSTRAINT FK_CDGK_CD FOREIGN KEY (MaChucDanh) REFERENCES dbo.CHUCDANH(MaChucDanh)
);

/* Thời khóa biểu header */
IF OBJECT_ID('dbo.THOIKHOABIEU','U') IS NULL
CREATE TABLE dbo.THOIKHOABIEU (
  MaTKB   CHAR(10)     NOT NULL PRIMARY KEY,
  NamHoc  NVARCHAR(10) NOT NULL,
  HocKy   NVARCHAR(10) NOT NULL
);

/* Thời khóa biểu chi tiết (mỗi buổi/tiết) */
IF OBJECT_ID('dbo.TKB_CHITIET','U') IS NULL
CREATE TABLE dbo.TKB_CHITIET (
  MaTKBCT       CHAR(12) NOT NULL PRIMARY KEY,
MaTKB         CHAR(10) NOT NULL,
  MaMonHoc      CHAR(10) NOT NULL,
  MaLop         CHAR(10) NOT NULL,
  MaLoaiHocVien CHAR(10) NOT NULL,
  MaNgonNgu     CHAR(10) NOT NULL,
  MaCaKip       CHAR(10) NOT NULL,
  NgayHoc       DATE     NOT NULL,
  SoTiet        INT      NOT NULL CHECK (SoTiet > 0),
  CONSTRAINT FK_CT_TKB   FOREIGN KEY (MaTKB)         REFERENCES dbo.THOIKHOABIEU(MaTKB),
  CONSTRAINT FK_CT_MH    FOREIGN KEY (MaMonHoc)      REFERENCES dbo.MONHOC(MaMonHoc),
  CONSTRAINT FK_CT_LOP   FOREIGN KEY (MaLop)         REFERENCES dbo.LOP(MaLop),
  CONSTRAINT FK_CT_LHV   FOREIGN KEY (MaLoaiHocVien) REFERENCES dbo.LOAIHOCVIEN(MaLoaiHocVien),
  CONSTRAINT FK_CT_NN    FOREIGN KEY (MaNgonNgu)     REFERENCES dbo.NGONNGU(MaNgonNgu),
  CONSTRAINT FK_CT_CK    FOREIGN KEY (MaCaKip)       REFERENCES dbo.CAKIP(MaCaKip)
);

/* Phân công giảng dạy 1 chi tiết cho GV (vai trò chính/phụ) */
IF OBJECT_ID('dbo.PHANCONG','U') IS NULL
CREATE TABLE dbo.PHANCONG (
  MaPC     INT IDENTITY(1,1) PRIMARY KEY,
  MaTKBCT  CHAR(12) NOT NULL,
  MaGV     CHAR(10) NOT NULL,
  VaiTro   NVARCHAR(10) NOT NULL DEFAULT(N'CHINH'), -- CHINH | PHU
  CONSTRAINT FK_PC_CT FOREIGN KEY (MaTKBCT) REFERENCES dbo.TKB_CHITIET(MaTKBCT),
  CONSTRAINT FK_PC_GV FOREIGN KEY (MaGV)    REFERENCES dbo.GIANGVIEN(MaGV)
);

/* Bảng kết quả vượt tải (có thể lưu theo kỳ và theo năm) */
IF OBJECT_ID('dbo.TINHVUOTTAI','U') IS NULL
CREATE TABLE dbo.TINHVUOTTAI (
  MaTinhVuotTai NVARCHAR(50) NOT NULL PRIMARY KEY,
  MaGV          CHAR(10)     NOT NULL,
  NamHoc        NVARCHAR(10) NOT NULL,
  HocKy         NVARCHAR(10) NULL, -- null = tổng năm
  TongGioChuan  DECIMAL(10,2) NOT NULL DEFAULT(0),
  TongGioThucTe DECIMAL(10,2) NOT NULL DEFAULT(0),
  NgayTinh      DATETIME2     NOT NULL DEFAULT(SYSDATETIME()),
  CONSTRAINT FK_VT_GV FOREIGN KEY (MaGV) REFERENCES dbo.GIANGVIEN(MaGV)
);


----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------


go
use thulai;

CREATE INDEX IX_TKBCT_Lop_NN_LHV_Ca ON dbo.TKB_CHITIET(MaLop, MaNgonNgu, MaLoaiHocVien, MaCaKip);
CREATE INDEX IX_TKBCT_TKB ON dbo.TKB_CHITIET(MaTKB);
CREATE INDEX IX_PC_TKBCT ON dbo.PHANCONG(MaTKBCT);
CREATE INDEX IX_PC_GV ON dbo.PHANCONG(MaGV);
CREATE INDEX IX_TKB_NamHoc_HocKy ON dbo.THOIKHOABIEU(NamHoc, HocKy);
CREATE INDEX IX_CDGK_GV_NamHoc_HocKy ON dbo.CHUCDANH_GV_KY(MaGV, NamHoc, HocKy);
CREATE INDEX IX_VT_MaGV_NamHoc_HocKy ON dbo.TINHVUOTTAI(MaGV, NamHoc, HocKy);
go


----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------


go
use thulai;
MERGE dbo.CHUCDANH AS T
USING (VALUES
 ('TROGIANG', N'Trợ giảng'),
 ('GV',       N'Giảng viên'),
 ('GVC',      N'Giảng viên chính'),
 ('GVCC',     N'Giảng viên cao cấp'),
 ('PGS',      N'Phó giáo sư'),
 ('GS',       N'Giáo sư')
) S(MaChucDanh, TenChucDanh)
ON T.MaChucDanh = S.MaChucDanh
WHEN MATCHED THEN UPDATE SET TenChucDanh = S.TenChucDanh
WHEN NOT MATCHED THEN INSERT (MaChucDanh,TenChucDanh) VALUES (S.MaChucDanh,S.TenChucDanh);

MERGE dbo.DINHMUCGIO AS T
USING (VALUES
 ('TROGIANG', 240),
 ('GV',       270),
 ('GVC',      300),
 ('PGS',      300),
 ('GVCC',     320),
 ('GS',       320)
) S(MaDinhMucGio, GioChuan)
ON T.MaDinhMucGio = S.MaDinhMucGio
WHEN MATCHED THEN UPDATE SET GioChuan = S.GioChuan
WHEN NOT MATCHED THEN INSERT (MaDinhMucGio,GioChuan) VALUES (S.MaDinhMucGio,S.GioChuan);

MERGE dbo.CHUCVU AS T
USING (VALUES
 ('GĐ',  N'Giám đốc',       10.0),
 ('PGĐ', N'Phó giám đốc',   15.0),
 ('TP',  N'Trưởng phòng',   25.0),
 ('PP',  N'Phó phòng',      15.0),
 ('CB',  N'Cán bộ',         10.0)
) S(MaChucVu, TenChucVu, HeSoGiam)
ON T.MaChucVu = S.MaChucVu
WHEN MATCHED THEN UPDATE SET TenChucVu = S.TenChucVu, HeSoGiam=S.HeSoGiam
WHEN NOT MATCHED THEN INSERT (MaChucVu,TenChucVu,HeSoGiam) VALUES (S.MaChucVu,S.TenChucVu,S.HeSoGiam);

MERGE dbo.LOAIHOCVIEN AS T
USING (VALUES
 ('VN',  N'Việt Nam'),
 ('LAO', N'Lào')
) S(MaLoaiHocVien, TenLoaiHocVien)
ON T.MaLoaiHocVien = S.MaLoaiHocVien
WHEN MATCHED THEN UPDATE SET TenLoaiHocVien = S.TenLoaiHocVien
WHEN NOT MATCHED THEN INSERT (MaLoaiHocVien,TenLoaiHocVien) VALUES (S.MaLoaiHocVien,S.TenLoaiHocVien);

MERGE dbo.NGONNGU AS T
USING (VALUES
 ('VI', N'Tiếng Việt'),
 ('EN', N'Tiếng Anh'),
 ('RU', N'Tiếng Nga')
) S(MaNgonNgu, TenNgonNgu)
ON T.MaNgonNgu = S.MaNgonNgu
WHEN MATCHED THEN UPDATE SET TenNgonNgu = S.TenNgonNgu
WHEN NOT MATCHED THEN INSERT (MaNgonNgu,TenNgonNgu) VALUES (S.MaNgonNgu,S.TenNgonNgu);

MERGE dbo.CAKIP AS T
USING (VALUES
 ('SANG',  N'Buổi sáng'),
 ('CHIEU', N'Buổi chiều'),
 ('TOI',   N'Buổi tối'),
 ('TRUA',  N'Trưa 12h')
) S(MaCaKip, TenCaKip)
ON T.MaCaKip = S.MaCaKip
WHEN MATCHED THEN UPDATE SET TenCaKip = S.TenCaKip
WHEN NOT MATCHED THEN INSERT (MaCaKip,TenCaKip) VALUES (S.MaCaKip,S.TenCaKip);

---------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------

go

/* 1 tiết = 45 phút */
CREATE OR ALTER FUNCTION dbo.ufn_GioPerTiet()
RETURNS DECIMAL(6,4)
AS
BEGIN
  RETURN 0.75;
END;
GO

/* % giảm tải chức vụ → hệ số nhân (10% => *0.90) */
CREATE OR ALTER FUNCTION dbo.ufn_HeSoGiamTaiTheoChucVu (@MaChucVu CHAR(10))
RETURNS DECIMAL(6,4)
AS
BEGIN
  DECLARE @pt DECIMAL(6,3) = (SELECT TOP(1) HeSoGiam FROM dbo.CHUCVU WHERE MaChucVu=@MaChucVu);
  IF @pt IS NULL RETURN 1.0;
RETURN CAST(1.0 - (@pt/100.0) AS DECIMAL(6,4));
END;
GO

/* Giờ chuẩn cơ sở theo chức danh → từ DINHMUCGIO; fallback theo tên nếu cần */
CREATE OR ALTER FUNCTION dbo.ufn_GioChuanCoSo_ByChucDanh (@MaChucDanh CHAR(10))
RETURNS INT
AS
BEGIN
  DECLARE @gio INT = (SELECT TOP(1) GioChuan FROM dbo.DINHMUCGIO WHERE MaDinhMucGio=@MaChucDanh);
  IF @gio IS NOT NULL RETURN @gio;

  DECLARE @ten NVARCHAR(100) = (SELECT TOP(1) TenChucDanh FROM dbo.CHUCDANH WHERE MaChucDanh=@MaChucDanh);
  IF @ten IS NULL RETURN 270;
  RETURN CASE 
    WHEN @ten LIKE N'%Trợ giảng%' THEN 240
    WHEN @ten LIKE N'%cao cấp%' OR @ten LIKE N'%GVCC%' OR @ten LIKE N'%Giáo sư%' THEN 320
    WHEN @ten LIKE N'%chính%' OR @ten LIKE N'%GVC%' OR @ten LIKE N'%PGS%' THEN 300
    ELSE 270
  END;
END;
GO

/* Lấy chức danh của GV theo kỳ (ưu tiên lịch sử), rồi áp dụng giảm tải chức vụ */
CREATE OR ALTER FUNCTION dbo.ufn_GioChuanSauGiamTai_Ky
(
  @MaGV CHAR(10), @NamHoc NVARCHAR(10), @HocKy NVARCHAR(10)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
  DECLARE @MaChucDanh CHAR(10) =
   (SELECT TOP(1) MaChucDanh FROM dbo.CHUCDANH_GV_KY 
     WHERE MaGV=@MaGV AND NamHoc=@NamHoc AND HocKy=@HocKy);
  IF @MaChucDanh IS NULL
    SET @MaChucDanh = (SELECT TOP(1) MaChucDanh FROM dbo.GIANGVIEN WHERE MaGV=@MaGV);

  DECLARE @gio INT = dbo.ufn_GioChuanCoSo_ByChucDanh(@MaChucDanh);
  DECLARE @MaChucVu CHAR(10) = (SELECT TOP(1) MaChucVu FROM dbo.GIANGVIEN WHERE MaGV=@MaGV);
  DECLARE @hsGiam DECIMAL(6,4) = dbo.ufn_HeSoGiamTaiTheoChucVu(@MaChucVu);

  RETURN @gio * @hsGiam;
END;
GO

/* Hệ số cộng bù tổng hợp (theo Lớp/Ngôn ngữ/Loại HV/Ca kíp) */
CREATE OR ALTER FUNCTION dbo.ufn_HeSoTong_Additive
(
  @MaLop CHAR(10), @MaNgonNgu CHAR(10), @MaLoaiHV CHAR(10), @MaCaKip CHAR(10)
)
RETURNS DECIMAL(6,3)
AS
BEGIN
  DECLARE @hs DECIMAL(6,3) = 1.0;

  -- Ngôn ngữ: EN/RU +0.1
  IF (SELECT TOP 1 TenNgonNgu FROM dbo.NGONNGU WHERE MaNgonNgu=@MaNgonNgu) IN (N'Tiếng Anh',N'Anh',N'English',N'Tiếng Nga',N'Nga',N'Russian')
      SET @hs += 0.1;

  -- Loại học viên: Lào +0.1
  IF (SELECT TOP 1 TenLoaiHocVien FROM dbo.LOAIHOCVIEN WHERE MaLoaiHocVien=@MaLoaiHV) LIKE N'%Lào%'
      SET @hs += 0.1;

  -- Ca kíp: Tối/Trưa 12h +0.1
  IF (SELECT TOP 1 TenCaKip FROM dbo.CAKIP WHERE MaCaKip=@MaCaKip) LIKE N'%tối%' COLLATE Vietnamese_CI_AI
     OR (SELECT TOP 1 TenCaKip FROM dbo.CAKIP WHERE MaCaKip=@MaCaKip) LIKE N'%trưa%' COLLATE Vietnamese_CI_AI
      SET @hs += 0.1;

  -- Quân số: >100 +0.2; >75 +0.1
  DECLARE @q INT = (SELECT TOP 1 QuanSo FROM dbo.LOP WHERE MaLop=@MaLop);
  IF @q IS NOT NULL
  BEGIN
    IF @q > 100 SET @hs += 0.2;
    ELSE IF @q > 75 SET @hs += 0.1;
  END

  RETURN @hs;
END;
GO

/* Trọng số vai trò (giảng chính/phụ) – bạn chỉnh tùy quy định */
GO
CREATE OR ALTER FUNCTION dbo.ufn_HeSoVaiTro (@VaiTro NVARCHAR(50))
RETURNS DECIMAL(6,3)
AS
BEGIN
    DECLARE @v NVARCHAR(50) = ISNULL(LTRIM(RTRIM(@VaiTro)), N'');

    -- Nếu chuỗi là số (vd '0.8'), ưu tiên dùng trực tiếp
IF TRY_CAST(@v AS DECIMAL(6,3)) IS NOT NULL
    BEGIN
        DECLARE @n DECIMAL(6,3) = TRY_CAST(@v AS DECIMAL(6,3));
        IF @n > 0 AND @n <= 2 RETURN @n;  -- giới hạn an toàn
    END

    -- So khớp không dấu + không phân biệt hoa/thường
    IF @v COLLATE Vietnamese_CI_AI LIKE N'%PHU%'       RETURN 0.5; -- PHU / PHỤ
    IF @v COLLATE Vietnamese_CI_AI LIKE N'%CHINH%'     RETURN 1.0; -- CHÍNH
    IF @v COLLATE Vietnamese_CI_AI LIKE N'%DONG GIANG%' 
       OR @v COLLATE Vietnamese_CI_AI LIKE N'%DONGG%' 
       OR @v COLLATE Vietnamese_CI_AI LIKE N'%ĐONG GI%' 
       OR @v COLLATE Vietnamese_CI_AI LIKE N'%ĐỒNG GIẢNG%' RETURN 0.7; -- Đồng giảng
    IF @v COLLATE Vietnamese_CI_AI LIKE N'%HUONG DAN%' 
       OR @v COLLATE Vietnamese_CI_AI LIKE N'%HUONGDAN%' 
       OR @v COLLATE Vietnamese_CI_AI LIKE N'%HƯỚNG DẪN%' RETURN 0.3; -- Hướng dẫn

    -- Mặc định
    RETURN 1.0;
END
GO


----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------

go


/* Chi tiết giờ quy đổi (đơn vị: GIỜ) cho từng dòng TKBCT x GV */
CREATE OR ALTER VIEW dbo.v_GioQuyDoi_ChiTiet
AS
SELECT
  pc.MaGV,
  tkb.NamHoc,
  tkb.HocKy,
  ct.MaTKBCT,
  ct.NgayHoc,
  ct.SoTiet,
  hs.HeSoTong,
  hv.HeSoVaiTro,
  GioQuyDoi = CAST( ct.SoTiet * dbo.ufn_GioPerTiet()
                    * hs.HeSoTong
                    * hv.HeSoVaiTro AS DECIMAL(10,2))
FROM dbo.PHANCONG      AS pc
JOIN dbo.TKB_CHITIET   AS ct  ON ct.MaTKBCT = pc.MaTKBCT
JOIN dbo.THOIKHOABIEU  AS tkb ON tkb.MaTKB  = ct.MaTKB
CROSS APPLY (
    SELECT HeSoTong = ISNULL(dbo.ufn_HeSoTong_Additive(ct.MaLop, ct.MaNgonNgu, ct.MaLoaiHocVien, ct.MaCaKip), 1.0)
) hs
CROSS APPLY (
    SELECT HeSoVaiTro = ISNULL(dbo.ufn_HeSoVaiTro(pc.VaiTro), 1.0)
) hv;
GO

/* Tổng giờ quy đổi theo GV – Năm – Kỳ */
CREATE OR ALTER VIEW dbo.v_TongGioQuyDoi_GV_NamKy
AS
SELECT MaGV, NamHoc, HocKy, SUM(GioQuyDoi) AS TongGioQuyDoi
FROM dbo.v_GioQuyDoi_ChiTiet
GROUP BY MaGV, NamHoc, HocKy;
GO


----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------

USE thulai;
GO
CREATE OR ALTER FUNCTION dbo.ufn_GioPerTiet()
RETURNS DECIMAL(6,4)
AS
BEGIN
  RETURN CONVERT(DECIMAL(6,4), 0.75);
END
GO


----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------



GO
CREATE OR ALTER FUNCTION dbo.ufn_HeSoTong_Additive
(
@MaLop CHAR(10), @MaNgonNgu CHAR(10), @MaLoaiHV CHAR(10), @MaCaKip CHAR(10)
)
RETURNS DECIMAL(6,3)
AS
BEGIN
  DECLARE @hs DECIMAL(6,3) = 1.0;
  IF (SELECT TOP 1 TenNgonNgu FROM dbo.NGONNGU WHERE MaNgonNgu=@MaNgonNgu)
       IN (N'Tiếng Anh',N'Anh',N'English',N'Tiếng Nga',N'Nga',N'Russian') SET @hs += 0.1;
  IF (SELECT TOP 1 TenLoaiHocVien FROM dbo.LOAIHOCVIEN WHERE MaLoaiHocVien=@MaLoaiHV) LIKE N'%Lào%' SET @hs += 0.1;
  IF (SELECT TOP 1 TenCaKip FROM dbo.CAKIP WHERE MaCaKip=@MaCaKip) COLLATE Vietnamese_CI_AI LIKE N'%tối%' 
     OR (SELECT TOP 1 TenCaKip FROM dbo.CAKIP WHERE MaCaKip=@MaCaKip) COLLATE Vietnamese_CI_AI LIKE N'%trưa%' SET @hs += 0.1;
  DECLARE @q INT = (SELECT TOP 1 QuanSo FROM dbo.LOP WHERE MaLop=@MaLop);
  IF @q IS NOT NULL
  BEGIN
    IF @q > 100 SET @hs += 0.2;
    ELSE IF @q > 75 SET @hs += 0.1;
  END
  RETURN @hs;
END
GO

GO
CREATE OR ALTER FUNCTION dbo.ufn_HeSoVaiTro (@VaiTro NVARCHAR(50))
RETURNS DECIMAL(6,3)
AS
BEGIN
  DECLARE @v NVARCHAR(50) = ISNULL(LTRIM(RTRIM(@VaiTro)), N'');
  IF TRY_CAST(@v AS DECIMAL(6,3)) IS NOT NULL
  BEGIN
    DECLARE @n DECIMAL(6,3) = TRY_CAST(@v AS DECIMAL(6,3));
    IF @n > 0 AND @n <= 2 RETURN @n;
  END
  IF @v COLLATE Vietnamese_CI_AI LIKE N'%PHU%' RETURN 0.5;
  IF @v COLLATE Vietnamese_CI_AI LIKE N'%CHINH%' RETURN 1.0;
  IF @v COLLATE Vietnamese_CI_AI LIKE N'%DONG GIANG%' OR @v COLLATE Vietnamese_CI_AI LIKE N'%ĐỒNG GIẢNG%' RETURN 0.7;
  IF @v COLLATE Vietnamese_CI_AI LIKE N'%HUONG DAN%' OR @v COLLATE Vietnamese_CI_AI LIKE N'%HƯỚNG DẪN%' RETURN 0.3;
  RETURN 1.0;
END
GO


----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------


-- drop & tạo lại view, dùng 3-part name để chắc chắn đúng DB
USE thulai;
GO
DROP VIEW IF EXISTS dbo.v_GioQuyDoi_ChiTiet;
GO
CREATE VIEW dbo.v_GioQuyDoi_ChiTiet
AS
SELECT
  pc.MaGV,
  tkb.NamHoc,
  tkb.HocKy,
  ct.MaTKBCT,
  ct.NgayHoc,
  ct.SoTiet,
  hs.HeSoTong,
  hv.HeSoVaiTro,
  GioQuyDoi = CAST(ct.SoTiet * thulai.dbo.ufn_GioPerTiet()
                   * hs.HeSoTong
                   * hv.HeSoVaiTro AS DECIMAL(10,2))
FROM dbo.PHANCONG     AS pc
JOIN dbo.TKB_CHITIET  AS ct  ON ct.MaTKBCT = pc.MaTKBCT
JOIN dbo.THOIKHOABIEU AS tkb ON tkb.MaTKB  = ct.MaTKB
CROSS APPLY (SELECT HeSoTong   = ISNULL(thulai.dbo.ufn_HeSoTong_Additive(ct.MaLop, ct.MaNgonNgu, ct.MaLoaiHocVien, ct.MaCaKip), 1.0)) hs
CROSS APPLY (SELECT HeSoVaiTro = ISNULL(thulai.dbo.ufn_HeSoVaiTro(pc.VaiTro), 1.0)) hv;
GO

DROP VIEW IF EXISTS dbo.v_TongGioQuyDoi_GV_NamKy;
GO
CREATE VIEW dbo.v_TongGioQuyDoi_GV_NamKy
AS
SELECT MaGV, NamHoc, HocKy, SUM(GioQuyDoi) AS TongGioQuyDoi
FROM dbo.v_GioQuyDoi_ChiTiet
GROUP BY MaGV, NamHoc, HocKy;
GO

EXEC sys.sp_refreshview 'dbo.v_GioQuyDoi_ChiTiet';
EXEC sys.sp_refreshview 'dbo.v_TongGioQuyDoi_GV_NamKy';
----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------


-- proc tính vượt tải
/* Tính vượt tải cho 1 (MaGV, NamHoc, HocKy) – dùng nội bộ */
GO
CREATE OR ALTER PROCEDURE dbo.sp_RecalcVuotTai_ForGVKy
  @MaGV  CHAR(10),
  @NamHoc NVARCHAR(10),
  @HocKy  NVARCHAR(10)
AS
BEGIN
  SET NOCOUNT ON;

  -- LẤY TỔNG GIỜ THỰC TẾ TỪ VIEW: cột đúng là TongGioQuyDoi
  DECLARE @GioTT DECIMAL(10,2) =
    ISNULL((
      SELECT TOP (1) v.TongGioQuyDoi
      FROM dbo.v_TongGioQuyDoi_GV_NamKy v
      WHERE v.MaGV=@MaGV AND v.NamHoc=@NamHoc AND v.HocKy=@HocKy
    ), 0);

  DECLARE @GioChuan DECIMAL(10,2) =
    dbo.ufn_GioChuanSauGiamTai_Ky(@MaGV,@NamHoc,@HocKy);

  MERGE dbo.TINHVUOTTAI AS T
  USING (SELECT @MaGV MaGV, @NamHoc NamHoc, @HocKy HocKy, @GioChuan GioChuan, @GioTT GioTT) S
    ON (T.MaGV=S.MaGV AND T.NamHoc=S.NamHoc AND ISNULL(T.HocKy,'')=ISNULL(S.HocKy,''))
  WHEN MATCHED THEN
    UPDATE SET T.TongGioChuan=S.GioChuan, T.TongGioThucTe=S.GioTT, T.NgayTinh=SYSDATETIME()
  WHEN NOT MATCHED THEN
    INSERT (MaTinhVuotTai, MaGV, NamHoc, HocKy, TongGioChuan, TongGioThucTe, NgayTinh)
    VALUES (CONCAT(S.MaGV,'-',S.NamHoc,'-',ISNULL(S.HocKy,'')),
            S.MaGV, S.NamHoc, S.HocKy, S.GioChuan, S.GioTT, SYSDATETIME());
END
GO


GO
CREATE OR ALTER PROCEDURE dbo.sp_TinhVuotTai
  @NamHoc NVARCHAR(10),
  @HocKy  NVARCHAR(10) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  IF @HocKy IS NOT NULL
  BEGIN
    DECLARE @aff TABLE (MaGV CHAR(10), HocKy NVARCHAR(10));
    INSERT INTO @aff(MaGV,HocKy)
    SELECT DISTINCT pc.MaGV, tkb.HocKy
    FROM dbo.PHANCONG pc
    JOIN dbo.TKB_CHITIET ct ON ct.MaTKBCT=pc.MaTKBCT
    JOIN dbo.THOIKHOABIEU tkb ON tkb.MaTKB=ct.MaTKB
    WHERE tkb.NamHoc=@NamHoc AND tkb.HocKy=@HocKy;

    DECLARE @MaGV CHAR(10), @Ky NVARCHAR(10);
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT MaGV, HocKy FROM @aff;
    OPEN cur; FETCH NEXT FROM cur INTO @MaGV, @Ky;
    WHILE @@FETCH_STATUS = 0
    BEGIN
      EXEC dbo.sp_RecalcVuotTai_ForGVKy @MaGV=@MaGV, @NamHoc=@NamHoc, @HocKy=@Ky;
      FETCH NEXT FROM cur INTO @MaGV, @Ky;
    END
    CLOSE cur; DEALLOCATE cur;

    SELECT MaGV, NamHoc, HocKy,
           TongGioChuan, TongGioThucTe,
           GioVuotTai = CASE WHEN TongGioThucTe>TongGioChuan THEN ROUND(TongGioThucTe-TongGioChuan,2) ELSE 0 END
    FROM dbo.TINHVUOTTAI
    WHERE NamHoc=@NamHoc AND HocKy=@HocKy
    ORDER BY MaGV;
    RETURN;
  END

  ;WITH GIO AS (
    SELECT gv.MaGV, k.HocKy,
           -- ĐÚNG CỘT: TongGioQuyDoi từ view tổng
           GioThucTe = ISNULL((
              SELECT TOP (1) v.TongGioQuyDoi
              FROM dbo.v_TongGioQuyDoi_GV_NamKy v
              WHERE v.MaGV=gv.MaGV AND v.NamHoc=@NamHoc AND v.HocKy=k.HocKy
           ),0),
GioChuan  = dbo.ufn_GioChuanSauGiamTai_Ky(gv.MaGV,@NamHoc,k.HocKy)
    FROM dbo.GIANGVIEN gv
    CROSS APPLY (VALUES (N'HK1'), (N'HK2')) k(HocKy)
  ),
  Gop AS (
    SELECT MaGV, @NamHoc AS NamHoc,
           GioThucTe = SUM(GioThucTe),
           GioChuan  = MAX(GioChuan)
    FROM GIO GROUP BY MaGV
  )
  MERGE dbo.TINHVUOTTAI AS T
  USING Gop S
    ON T.MaGV=S.MaGV AND T.NamHoc=S.NamHoc AND T.HocKy IS NULL
  WHEN MATCHED THEN
    UPDATE SET T.TongGioChuan=S.GioChuan, T.TongGioThucTe=S.GioThucTe, T.NgayTinh=SYSDATETIME()
  WHEN NOT MATCHED THEN
    INSERT (MaTinhVuotTai, MaGV, NamHoc, HocKy, TongGioChuan, TongGioThucTe, NgayTinh)
    VALUES (CONCAT(S.MaGV,'-',S.NamHoc), S.MaGV, S.NamHoc, NULL, S.GioChuan, S.GioThucTe, SYSDATETIME());

  SELECT MaGV, NamHoc, HocKy=NULL,
         TongGioChuan, TongGioThucTe,
         GioVuotTai = CASE WHEN TongGioThucTe>TongGioChuan THEN ROUND(TongGioThucTe-TongGioChuan,2) ELSE 0 END
  FROM dbo.TINHVUOTTAI
  WHERE NamHoc=@NamHoc AND HocKy IS NULL
  ORDER BY MaGV;
END
GO






----------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------



-- trigger
GO
CREATE OR ALTER TRIGGER dbo.trg_PHANCONG_AUD_UpdateVuotTai
ON dbo.PHANCONG
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @aff TABLE (MaGV CHAR(10), NamHoc NVARCHAR(10), HocKy NVARCHAR(10));

  -- Lấy (MaGV, MaTKBCT) bị ảnh hưởng từ inserted/deleted
  ;WITH X AS (
    SELECT MaGV, MaTKBCT FROM inserted
    UNION
    SELECT MaGV, MaTKBCT FROM deleted
  )
  INSERT INTO @aff(MaGV, NamHoc, HocKy)
  SELECT DISTINCT x.MaGV, tkb.NamHoc, tkb.HocKy
  FROM X
  JOIN dbo.TKB_CHITIET  ct  ON ct.MaTKBCT = x.MaTKBCT
  JOIN dbo.THOIKHOABIEU tkb ON tkb.MaTKB  = ct.MaTKB;

  DECLARE @MaGV CHAR(10), @Nam NVARCHAR(10), @Ky NVARCHAR(10);
  DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT DISTINCT MaGV, NamHoc, HocKy FROM @aff;
  OPEN cur; FETCH NEXT FROM cur INTO @MaGV, @Nam, @Ky;
  WHILE @@FETCH_STATUS=0
  BEGIN
    EXEC dbo.sp_RecalcVuotTai_ForGVKy @MaGV=@MaGV, @NamHoc=@Nam, @HocKy=@Ky;
    FETCH NEXT FROM cur INTO @MaGV, @Nam, @Ky;
  END
  CLOSE cur; DEALLOCATE cur;
END
GO



GO
CREATE OR ALTER TRIGGER dbo.trg_PHANCONG_AUD_UpdateVuotTai
ON dbo.PHANCONG
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @aff TABLE (MaGV CHAR(10), NamHoc NVARCHAR(10), HocKy NVARCHAR(10));

  -- Lấy (MaGV, MaTKBCT) bị ảnh hưởng từ inserted/deleted
  ;WITH X AS (
    SELECT MaGV, MaTKBCT FROM inserted
    UNION
    SELECT MaGV, MaTKBCT FROM deleted
  )
  INSERT INTO @aff(MaGV, NamHoc, HocKy)
  SELECT DISTINCT x.MaGV, tkb.NamHoc, tkb.HocKy
  FROM X
  JOIN dbo.TKB_CHITIET  ct  ON ct.MaTKBCT = x.MaTKBCT
  JOIN dbo.THOIKHOABIEU tkb ON tkb.MaTKB  = ct.MaTKB;

  DECLARE @MaGV CHAR(10), @Nam NVARCHAR(10), @Ky NVARCHAR(10);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT DISTINCT MaGV, NamHoc, HocKy FROM @aff;
  OPEN cur; FETCH NEXT FROM cur INTO @MaGV, @Nam, @Ky;
  WHILE @@FETCH_STATUS=0
  BEGIN
    EXEC dbo.sp_RecalcVuotTai_ForGVKy @MaGV=@MaGV, @NamHoc=@Nam, @HocKy=@Ky;
    FETCH NEXT FROM cur INTO @MaGV, @Nam, @Ky;
  END
  CLOSE cur; DEALLOCATE cur;
END
GO

EXEC dbo.sp_TinhVuotTai @NamHoc=N'2025-2026', @HocKy=N'HK1';
EXEC dbo.sp_TinhVuotTai @NamHoc=N'2025-2026', @HocKy=NULL;








USE thulai;
GO

/* 1.1. Bảng DONVI */
IF OBJECT_ID('dbo.DONVI','U') IS NULL
BEGIN
  CREATE TABLE dbo.DONVI (
    MaDonVi   CHAR(10)       NOT NULL PRIMARY KEY,
    TenDonVi  NVARCHAR(200)  NOT NULL
  );
END
GO

/* 1.2. Thêm cột MaDonVi vào GIANGVIEN (nếu chưa có) */
IF COL_LENGTH('dbo.GIANGVIEN','MaDonVi') IS NULL
BEGIN
  ALTER TABLE dbo.GIANGVIEN ADD MaDonVi CHAR(10) NULL;
END
GO

/* 1.3. Khóa ngoại GIANGVIEN -> DONVI (nếu chưa có) */
IF NOT EXISTS (
  SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_GV_DONVI'
)
BEGIN
  ALTER TABLE dbo.GIANGVIEN
  ADD CONSTRAINT FK_GV_DONVI
  FOREIGN KEY (MaDonVi) REFERENCES dbo.DONVI(MaDonVi);
END
GO

/* 1.4. Chỉ mục gợi ý cho truy vấn theo đơn vị */
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes WHERE name = 'IX_GV_MaDonVi' AND object_id = OBJECT_ID('dbo.GIANGVIEN')
)
BEGIN
  CREATE INDEX IX_GV_MaDonVi ON dbo.GIANGVIEN(MaDonVi);
END
GO

MERGE dbo.DONVI AS T
USING (VALUES
  ('KHOA01', N'Khoa Cơ bản'),
  ('KHOA02', N'Khoa Chuyên ngành'),
  ('TT01',   N'Trung tâm Ngoại ngữ')
) S(MaDonVi, TenDonVi)
ON T.MaDonVi = S.MaDonVi
WHEN MATCHED THEN UPDATE SET T.TenDonVi = S.TenDonVi
WHEN NOT MATCHED THEN INSERT (MaDonVi, TenDonVi) VALUES (S.MaDonVi, S.TenDonVi);
GO