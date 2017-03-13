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
    public class ToController : BaseController
    {
        private IToRepository toRepository;

        public ToController()
        {
            toRepository = uow.Repository<ToRepository>();
        }

        public AjaxResult GetByQuanHuyenID(int quanHuyenID) {
            var tos = toRepository.GetByQuanHuyenID(quanHuyenID);

            return new AjaxResult() { 
                Data = tos.ToList()
            };
        }
	}
}