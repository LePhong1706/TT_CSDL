using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using Web_vuottai.Models;

namespace Web_vuottai.Services.Pdf
{
    // Model đã được nhóm
    using GroupedVuotTaiData = Dictionary<string, List<v_VuotTai_TongQuan>>;

    public class VuotTaiPdfDocument : IDocument
    {
        private readonly GroupedVuotTaiData _model;
        private readonly string? _namHoc;
        private readonly int? _hocKy;
        private readonly CultureInfo _vi = CultureInfo.GetCultureInfo("vi-VN");

        // TextStyle cố định
        private static TextStyle TitleStyle { get; } = TextStyle.Default.FontSize(13).Bold();
        private static TextStyle HeaderStyle { get; } = TextStyle.Default.FontSize(10).Bold();
        private static TextStyle CellStyle { get; } = TextStyle.Default.FontSize(10);
        private static TextStyle GroupHeaderStyle { get; } = TextStyle.Default.FontSize(10).Bold().Italic();

        public VuotTaiPdfDocument(GroupedVuotTaiData model, string? namHoc, int? hocKy)
        {
            _model = model;
            _namHoc = namHoc;
            _hocKy = hocKy;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // HÀM MỚI
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Cấu hình trang (A4 ngang)
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontFamily("Times New Roman").Fallback(f => f.FontFamily("Arial")));

                page.Header().Element(ComposeHeader);

                // Content (Nội dung) sẽ chứa cả Bảng và Chữ ký
                page.Content().Element(ComposeContent);

                // Footer (Chân trang) giờ chỉ dùng để đánh số trang
                page.Footer().AlignRight().Text(t =>
                {
                    t.Span("Trang ");
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("BỘ QUỐC PHÒNG");
                    col.Item().AlignCenter().Text("HỌC VIỆN KỸ THUẬT QUÂN SỰ").Bold();
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignCenter().Text("Mẫu số CXX-HD").Bold();
                    col.Item().AlignCenter().Text($"Số: .................... / {DateTime.Now.Year}");
                });
            });
        }

        // HÀM MỚI
        void ComposeContent(IContainer container)
        {
            // Tiêu đề chính
            var title = "BẢNG KÊ THANH TOÁN VƯỢT TẢI CHO GIẢNG VIÊN";
            var term = string.IsNullOrEmpty(_namHoc) ? "Toàn thời gian" : $"Năm học {_namHoc}" + (_hocKy.HasValue ? $" - Học kỳ {_hocKy}" : "");

            container.PaddingVertical(10).Column(col =>
            {
                col.Item().AlignCenter().Text(title).Style(TitleStyle);
                col.Item().AlignCenter().Text(term).Style(HeaderStyle.Italic());
                col.Spacing(10);

                // 1. Vẽ Bảng
                col.Item().Element(ComposeTable);

                // 2. Vẽ Chữ ký (Footer cũ) ngay sau Bảng
                col.Item().Element(ComposeFooter);
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // Định nghĩa cột
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(40);  // STT
                    cols.RelativeColumn(2.5f); // Họ và tên
                    cols.RelativeColumn(1.5f); // Chức vụ
                    cols.RelativeColumn(1.5f); // Chức danh
                    cols.ConstantColumn(80);  // Năm Học
                    cols.ConstantColumn(50);  // Học Kì
                    cols.RelativeColumn(1.5f); // Số tiền
                    cols.RelativeColumn(1.2f); // Ký nhận
                });

                // Vẽ Header Bảng
                table.Header(h =>
                {
                    static IContainer HeaderCell(IContainer c) => c.Border(0.5f).BorderColor(Colors.Black).Background(Colors.Grey.Lighten3).AlignCenter().AlignMiddle().Padding(2);

                    h.Cell().Element(HeaderCell).Text("TT").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Họ và tên").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Chức vụ").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Chức danh").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Năm Học").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Học Kì").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Số tiền").Style(HeaderStyle);
                    h.Cell().Element(HeaderCell).Text("Ký nhận").Style(HeaderStyle);
                });

                // --- Vẽ Nội dung Bảng ---
                static IContainer BodyCell(IContainer c, bool isRight = false)
                {
                    var cell = c.BorderBottom(0.5f).BorderLeft(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(2);
                    return isRight ? cell.AlignRight() : cell.AlignLeft();
                }

                int groupIndex = 0;
                decimal totalSum = 0;

                // Lặp qua từng Đơn vị (Group)
                foreach (var group in _model)
                {
                    groupIndex++;

                    // Dòng tiêu đề Nhóm (I. VIỆN CÔNG NGHỆ THÔNG TIN)
                    table.Cell().ColumnSpan(8).Background(Colors.Grey.Lighten4).Border(0.5f).BorderColor(Colors.Black).Padding(2)
                        .Text($"{IntToRoman(groupIndex)}. {group.Key.ToUpper()}").Style(GroupHeaderStyle);

                    int itemIndex = 0;

                    // Lặp qua từng Giảng viên trong Nhóm
                    foreach (var r in group.Value)
                    {
                        itemIndex++;
                        table.Cell().Element(c => BodyCell(c)).AlignCenter().Text(itemIndex.ToString()).Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c)).Text(r.HoTen).Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c)).Text(r.TenChucVu).Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c)).Text(r.TenChucDanh).Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c)).AlignCenter().Text(r.NamHoc).Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c)).AlignCenter().Text(r.HocKy?.ToString() ?? "-").Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c, true)).Text(r.ThanhTien?.ToString("N0", _vi) ?? "0").Style(CellStyle);
                        table.Cell().Element(c => BodyCell(c)).Text("").Style(CellStyle); // Cột Ký nhận
                    }



                    totalSum += group.Value.Sum(r => r.ThanhTien ?? 0);
                }

                // --- Dòng tổng cộng ---
                table.Cell().ColumnSpan(6).Border(0.5f).BorderColor(Colors.Black).AlignRight().Padding(2).Text("Tổng cộng").Style(HeaderStyle);
                table.Cell().Element(c => BodyCell(c, true)).Text(totalSum.ToString("N0", _vi)).Style(HeaderStyle);
                table.Cell().Element(c => BodyCell(c)).Text("").Style(HeaderStyle);

            });
        }

        void ComposeFooter(IContainer container)
        {
            container.PaddingTop(10).Column(col =>
            {
                // Dòng tổng tiền bằng chữ
                col.Item().Text($"Tổng số tiền (viết bằng chữ): {NumberToWords(Convert.ToInt64(Math.Round(_model.SelectMany(g => g.Value).Sum(r => r.ThanhTien ?? 0))))} đồng.");

                col.Spacing(5);

                col.Item().AlignCenter().Text($"                                                                                                                                 Hà Nội, Ngày {DateTime.Now.Day:D2} tháng {DateTime.Now.Month:D2} năm {DateTime.Now.Year}");

                col.Spacing(10);

                // Dòng ký tên
                col.Item().Row(row =>
                {
                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().Text("NGƯỜI CẤP").Style(HeaderStyle);
                    });

                    row.RelativeItem().AlignCenter().Column(c =>
                    {
                        c.Item().Text("            TL. GIÁM ĐỐC").Style(HeaderStyle);
                        c.Item().Text("TRƯỞNG PHÒNG ĐÀO TẠO").Style(HeaderStyle);
                    });
                });

                // Dòng tên người ký
                col.Item().PaddingTop(50).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Text("Đại úy Lê Trần Đăng Phong").Style(HeaderStyle);
                    row.RelativeItem().AlignCenter().Text("Đại tá Phan Văn Trung").Style(HeaderStyle);
                });

            });
        }

        // --- Hàm tiện ích ---

        private static string IntToRoman(int n)
        {
            // (Hàm này có thể được triển khai chi tiết hơn nếu cần)
            var map = new[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            var val = new[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string res = "";
            for (int i = 0; i < 13; i++) { while (n >= val[i]) { res += map[i]; n -= val[i]; } }
            return res;
        }

        // Hàm đọc số (đơn giản, cần thư viện chuyên dụng nếu muốn chính xác tuyệt đối)
        private static string NumberToWords(long number)
        {
            if (number == 0) return "Không";
            if (number < 0) return "âm " + NumberToWords(Math.Abs(number));
            string words = "";
            string[] unitsMap = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] powersMap = { "", " nghìn", " triệu", " tỷ" };
            int powerIndex = 0;

            do
            {
                long n = number % 1000;
                number /= 1000; // number bây giờ là các khối còn lại

                if (n == 0)
                {
                    powerIndex++;
                    continue; // Bỏ qua khối 000
                }

                string part = "";
                long hundreds = n / 100;
                long tens = (n % 100) / 10;
                long units = n % 10;

                // 1. Hàng Trăm
                if (hundreds > 0)
                {
                    part = unitsMap[hundreds] + " trăm";
                }
                // *** SỬA LOGIC "KHÔNG TRĂM" ***
                // Thêm "không trăm" nếu: 
                // 1. Hàng trăm = 0
                // 2. Hàng chục hoặc đơn vị > 0
                // 3. Và quan trọng nhất: Đây KHÔNG PHẢI khối cuối cùng (tức là 'number' (phần còn lại) > 0)
                else if (hundreds == 0 && (tens > 0 || units > 0) && number > 0)
                {
                    part = "không trăm";
                }

                // 2. Hàng Chục
                if (tens > 1) // 20-99
                {
                    part += (part == "" ? "" : " ") + unitsMap[tens] + " mươi";
                }
                else if (tens == 1) // 10-19
                {
                    part += (part == "" ? "" : " ") + "mười";
                }
                else if (tens == 0 && units > 0) // 101-109 hoặc 001, 002
                {
                    // Thêm "linh" nếu có "trăm" (X trăm hoặc không trăm)
                    if (part != "")
                    {
                        part += " linh";
                    }
                }

                // 3. Hàng Đơn vị
                if (units == 5)
                {
                    if (tens > 0) // 15, 25, 35...
                        part += " lăm";
                    else // 5, 105, 205...
                        part += (part == "" ? "" : " ") + "năm";
                }
                else if (units == 1)
                {
                    if (tens > 1) // 21, 31...
                        part += " mốt";
                    else if (tens == 1) // 11
                        part += " một";
                    else if (units > 0) // 1, 101, 201...
                        part += (part == "" ? "" : " ") + "một";
                }
                else if (units > 0) // 2,3,4,6,7,8,9
                {
                    part += (part == "" ? "" : " ") + unitsMap[units];
                }

                // 4. Thêm tên khối (nghìn, triệu...)
                words = part + powersMap[powerIndex] + " " + words;

                powerIndex++;
            } while (number > 0);

            words = words.Trim();
            // Đảm bảo chữ cái đầu tiên luôn viết hoa
            return char.ToUpper(words[0]) + words.Substring(1);
        }
    }
}
