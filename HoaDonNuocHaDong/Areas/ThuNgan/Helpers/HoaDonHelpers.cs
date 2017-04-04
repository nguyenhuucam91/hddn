using HoaDonNuocHaDong.Areas.ThuNgan.Models;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories;
using HoaDonNuocHaDong.Areas.ThuNgan.Repositories.Interfaces;
using HoaDonNuocHaDong.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoaDonNuocHaDong.Areas.ThuNgan.Helpers
{
    public class HoaDonHelpers
    {
        /// <summary>
        ///     entry for the two other methods, namely ThanhToan() & HuyThanhToan()
        ///     returns if success
        /// </summary>
        /// <requires>
        ///     model neq null
        /// </requires>
        public static bool CapNhatThanhToan(HoaDonModel model, bool status, HDNHDUnitOfWork uow = null)
        {
            uow = uow ?? new HDNHDUnitOfWork();

            if (status)
                return ThanhToan(model, uow);
            else
                return HuyThanhToan(model, uow);
        }

        /// <summary>
        ///     set un-paid HoaDon as paid
        /// </summary>
        /// <requires>
        ///     model neq null /\ uow neq null
        /// </requires>
        /// <effects>
        ///     if HoaDon.TrangThaiThu is paid
        ///         do nothing
        ///     else
        ///         create new GiaoDich <tt>giaoDich</tt> (SoTien = SoTienNopTheoThang.SoTienPhaiNop)
        ///         
        ///         if DuCo neq null /\ DuCo.SoTienDu gt SoTienNopTheoThang.SoTienPhaiNop
        ///             update giaoDich (SoDu=-SoTienNopTheoThang.SoTienPhaiNop)
        ///             update DuCo.SoTienDu += giaoDich.SoDu
        ///             if DuCo.SoTienDu eq 0
        ///                 delete DuCo
        ///             
        ///         update HoaDon (TrangThaiThu, NgayNopTien)
        ///         update SoTienNopTheoThang.SoTienDaThu = SoTienNopTheoThang.SoTienPhaiNop
        ///         save all
        /// </effects>
        public static bool ThanhToan(HoaDonModel model, HDNHDUnitOfWork uow)
        {
            if (model.HoaDon.Trangthaithu == true) return true;

            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();

            uow.BeginTransaction();
            try
            {
                // create GiaoDich
                HDNHD.Models.DataContexts.GiaoDich giaoDich = new HDNHD.Models.DataContexts.GiaoDich()
                {
                    TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                    NgayGiaoDich = DateTime.Now,
                    SoTien = (int?)model.SoTienNopTheoThang.SoTienPhaiNop,
                    SoDu = 0
                };
                giaoDichRepository.Insert(giaoDich);

                if (model.DuCo != null && model.DuCo.SoTienDu >= model.SoTienNopTheoThang.SoTienPhaiNop)
                {
                    giaoDich.SoDu = -(int?)model.SoTienNopTheoThang.SoTienPhaiNop;
                    model.DuCo.SoTienDu += giaoDich.SoDu;

                    // delete DuCo if SoTienDu = 0
                    if (model.DuCo.SoTienDu == 0)
                    {
                        duCoRepository.Delete(model.DuCo);
                    }
                }

                // update HoaDon
                model.HoaDon.Trangthaithu = true;
                model.HoaDon.NgayNopTien = model.HoaDon.NgayNopTien ?? DateTime.Now;
                // update SoTienNopTheoThang
                model.SoTienNopTheoThang.SoTienDaThu = (int?)model.SoTienNopTheoThang.SoTienPhaiNop;

                uow.SubmitChanges();
                uow.Commit();
            }
            catch (Exception e)
            {
                uow.RollBack();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     set paid HoaDon as un-paid 
        /// </summary>
        /// <requires>
        ///     model neq null /\ uow neq null
        /// </requires>
        /// <effects>
        ///     if HoaDon is NOT paid
        ///         do nothing
        ///     else 
        ///         update HoaDon (TrangThaiThu, NgayNopTien)
        ///         // roll back all GiaoDich for this HoaDon
        ///         if DuCo eq null
        ///             create new DuCo duCo
        ///         foreach GiaoDich <tt>giaoDich</tt> associated with SoTienNopTheoThang of this HoaDon
        ///             SoTienNopTheoThang.SoTienDaThu -= giaoDich.SoTien
        ///             duCo.SoTienDu -= giaoDich.SoDu
        ///         if DuCo.SoTienDu > 0
        ///             insert to db
        /// </effects>
        public static bool HuyThanhToan(HoaDonModel model, HDNHDUnitOfWork uow)
        {
            if (model.HoaDon.Trangthaithu == false || model.HoaDon.Trangthaithu == null) return true;

            IDuCoRepository duCoRepository = uow.Repository<DuCoRepository>();
            IGiaoDichRepository giaoDichRepository = uow.Repository<GiaoDichRepository>();

            uow.BeginTransaction();
            try
            {
                // update HoaDon
                model.HoaDon.Trangthaithu = false;
                model.HoaDon.NgayNopTien = null;

                model.DuCo = model.DuCo ?? new HDNHD.Models.DataContexts.DuCo()
                {
                    TienNopTheoThangID = model.SoTienNopTheoThang.ID,
                    KhachhangID = model.KhachHang.KhachhangID,
                    SoTienDu = 0
                };

                var giaoDichs = giaoDichRepository.GetAll(m => m.TienNopTheoThangID == model.SoTienNopTheoThang.ID);

                // roll back giaoDichs
                foreach (HDNHD.Models.DataContexts.GiaoDich giaoDich in giaoDichs)
                {
                    model.SoTienNopTheoThang.SoTienDaThu -= giaoDich.SoTien;
                    model.DuCo.SoTienDu -= giaoDich.SoDu;
                    giaoDichRepository.Delete(giaoDich);
                }

                if (model.DuCo.SoTienDu > 0)
                    duCoRepository.Insert(model.DuCo);

                // save
                uow.SubmitChanges();
                uow.Commit();
            }
            catch (Exception e)
            {
                uow.RollBack();

                return false;
            }

            return true;
        }
    }
}