using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Repositories;
using System;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Helpers
{
    public class GiaoDichHelpers
    {
        /// <summary>
        /// Khách hàng thanh toán hóa đơn hoặc nộp vào tài khoản
        /// ngayThu không đúng > ngày hiện tại
        /// </summary>
        /// <requires>
        /// model != null /\ model.KhachHang != null /\ model.SoTienNopTheoThang != null /\ soTien > 0
        /// </requires>
        /// <effects>
        /// create new GiaoDich & insert
        /// update model
        ///     SoTienDaThu += soTien
        /// 
        /// duNo = SoTienPhaiNop - SoTienDaThu
        /// if duNo lt= 0
        ///     TrangThaiThu = true
        ///     NgayThu = ngayThu
        ///     if duNo lt 0
        ///         if DuCo == null
        ///             create new DuCo & insert
        ///         DuCo.SoTienDu -= duNo
        /// else
        ///     giaoDich.SoDu = duNo
        /// </effects>
        public static bool ThemGiaoDich(HoaDonModel model, int soTien, DateTime? ngayThu = null, HDNHDUnitOfWork uow = null)
        {
            if (ngayThu == null) ngayThu = DateTime.Now;
            if (uow == null) uow = new HDNHDUnitOfWork();
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();

            uow.BeginTransaction();
            try
            {
                // add new GiaoDich
                var giaoDich = new HDNHD.Models.DataContexts.GiaoDich()
                {
                    TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                    SoTien = soTien,
                    SoDu = 0,
                    NgayGiaoDich = ngayThu
                };
                giaoDichRepository.Insert(giaoDich);

                // update model
                model.SoTienNopTheoThang.SoTienDaThu += soTien;

                int duNo = (int)(model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);

                if (duNo <= 0)
                {
                    model.HoaDon.Trangthaithu = true;
                    model.HoaDon.NgayNopTien = ngayThu;
                    
                    if (duNo < 0)
                    {
                        if (model.DuCo == null)
                        {
                            model.DuCo = new HDNHD.Models.DataContexts.DuCo()
                            {
                                KhachhangID = model.KhachHang.KhachhangID,
                                TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                            };
                            duCoRepository.Insert(model.DuCo);
                        }
                        model.DuCo.SoTienDu = -duNo;

                        // trừ dư có cho hóa đơn tiếp theo (nếu có)
                        apDungDuCo(model, uow);
                    }
                } else
                {
                    giaoDich.SoDu = duNo;

                    // TODO: nếu hóa đơn đã thanh toán, giaoDich.SoDu = dư có sau giao dịch
                }
               
                uow.SubmitChanges();
                uow.Commit();
                return true;
            }
            catch (Exception)
            {
                uow.RollBack();
            }

            return false;
        }
        
        /// <summary>
        /// áp dụng dư có tại model cho những hóa đơn tháng sau (nếu có)
        /// </summary>
        public static void apDungDuCo(HoaDonModel model, HDNHDUnitOfWork uow = null)
        {
            if (uow == null) uow = new HDNHDUnitOfWork();
            var hoaDonRepository = uow.Repository<HoaDonRepository>();
            var duCoRepository = uow.Repository<DuCoRepository>();

            if (model.HoaDonTiepTheo != null && model.DuCo != null && model.HoaDonTiepTheo.Tongsotieuthu > 0) // đã nhập số liệu
            {
                var _model = hoaDonRepository.GetHoaDonModelByID(model.HoaDonTiepTheo.HoadonnuocID, null);

                // cap nhat trang thai du co
                model.DuCo.TrangThaiTruHet = true;
                model.DuCo.NgayTruHet = new DateTime(_model.HoaDon.NamHoaDon.Value, _model.HoaDon.ThangHoaDon.Value, 1);


                if (model.DuCo.SoTienDu <= _model.SoTienNopTheoThang.SoTienTrenHoaDon)
                {
                    _model.SoTienNopTheoThang.SoTienPhaiNop = _model.SoTienNopTheoThang.SoTienTrenHoaDon - model.DuCo.SoTienDu;

                    if (_model.DuCo != null)
                    {
                        duCoRepository.Delete(_model.DuCo);
                        thuHoiDuCo(_model, uow);
                    }
                }
                else
                {
                    _model.SoTienNopTheoThang.SoTienPhaiNop = 0;
                    
                    if (_model.DuCo == null)
                    {
                        _model.DuCo = new HDNHD.Models.DataContexts.DuCo()
                        {
                            KhachhangID = _model.KhachHang.KhachhangID,
                            TienNopTheoThangID = _model.SoTienNopTheoThang.ID,
                        };
                        duCoRepository.Insert(_model.DuCo);
                    }

                    _model.DuCo.SoTienDu = model.DuCo.SoTienDu - _model.SoTienNopTheoThang.SoTienTrenHoaDon;
                }

                if (_model.SoTienNopTheoThang.SoTienPhaiNop == 0)
                {
                    _model.HoaDon.Trangthaithu = true;
                    _model.HoaDon.NgayNopTien = new DateTime(_model.HoaDon.NamHoaDon.Value, _model.HoaDon.ThangHoaDon.Value, 1);
                }

                uow.SubmitChanges();

                // recursive
                apDungDuCo(_model, uow);
            }
        }

        /// <summary>
        /// Hủy giao dịch với id xác định
        /// </summary>
        /// <requires>
        /// lastGiaoDich != null /\ lastGiaoDich.GiaoDich.SoTien != null
        /// </requires>
        /// <effects>
        /// soTien = lastGiaoDich.SoTien
        /// delete GiaoDich
        /// 
        /// update model
        ///     SoTienDaThu -= soTien
        /// 
        /// duNo = SoTienPhaiNop - SoTienDaThu
        /// if duNo > 0
        ///     TrangThaiThu = false
        ///     if DuCo != null
        ///         delete DuCo
        /// else
        ///     if DuCo != null
        ///         DuCo.SoTienDu -= soTien
        ///         if DuCo.SoTienDu eq 0
        ///             delete DuCo
        /// </effects>
        public static bool HuyGiaoDich(GiaoDichModel model, HDNHDUnitOfWork uow = null)
        {
            if (uow == null) uow = new HDNHDUnitOfWork();

            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();
            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();

            uow.BeginTransaction();
            try
            {
                var soTien = model.GiaoDich.SoTien.Value;
                giaoDichRepository.Delete(model.GiaoDich);

                // update model
                model.SoTienNopTheoThang.SoTienDaThu -= soTien;
                
                int duNo = (int) (model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);

                if (duNo > 0)
                {
                    model.HoaDon.Trangthaithu = false;
                    model.HoaDon.NgayNopTien = null;
                }

                // trừ dư có
                if (model.DuCo != null)
                {
                    // thu hồi dư có nếu đã áp dụng cho hóa đơn tháng sau 
                    IHoaDonRepository hoaDonRepository = uow.Repository<HoaDonRepository>();
                    var hoaDonModel = hoaDonRepository.GetHoaDonModelByID(model.HoaDon.HoadonnuocID);

                    // cập nhật dư có
                    model.DuCo.SoTienDu -= soTien;

                    if (model.DuCo.SoTienDu <= 0)
                    {
                        duCoRepository.Delete(model.DuCo);
                        thuHoiDuCo(hoaDonModel, uow);
                    } else
                    {
                        apDungDuCo(hoaDonModel, uow);
                    }
                }

                uow.SubmitChanges();
                uow.Commit();
            }
            catch (Exception)
            {
                uow.RollBack();
                return false;
            }

            return true;
        }

        /// <summary>
        /// thu hồi dư có nếu đã áp dụng cho những hóa đơn tháng sau trước đó
        /// </summary>
        public static void thuHoiDuCo(HoaDonModel model, HDNHDUnitOfWork uow = null)
        {
            if (uow == null) uow = new HDNHDUnitOfWork();
            var hoaDonRepository = uow.Repository<HoaDonRepository>();
            var duCoRepository = uow.Repository<DuCoRepository>();

            if (model.HoaDonTiepTheo != null && model.HoaDonTiepTheo.Tongsotieuthu > 0)
            {
                var _model = hoaDonRepository.GetHoaDonModelByID(model.HoaDonTiepTheo.HoadonnuocID, null);

                _model.SoTienNopTheoThang.SoTienPhaiNop = _model.SoTienNopTheoThang.SoTienTrenHoaDon;
                _model.HoaDon.Trangthaithu = false;
                _model.HoaDon.NgayNopTien = null;
                
                if (_model.DuCo != null)
                {
                    duCoRepository.Delete(_model.DuCo);
                    uow.SubmitChanges();

                    // recursive
                    thuHoiDuCo(_model, uow);
                }
                uow.SubmitChanges();
            }
        }
    }
}