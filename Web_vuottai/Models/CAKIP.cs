    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    namespace Web_vuottai.Models;

    [Table("CAKIP")]
    public partial class CAKIP
    {
        [Key]
        public int CaKipId { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập tên ca kíp")]
        [Display(Name = "Tên ca kíp")]
        public string? TenCaKip { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        [Required(ErrorMessage = "Vui lòng nhập hệ số")]
        [Display(Name = "Hệ số")]
        public decimal HeSo { get; set; }

        [InverseProperty("CaKip")]
        public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
    }
