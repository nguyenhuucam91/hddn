using HoaDonNuocHaDong;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Helper
{
    public class KhachHang
    {

        static HoaDonHaDongEntities db = new HoaDonHaDongEntities();
        HoaDonHaDongEntities _db = new HoaDonHaDongEntities();

        public const int LOAIKHCANHAN = 1;
        public const int LOAIKHDOANHNGHIEP = 2;
        //loại áp giá
        public const int SINHHOAT = 1;
        public const int KINHDOANHDICHVU = 4;
        public const int SANXUAT = 5;
        public const int DONVISUNGHIEP = 3;
        public const int COQUANHANHCHINH = 2;
        public const int TONGHOP = 7;
        public const int DACBIET = 8;
        public const int SH1 = 9;
        public const int SH2 = 10;
        public const int SH3 = 11;
        public const int SH4 = 12;

        /// <summary>
        /// Hàm để lấy thông tin của khách hàng: số nhân khẩu
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <returns></returns>
        public int getSoNhanKhau(int KhachHangID)
        {
            var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KhachHangID);
            if (khachHang != null)
            {
                return khachHang.Sonhankhau.Value;
            }
            return 0;
        }



        /// <summary>
        /// Hàm để lấy thông tin số hộ khẩu
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <returns></returns>
        public int getSoHo(int KhachHangID)
        {
            var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KhachHangID);
            if (khachHang != null)
            {
                return khachHang.Soho.Value;
            }
            return 0;
        }

        /// <summary>
        /// Hàm để lấy thông tin loại khách hàng thuộc KHID
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <returns></returns>
        public static int getLoaiKH(int KhachHangID)
        {
            var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KhachHangID);
            if (khachHang != null)
            {
                return khachHang.LoaiKHID.Value;
            }
            return 0;
        }

        public static String getTenLoaiKH(int KhachHangID)
        {
            var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KhachHangID);
            if (khachHang != null)
            {
                var loaiKH = db.LoaiKHs.Find(khachHang.LoaiKHID);
                if (loaiKH != null)
                {
                    return loaiKH.Ten;
                }
            }
            return "";
        }

        /// <summary>
        /// Lấy hình thức thanh toán
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <returns></returns>
        public static String getHTTTID(int KhachHangID)
        {
            var khachHang = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KhachHangID);

            if (khachHang != null)
            {
                return db.Hinhthucthanhtoans.FirstOrDefault(p => p.HinhthucttID == khachHang.HinhthucttID).Ten;
            }

            return "";
        }

        /// <summary>
        /// Lấy tuyến khách hàng dựa theo tuyến KHID
        /// </summary>
        /// <param name="tuyenKHID"></param>
        /// <returns></returns>
        public static String getTuyenKH(int? tuyenKHID)
        {
            if (tuyenKHID != null)
            {
                var tuyenID = db.Tuyenkhachhangs.FirstOrDefault(p => p.TuyenKHID == tuyenKHID);
                if (tuyenID != null)
                {
                    return tuyenID.Ten;
                }
                return "";
            }
            return "";
        }

        /// <summary>
        ///  Lấy quận của khách hàng dựa theo quận ID
        /// </summary>
        /// <param name="quanID"></param>
        /// <returns></returns>
        public String getQuan(int? quanID)
        {
            if (quanID != null)
            {
                var quan = _db.Quanhuyens.FirstOrDefault(p => p.QuanhuyenID == quanID);
                if (quan != null)
                {
                    return quan.Ten;
                }
            }
            return "";
        }

        /// <summary>
        ///  Lấy quận của khách hàng dựa theo quận ID
        /// </summary>
        /// <param name="quanID"></param>
        /// <returns></returns>
        public String getPhuong(int? phuongXaID)
        {
            if (phuongXaID != null)
            {
                var pX = _db.Phuongxas.FirstOrDefault(p => p.PhuongxaID == phuongXaID);
                if (pX != null)
                {
                    return pX.Ten;
                }
            }
            return "";
        }

        public Khachhang getKH(int KHID)
        {
            HoaDonHaDongEntities db = new HoaDonHaDongEntities();
            return db.Khachhangs.Find(KHID);
        }
        /// <summary>
        ///  Lấy cụm dân cư của khách hàng
        /// </summary>
        /// <param name="cumDanCuID"></param>
        /// <returns></returns>
        public String getCumDanCu(int? cumDanCuID)
        {
            if (cumDanCuID != null)
            {
                var cumDanCu = _db.Cumdancus.FirstOrDefault(p => p.CumdancuID == cumDanCuID);
                if (cumDanCu != null)
                {
                    return cumDanCu.Ten;
                }
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="KHID"></param>
        /// <returns></returns>

        public static String getLoaiApGia(int KHID)
        {
            var loaiApGiaID = db.Khachhangs.Where(p => p.KhachhangID == KHID).FirstOrDefault();
            //nếu không có loại áp giá cho khách hàng
            if (loaiApGiaID != null)
            {
                return db.Loaiapgias.Where(p => p.LoaiapgiaID == loaiApGiaID.LoaiapgiaID).FirstOrDefault().Ten;
            }
            return "";
        }

        /// <summary>
        /// Làm tròn số khi chia %, cách thức đều là làm tròn xuống
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double roundNumber(double number)
        {
            return (int)number;
        }

        public static int getTTDoc(int KHID)
        {
            Khachhang kH = db.Khachhangs.FirstOrDefault(p => p.KhachhangID == KHID);
            if (kH != null)
            {
                return kH.TTDoc.Value;
            }
            return 0;
        }

        /// <summary>
        /// Lưu chỉ số tổng hợp cho khách hàng
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <param name="isPhanTram"></param>
        /// <param name="SH"></param>
        /// <param name="KD"></param>
        /// <param name="SX"></param>
        public void saveGiaTongHop(int KhachHangID, byte isPhanTram, double SH, double KD, double HC, double CC, double SX, int month, short year)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();            
            _db.Apgiatonghops.RemoveRange(_db.Apgiatonghops.Where(p => p.KhachhangID == KhachHangID).ToList());
            if (KD != -1)
            {
                Apgiatonghop apTongHopKD = new Apgiatonghop();
                apTongHopKD.KhachhangID = KhachHangID;
                apTongHopKD.IsDelete = false;
                apTongHopKD.CachTinh = isPhanTram;
                apTongHopKD.IDLoaiApGia = KhachHang.KINHDOANHDICHVU; //KD
                apTongHopKD.SanLuong = KD;
                apTongHopKD.NamTongHop = year;
                apTongHopKD.ThangTongHop = month;
                _db.Apgiatonghops.Add(apTongHopKD);
                _db.SaveChanges();
            }
            if (HC != -1)
            {
                Apgiatonghop apTongHopHC = new Apgiatonghop();
                apTongHopHC.KhachhangID = KhachHangID;
                apTongHopHC.IsDelete = false;
                apTongHopHC.CachTinh = isPhanTram;
                apTongHopHC.IDLoaiApGia = KhachHang.COQUANHANHCHINH; //HC
                apTongHopHC.SanLuong = HC;
                apTongHopHC.NamTongHop = year;
                apTongHopHC.ThangTongHop = month;
                _db.Apgiatonghops.Add(apTongHopHC);
                _db.SaveChanges();
            }
            if (CC != -1)
            {
                Apgiatonghop apTongHopCC = new Apgiatonghop();
                apTongHopCC.KhachhangID = KhachHangID;
                apTongHopCC.IsDelete = false;
                apTongHopCC.CachTinh = isPhanTram;
                apTongHopCC.IDLoaiApGia = KhachHang.DONVISUNGHIEP; //CC
                apTongHopCC.SanLuong = CC;
                apTongHopCC.NamTongHop = year;
                apTongHopCC.ThangTongHop = month;
                _db.Apgiatonghops.Add(apTongHopCC);
                _db.SaveChanges();
            }
            if (SX != -1)
            {
                Apgiatonghop apTongHopSX = new Apgiatonghop();
                apTongHopSX.KhachhangID = KhachHangID;
                apTongHopSX.IsDelete = false;
                apTongHopSX.CachTinh = isPhanTram;
                apTongHopSX.IDLoaiApGia = KhachHang.SANXUAT; //SX
                apTongHopSX.SanLuong = SX;
                apTongHopSX.NamTongHop = year;
                apTongHopSX.ThangTongHop = month;
                _db.Apgiatonghops.Add(apTongHopSX);
                _db.SaveChanges();
            }
            if (SH != -1)
            {
                Apgiatonghop apTongHop = new Apgiatonghop();
                apTongHop.KhachhangID = KhachHangID;
                apTongHop.IsDelete = false;
                apTongHop.CachTinh = isPhanTram;
                apTongHop.IDLoaiApGia = KhachHang.SINHHOAT; //SH
                apTongHop.SanLuong = SH;
                apTongHop.NamTongHop = year;
                apTongHop.ThangTongHop = month;
                _db.Apgiatonghops.Add(apTongHop);
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Lưu thông tin giá đặc biệt
        /// </summary>
        /// <param name="KhachHangID"></param>
        /// <param name="isPhanTram"></param>
        /// <param name="SH"></param>
        /// <param name="KD"></param>
        /// <param name="HC"></param>
        /// <param name="CC"></param>
        /// <param name="SX"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        public void saveGiaDacBiet(int KhachHangID, double SH, double KD, double HC, double CC, double SX, double SH1, double SH2, double SH3, double SH4, int month, int year)
        {
            HoaDonHaDongEntities db = new HoaDonHaDongEntities();
            //xóa record ứng với áp giá tổng hợp của khách hàng đó
            db.Apgiatonghops.RemoveRange(db.Apgiatonghops.Where(p => p.KhachhangID == KhachHangID && p.ThangTongHop == month && p.NamTongHop == year));
            if (KD != -1)
            {
                Apgiatonghop apTongHopKD = new Apgiatonghop();
                apTongHopKD.KhachhangID = KhachHangID;
                apTongHopKD.IsDelete = false;
                apTongHopKD.IDLoaiApGia = KhachHang.KINHDOANHDICHVU; //KD
                apTongHopKD.SanLuong = KD;
                apTongHopKD.NamTongHop = year;
                apTongHopKD.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHopKD);
            }
            if (HC != -1)
            {
                Apgiatonghop apTongHopHC = new Apgiatonghop();
                apTongHopHC.KhachhangID = KhachHangID;
                apTongHopHC.IsDelete = false;
                apTongHopHC.IDLoaiApGia = KhachHang.COQUANHANHCHINH; //HC
                apTongHopHC.SanLuong = HC;
                apTongHopHC.NamTongHop = year;
                apTongHopHC.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHopHC);
            }
            if (CC != -1)
            {
                Apgiatonghop apTongHopCC = new Apgiatonghop();
                apTongHopCC.KhachhangID = KhachHangID;
                apTongHopCC.IsDelete = false;
                apTongHopCC.IDLoaiApGia = KhachHang.DONVISUNGHIEP; //CC
                apTongHopCC.SanLuong = CC;
                apTongHopCC.NamTongHop = year;
                apTongHopCC.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHopCC);
            }
            if (SX != -1)
            {
                Apgiatonghop apTongHopSX = new Apgiatonghop();
                apTongHopSX.KhachhangID = KhachHangID;
                apTongHopSX.IsDelete = false;
                apTongHopSX.IDLoaiApGia = KhachHang.SANXUAT; //SX
                apTongHopSX.SanLuong = SX;
                apTongHopSX.NamTongHop = year;
                apTongHopSX.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHopSX);
            }
            if (SH != -1)
            {
                Apgiatonghop apTongHop = new Apgiatonghop();
                apTongHop.KhachhangID = KhachHangID;
                apTongHop.IsDelete = false;
                apTongHop.IDLoaiApGia = KhachHang.SINHHOAT; //SH
                apTongHop.SanLuong = SH;
                apTongHop.NamTongHop = year;
                apTongHop.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHop);
            }

            if (SH1 != -1)
            {
                Apgiatonghop apTongHop = new Apgiatonghop();
                apTongHop.KhachhangID = KhachHangID;
                apTongHop.IsDelete = false;
                apTongHop.IDLoaiApGia = KhachHang.SH1; //SH
                apTongHop.SanLuong = SH1;
                apTongHop.NamTongHop = year;
                apTongHop.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHop);
            }

            if (SH2 != -1)
            {
                Apgiatonghop apTongHop = new Apgiatonghop();
                apTongHop.KhachhangID = KhachHangID;
                apTongHop.IsDelete = false;
                apTongHop.IDLoaiApGia = KhachHang.SH2; //SH
                apTongHop.SanLuong = SH2;
                apTongHop.NamTongHop = year;
                apTongHop.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHop);
            }

            if (SH3 != -1)
            {
                Apgiatonghop apTongHop = new Apgiatonghop();
                apTongHop.KhachhangID = KhachHangID;
                apTongHop.IsDelete = false;
                apTongHop.IDLoaiApGia = KhachHang.SH3; //SH
                apTongHop.SanLuong = SH3;
                apTongHop.NamTongHop = year;
                apTongHop.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHop);
            }

            if (SH4 != -1)
            {
                Apgiatonghop apTongHop = new Apgiatonghop();
                apTongHop.KhachhangID = KhachHangID;
                apTongHop.IsDelete = false;
                apTongHop.IDLoaiApGia = KhachHang.SH4; //SH
                apTongHop.SanLuong = SH4;
                apTongHop.NamTongHop = year;
                apTongHop.ThangTongHop = month;
                db.Apgiatonghops.Add(apTongHop);
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Đẩy danh sách khách hàng xuống
        /// </summary>
        /// <param name="TTDoc"></param>
        /// <param name="tuyen"></param>
        public string pushKhachHangXuong(int TTDoc, int tuyen, long createdTime)
        {
            HoaDonHaDongEntities _db = new HoaDonHaDongEntities();
            var khachHang = _db.Khachhangs.Where(p => p.TTDoc >= TTDoc && p.TuyenKHID == tuyen && p.IsDelete == false).ToList();
            foreach (var item in khachHang)
            {
                if (item.TTDoc >= TTDoc)
                {
                    if (createdTime > item.UpdatedTime || item.UpdatedTime == null)
                    {
                        Khachhang kH = _db.Khachhangs.Find(item.KhachhangID);
                        if (kH.Ngaykyhopdong == null)
                        {
                            kH.Ngaykyhopdong = new DateTime(1970, 1, 1);
                        }
                        int thuTuDoc = kH.TTDoc.Value;
                        kH.TTDoc = thuTuDoc + 1;
                        _db.Entry(item).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Đẩy danh sách khách hàng lên
        /// </summary>
        /// <param name="TTDocSau"></param>
        /// <param name="TTDocTruoc"></param>
        /// <param name="tuyen"></param>
        public static void pullKhachHangLen(int TTDocSau, int TTDocTruoc, int tuyen)
        {
            var khachHang = db.Khachhangs.Where(p => p.TTDoc <= TTDocSau && p.TTDoc >= TTDocTruoc && p.TuyenKHID == tuyen && p.IsDelete == false).ToList();
            foreach (var item in khachHang)
            {
                Khachhang kH = db.Khachhangs.Find(item.KhachhangID);
                int thuTuDoc = kH.TTDoc.Value;
                kH.TTDoc = thuTuDoc - 1;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void pushKhachHang(int thuTuDocCu, int thuTuDocMoi, int tuyen)
        {
            var khachHang = db.Khachhangs.Where(p => p.TTDoc >= thuTuDocMoi && p.TTDoc < thuTuDocCu && p.TuyenKHID == tuyen && p.IsDelete == false).ToList();
            foreach (var item in khachHang)
            {
                Khachhang kH = db.Khachhangs.Find(item.KhachhangID);
                int thuTuDoc = kH.TTDoc.Value;
                kH.TTDoc = thuTuDoc + 1;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Đẩy danh sách khách hàng xuống khi Edit.
        /// </summary>
        /// <param name="TTDoc"></param>
        /// <param name="tuyen"></param>
        public static void pushKhachHangXuongKhiEdit(int TTDoc, int tuyen)
        {

            var khachHang = db.Khachhangs.Where(p => p.TTDoc >= TTDoc && p.TuyenKHID == tuyen && p.IsDelete == false).Distinct().ToList();
            foreach (var item in khachHang)
            {
                Khachhang kH = db.Khachhangs.Find(item.KhachhangID);
                int thuTuDoc = kH.TTDoc.Value;
                kH.TTDoc = thuTuDoc + 1;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void changeTTDocPosition(int thuTuDocCu, int thuTuDocMoi, int tuyenIDCu, int tuyenIDMoi)
        {
            if (tuyenIDCu == tuyenIDMoi)
            {
                if (thuTuDocCu != thuTuDocMoi)
                {
                    if (thuTuDocCu < thuTuDocMoi)
                    {
                        pullKhachHangLen(thuTuDocMoi, thuTuDocCu, tuyenIDMoi);
                    }
                    else
                    {
                        pushKhachHang(thuTuDocCu, thuTuDocMoi, tuyenIDMoi);
                    }
                }                
            }
            else
            {
                pushKhachHangXuongKhiEdit(thuTuDocMoi, tuyenIDMoi);
            }            
        }

    }
}