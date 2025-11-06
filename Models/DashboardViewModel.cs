using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_vuottai.Models
{
    public class ChartData
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Data { get; set; } = new List<decimal>();
    }
    public class DashboardViewModel
    {
        public int TongGiangVien { get; set; }
        public int TongLop { get; set; }
        public decimal TongGioVuot { get; set; }
                public decimal TongThanhTien { get; set; }
                public int SoGiangVienVuotTai { get; set; } // Thêm thuộc tính này
                
                // Dữ liệu cho biểu đồ Top 5 Giảng viên vượt tải
                public ChartData TopVuotTaiChart { get; set; } = new ChartData();
        
                // Dữ liệu cho biểu đồ Tỷ lệ giờ giảng theo Đơn vị
                public ChartData DonViChart { get; set; } = new ChartData();
        
                // Thêm thuộc tính để giữ danh sách chi tiết cho bảng
                public IEnumerable<v_VuotTai_TongQuan> VuotTaiList { get; set; } = new List<v_VuotTai_TongQuan>();
            }
        }
        