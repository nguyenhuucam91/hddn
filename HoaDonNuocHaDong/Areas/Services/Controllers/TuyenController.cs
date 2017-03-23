using HDNHD.Core.Models;
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
    public class TuyenController : BaseController
    {
        private ITuyenKHRepository tuyenKHRepository;

        public TuyenController()
        {
            tuyenKHRepository = uow.Repository<TuyenKHRepository>();
        }

        public AjaxResult GetByNhanVienID(int nhanVienID)
        {
            var models = tuyenKHRepository.GetByNhanVienID(nhanVienID).Select(m => new { 
                TuyenKHID = m.TuyenKHID,
                MaTuyen = m.Matuyen,
                Ten = m.Ten
            });
            return new AjaxResult()
            {
                Data = models.ToList()
            };
        }
	}
}