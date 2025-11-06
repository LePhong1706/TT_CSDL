namespace Web_vuottai.Models
{
    using System.ComponentModel.DataAnnotations;
    public class GiangVienUpsertVm
    {
        // Thông tin GV
        public int? GiangVienId { get; set; }   // null = Create
        [Required, StringLength(20)] public string MaGV { get; set; } = "";
        [Required, StringLength(100)] public string HoTen { get; set; } = "";
        [Display(Name = "Đơn vị")] public int? DonViId { get; set; }
        [Display(Name = "Chức vụ")] public int? ChucVuId { get; set; }

        // Mapping chức danh theo kỳ
        [Required, Display(Name = "Chức danh")] public int ChucDanhId { get; set; }
        [Required, Display(Name = "Năm học")] public string NamHoc { get; set; } = "2025-2026";
        [Required, Range(1, 2), Display(Name = "Học kỳ")] public int HocKy { get; set; } = 1;
    }
}
