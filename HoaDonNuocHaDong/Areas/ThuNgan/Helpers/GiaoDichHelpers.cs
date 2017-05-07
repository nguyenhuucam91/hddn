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
                    }
                } else
                {
                    giaoDich.SoDu = duNo;
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

                // trừ dư có
                int duNo = (int) (model.SoTienNopTheoThang.SoTienPhaiNop - model.SoTienNopTheoThang.SoTienDaThu);

                if (duNo > 0)
                {
                    model.HoaDon.Trangthaithu = false;
                    if (model.DuCo != null)
                        duCoRepository.Delete(model.DuCo);
                } else
                {
                    if (model.DuCo != null)
                    {
                        model.DuCo.SoTienDu -= soTien;

                        if (model.DuCo.SoTienDu == 0)
                            duCoRepository.Delete(model.DuCo);
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
    }
}