using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("CHUCDANH")]
public partial class CHUCDANH
{
    [Key]
    public int ChucDanhId { get; set; }

    [StringLength(100)] 
    [Display(Name = "Tên Chức Danh")] 
    public string? TenChucDanh { get; set; }

    [InverseProperty("ChucDanh")]
    public virtual ICollection<CHUCDANH_GIOCHUAN> CHUCDANH_GIOCHUANs { get; set; } = new List<CHUCDANH_GIOCHUAN>();

    [InverseProperty("ChucDanh")]
    public virtual ICollection<DONGIAGIO> DONGIAGIOs { get; set; } = new List<DONGIAGIO>();

    [InverseProperty("ChucDanh")]
    public virtual ICollection<GV_CHUCDANH> GV_CHUCDANHs { get; set; } = new List<GV_CHUCDANH>();
}
