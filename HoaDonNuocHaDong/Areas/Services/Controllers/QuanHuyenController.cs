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
    public class QuanHuyenController : BaseController
    {
        private IQuanHuyenRepository quanHuyenRepository;

        public QuanHuyenController()
        {
            quanHuyenRepository = uow.Repository<QuanHuyenRepository>();
        }

        public AjaxResult GetAll()
        {
            var models = quanHuyenRepository.GetAll(m => m.IsDelete == false);

            return new AjaxResult() {
                Data = models.ToList()
            };
        }
	}
}