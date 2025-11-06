using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Keyless]
public partial class v_VuotTai_TongQuan
{
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

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? GioChuan { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal? GioThucTe { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal? GioVuot { get; set; }

    [Column(TypeName = "decimal(38, 4)")]
    public decimal? ThanhTien { get; set; }
}
