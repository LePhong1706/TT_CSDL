using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("GIANGVIEN")]
[Index("MaGV", Name = "UQ__GIANGVIE__2725AEF2BF586937", IsUnique = true)]
public partial class GIANGVIEN
{
    [Key]
    public int GiangVienId { get; set; }

    [StringLength(50)]
    public string? MaGV { get; set; }

    [StringLength(200)]
    public string? HoTen { get; set; }

    public int? DonViId { get; set; }

    public int? ChucVuId { get; set; }

    [ForeignKey("ChucVuId")]
    [InverseProperty("GIANGVIENs")]
    public virtual CHUCVU? ChucVu { get; set; }

    [ForeignKey("DonViId")]
    [InverseProperty("GIANGVIENs")]
    public virtual DONVI? DonVi { get; set; }

    [InverseProperty("GiangVien")]
    public virtual ICollection<GV_CHUCDANH> GV_CHUCDANHs { get; set; } = new List<GV_CHUCDANH>();

    [InverseProperty("GiangVien")]
    public virtual ICollection<PHANCONG> PHANCONGs { get; set; } = new List<PHANCONG>();

    [InverseProperty("GiangVien")]
    public virtual ICollection<VUOTTAI_SUM> VUOTTAI_SUMs { get; set; } = new List<VUOTTAI_SUM>();
}
