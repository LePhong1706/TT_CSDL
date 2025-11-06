using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace Web_vuottai.Models;
[Table("GV_CHUCDANH")]
[Index("GiangVienId", "NamHoc", "HocKy", Name = "UQ_GVCD", IsUnique = true)]
public partial class GV_CHUCDANH
{
    [Key]
    public int GvChucDanhId { get; set; }

    public int GiangVienId { get; set; }

    public int ChucDanhId { get; set; }

    [StringLength(10)]
    [Display(Name = "Năm Học")] // <-- Thêm để hiển thị đẹp
    public string NamHoc { get; set; } = null!;
    [Display(Name = "Học Kỳ")] // <-- Thêm để hiển thị đẹp
    public int HocKy { get; set; }

    [ForeignKey("ChucDanhId")]
    [InverseProperty("GV_CHUCDANHs")]
    public virtual CHUCDANH ChucDanh { get; set; } = null!;

    [ForeignKey("GiangVienId")]
    [InverseProperty("GV_CHUCDANHs")]
    public virtual GIANGVIEN GiangVien { get; set; } = null!;
}
