using HDNHD.Core.Models;
using HDNHD.Models.DataContexts;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Repositories;
using HoaDonNuocHaDong.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoaDonNuocHaDong.Areas.Services.Controllers
{
    public class QuanHuyenController : BaseController
    {
        private IQuanHuyenRepository quanHuyenRepository;

        public QuanHuyenController()
        {
            quanHuyenRepository = uow.Repository<QuanHuyenRepository>();
        }

        /// <summary>
        /// returns list of all existing QuanHuyen 
        ///     customized by <tt>this.nhanVien</tt> if exist
        /// </summary>
        public AjaxResult GetAll()
        {
            var models = quanHuyenRepository.GetAll(m => m.IsDelete == false);
            var context = (HDNHDDataContext) uow.GetDataContext();

            if (nhanVien != null && nhanVien.ToQuanHuyenID.HasValue)
            {
                models = from model in models
                         join to in context.ToQuanHuyens on model.QuanhuyenID equals to.QuanHuyenID
                         where to.ToQuanHuyenID == nhanVien.ToQuanHuyenID.Value
                         select model;
            }

            return new AjaxResult() {
                Data = models.ToList()
            };
        }
	}
}