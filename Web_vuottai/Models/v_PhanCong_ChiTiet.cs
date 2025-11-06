using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Keyless]
[Table("v_PhanCong_ChiTiet", Schema = "dbo")]
public partial class v_PhanCong_ChiTiet
{
    public int PhanCongId { get; set; }

    public int GiangVienId { get; set; }

    [StringLength(50)]
    public string? MaGV { get; set; }

    [StringLength(200)]
    public string? HoTen { get; set; }

    [StringLength(200)]
    public string? TenDonVi { get; set; }

    [StringLength(100)]
    public string? TenChucDanh { get; set; }

    [StringLength(100)]
    public string? TenChucVu { get; set; }

    [StringLength(10)]
    public string? NamHoc { get; set; }

    public int? HocKy { get; set; }

    [StringLength(50)]
    public string? MaLop { get; set; }

    [StringLength(200)]
    public string? TenLop { get; set; }

    public int QuanSo { get; set; }

    [StringLength(200)]
    public string? TenMonHoc { get; set; }

    [StringLength(100)]
    public string? TenLoaiHV { get; set; }

    [StringLength(50)]
    public string? TenNgonNgu { get; set; }

    [StringLength(100)]
    public string? TenCaKip { get; set; }

    public DateOnly NgayHoc { get; set; }

    public int SoTiet { get; set; }

    [StringLength(50)]
    public string? VaiTro { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? GioQuyDoi { get; set; }
}
