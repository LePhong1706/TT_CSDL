using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("VUOTTAI_SUM")]
[Index("GiangVienId", "NamHoc", "HocKy", Name = "UQ_VT", IsUnique = true)]
public partial class VUOTTAI_SUM
{
    [Key]
    public int Id { get; set; }

    public int GiangVienId { get; set; }

    [StringLength(10)]
    public string? NamHoc { get; set; }

    public int? HocKy { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal GioChuan { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal GioThucTe { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal GioVuot { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ThanhTien { get; set; }

    public DateTime LastCalcAt { get; set; }

    [ForeignKey("GiangVienId")]
    [InverseProperty("VUOTTAI_SUMs")]
    public virtual GIANGVIEN GiangVien { get; set; } = null!;
}
