namespace Web_vuottai.Models
{
    public class PhanCongEditVm
    {
        public int? PhanCongId { get; set; }  // null = Create
        public int? TkbCtId { get; set; }     // null = Create

        // TKB header
        public string NamHoc { get; set; } = "2025-2026";
        public int HocKy { get; set; } = 1;

        // Chi tiết
        public int LopId { get; set; }
        public int MonHocId { get; set; }
        public int LoaiHVId { get; set; }
        public int NgonNguId { get; set; }
        public int CaKipId { get; set; }
        public DateTime NgayHoc { get; set; }
        public int SoTiet { get; set; }

        // Phân công
        public int GiangVienId { get; set; }
        public string? VaiTro { get; set; } = "CHINH";
    }
}
