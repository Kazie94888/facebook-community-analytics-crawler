using System.Collections.Generic;

namespace LookOn.Core.Helpers;

public static class CityHelper
{
    public static Dictionary<int, List<string>> Names = new()
    {
        {0, new List<string>{"Others"}},
        {1, new List<string>{"Hòa Bình", "hòa bình", "Hoa Binh", "hoa binh", "HoaBinh", "hoabinh"}},
        {2, new List<string>{"Sơn La", "sơn la", "Son La", "son la", "SonLa", "sonla"} },
        {3, new List<string>{"Điện Biên", "điện biên", "Dien Bien", "dien bien", "DienBien", "dienbien"} },
        {4, new List<string>{"Lai Châu", "lai châu", "Lai Chau", "lai chau", "LaiChau", "laichau"} },
        {5, new List<string>{"Lào Cai", "lào cai", "Lao Cai", "lao cai", "LaoCai", "laocai"} },
        {6, new List<string>{"Yên Bái", "yên bái", "Yen Bai", "yen bai", "YenBai", "yenbai"} },
        {7, new List<string>{"Phú Thọ", "phú thọ", "Phu Tho", "phu tho", "PhuTho", "phutho"} },
        {8, new List<string>{"Hà Giang", "hà giang", "Ha Giang", "ha giang", "HaGiang", "hagiang"} },
        {9, new List<string>{"Tuyên Quang", "tuyên quang", "Tuyen Quang", "tuyen quang", "TuyenQuang", "tuyenquang"} },
        {10, new List<string>{"Cao Bằng", "cao bằng", "Cao Bang", "cao bang", "CaoBang", "caobang"} },
        {11, new List<string>{"Bắc Kạn", "bắc kạn", "Bac Kan", "bac kan", "BacKan", "backan"} },
        {12, new List<string>{"Thái Nguyên", "thái nguyên", "Thai Nguyen", "thai nguyen", "ThaiNguyen", "thainguyen"} },
        {13, new List<string>{"Lạng Sơn", "lạng sơn", "Lang Son", "lang son", "LangSon", "langson"} },
        {14, new List<string>{"Bắc Giang", "bắc giang", "Bac Giang", "bac giang", "BacGiang", "bacgiang"} },
        {15, new List<string>{"Quảng Ninh", "quảng ninh", "Quang Ninh", "quang ninh", "QuangNinh", "quangninh"} },
        {16, new List<string>{"Hà Nội", "hà nội", "Ha Noi", "ha noi", "HaNoi", "hanoi"} },
        {17, new List<string>{"Bắc Ninh", "bắc ninh", "Bac Ninh", "bac ninh", "BacNinh", "bacninh"} },
        {18, new List<string>{"Hà Nam", "hà nam", "Ha Nam", "ha nam", "HaNam", "hanam"} },
        {19, new List<string>{"Hải Dương", "hải dương", "Hai Duong", "hai duong", "HaiDuong", "haiduong"} },
        {20, new List<string>{"Hải Phòng", "hải phòng", "Hai Phong", "hai phong", "HaiPhong", "haiphong"} },
        {21, new List<string>{"Hưng Yên", "hưng yên", "Hung Yen", "hung yen", "HungYen", "hungyen"} },
        {22, new List<string>{"Nam Định", "nam định", "Nam Dinh", "nam dinh", "NamDinh", "namdinh"} },
        {23, new List<string>{"Thái Bình", "thái bình", "Thai Binh", "thai binh", "ThaiBinh", "thaibinh"} },
        {24, new List<string>{"Vĩnh Phúc", "vĩnh phúc", "Vinh Phuc", "vinh phuc", "VinhPhuc", "vinhphuc"} },
        {25, new List<string>{"Ninh Bình", "ninh bình", "Ninh Binh", "ninh binh", "NinhBinh", "ninhbinh"} },
        {26, new List<string>{"Thanh Hóa", "thanh hóa", "Thanh Hoa", "thanh hoa", "ThanhHoa", "thanhoa"} },
        {27, new List<string>{"Nghệ An", "nghệ an", "Nghe An", "nghe an", "NgheAn", "nghean"} },
        {28, new List<string>{"Hà Tĩnh", "hà tĩnh", "Ha Tinh", "ha tinh", "HaTinh", "hatinh"} },
        {29, new List<string>{"Quảng Bình", "quảng bình", "Quang Binh", "quang binh", "QuangBinh", "quangbinh"} },
        {30, new List<string>{"Quảng Trị", "quảng trị", "Quang Tri", "quang tri", "QuangTri", "quangtri"} },
        {31, new List<string>{"Thừa Thiên Huế", "thừa thiên huế", "Thua Thien Hue", "thua thien hue", "ThuaThienHue", "thuathienhue"} },
        {32, new List<string>{"Đà Nẵng", "đà nẵng", "Da Nang", "da nang", "DaNang", "danang"} },
        {33, new List<string>{"Quảng Nam", "quảng nam", "Quang Nam", "quang nam", "QuangNam", "quangnam"} },
        {34, new List<string>{"Quảng Ngãi", "quảng ngãi", "Quang Ngai", "quang ngai", "QuangNgai", "quangngai"} },
        {35, new List<string>{"Bình Định", "bình định", "Binh Dinh", "binh dinh", "BinhDinh", "binhdinh"} },
        {36, new List<string>{"Phú Yên", "phú yên", "Phu Yen", "phu yen", "PhuYen", "phuyen"} },
        {37, new List<string>{"Khánh Hòa", "khánh hòa", "Khanh Hoa", "khanh hoa", "KhanhHoa", "khanhhoa"} },
        {38, new List<string>{"Ninh Thuận", "ninh thuận", "Ninh Thuan", "ninh thuan", "NinhThuan", "ninhthuan"} },
        {39, new List<string>{"Bình Thuận", "bình thuận", "Binh Thuan", "binh thuan", "BinhThuan", "binhthuan"} },
        {40, new List<string>{"Kon Tum", "kon tum", "Kon Tum", "kon tum", "KonTum", "kontum"} },
        {41, new List<string>{"Gia Lai", "gia lai", "Gia Lai", "gia lai", "GiaLai", "gialai"} },
        {42, new List<string>{"Đắk Lắk", "đắk lắk", "Dak Lak", "dak lak", "DakLak", "daklak","DacLak","daclak"} },
        {43, new List<string>{"Đắk Nông", "đắk nông", "Dak Nong", "dak nong", "DakNong", "daknong", "DacNong", "dacnong"} },
        {44, new List<string>{"Lâm Đồng", "lâm đồng", "Lam Dong", "lam dong", "LamDong", "lamdong"} },
        {45, new List<string>{"TP Hồ Chí Minh", "tp hồ chí minh", "TP Ho Chi Minh", "tp ho chi minh", "HoChiMinh", "hochiminh"} },
        {46, new List<string>{"Bà Rịa Vũng Tàu", "bà rịa vũng tàu", "Ba Ria Vung Tau", "ba ria vung tau", "BaRiaVungTau", "bariavungtau"} },
        {47, new List<string>{"Bình Dương", "bình dương", "Binh Duong", "binh duong", "BinhDuong", "binhduong"} },
        {48, new List<string>{"Bình Phước", "bình phước", "Binh Phuoc", "binh phuoc", "BinhPhuoc", "binhphuoc"} },
        {49, new List<string>{"Đồng Nai", "đồng nai", "Dong Nai", "dong nai", "DongNai", "dongnai"} },
        {50, new List<string>{"Tây Ninh", "tây ninh", "Tay Ninh", "tay ninh", "TayNinh", "tayninh"} },
        {51, new List<string>{"An Giang", "an gianh", "An Giang", "an giang", "AnGiang", "angiang"} },
        {52, new List<string>{"Bạc Liêu", "bạc liêu", "Bac Lieu", "bac lieu", "BacLieu", "baclieu"} },
        {53, new List<string>{"Bến Tre", "bến tre", "Ben Tre", "ben tre", "BenTre", "bentre"} },
        {54, new List<string>{"Cà Mau", "cà mau", "Ca Mau", "ca mau", "CaMau", "camau"} },
        {55, new List<string>{"Cần Thơ", "cần thơ", "Can Tho", "can tho", "CanTho", "cantho"} },
        {56, new List<string>{"Đồng Tháp", "đồng tháp", "Dong Thap", "dong thap", "DongThap", "dongthap"} },
        {57, new List<string>{"Hậu Giang", "hậu giang", "Hau Giang", "hau giang", "HauGiang", "haugiang"} },
        {58, new List<string>{"Kiên Giang", "kiên giang", "Kien Giang", "kien giang", "KienGiang", "kiengiang"} },
        {59, new List<string>{"Long An", "long an", "Long An", "long an", "LongAn", "longan"} },
        {60, new List<string>{"Sóc Trăng", "sóc trăng", "Soc Trang", "soc trang", "SocTrang", "soctrang"} },
        {61, new List<string>{"Tiền Giang", "tiền giang", "Tien Giang", "tien giang", "TienGiang", "tiengiang"} },
        {62, new List<string>{"Trà Vinh", "trà vinh", "Tra Vinh", "tra vinh", "TraVinh", "travinh"} },
        {63, new List<string>{"Vĩnh Long", "vĩnh long", "Vinh Long", "vinh long", "VinhLong", "vinhlong"} }
    };

    public static List<string> CityNames = new()
    {
        "TP Hồ Chí Minh", "Hà Nội", "Đà Nẵng", "Bình Dương", "Hưng Yên", "Vĩnh Phúc", "Hòa Bình","Sơn La","Điện Biên","Lai Châu","Lào Cai","Yên Bái","Phú Thọ","Hà Giang","Tuyên Quang","Cao Bằng","Bắc Kạn","Thái Nguyên","Lạng Sơn","Bắc Giang","Quảng Ninh","Hà Nội","Bắc Ninh","Hà Nam","Hải Dương","Hải Phòng","Hưng Yên","Nam Định","Thái Bình","Vĩnh Phúc","Ninh Bình","Thanh Hóa","Nghệ An","Hà Tĩnh","Quảng Bình","Quảng Trị","Thừa Thiên Huế","Đà Nẵng","Quảng Nam","Quảng Ngãi","Bình Định","Phú Yên","Khánh Hòa","Ninh Thuận","Bình Thuận","Kon Tum","Gia Lai","Đắk Lắk","Đắk Nông","Lâm Đồng","TP Hồ Chí Minh","Bà Rịa Vũng Tàu","Bình Dương","Bình Phước","Đồng Nai","Tây Ninh","An Giang","Bạc Liêu","Bến Tre","Cà Mau","Cần Thơ","Đồng Tháp","Hậu Giang","Kiên Giang","Long An","Sóc Trăng","Tiền Giang","Trà Vinh","Vĩnh Long"
    };
}