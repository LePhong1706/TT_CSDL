namespace Web_vuottai.Models
{
    public class GiangVienListVm
    {
        public int GiangVienId { get; set; }
        public string MaGV { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string? TenDonVi { get; set; }
        public string? TenChucVu { get; set; }
        public string? TenChucDanh { get; set; }   // lấy theo năm/kỳ
        public string NamHoc { get; set; } = "";
        public int? HocKy { get; set; }
    }
}
