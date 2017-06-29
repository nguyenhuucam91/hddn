using HoaDonHaDong.Helper;
using HoaDonNuocHaDong.Base;
using HoaDonNuocHaDong.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoaDonNuocHaDong.Controllers
{
    public class LogController : BaseController
    {
        HoaDonHaDongEntities db = new HoaDonHaDongEntities();


        //
        // GET: /Log/

        public ActionResult Index()
        {
            saveControllerList();

            DateTime aWeekAgoFromCurrent = DateTime.Now.AddDays(-7);
            DateTime currentDate = DateTime.Now;
            List<Lichsusudungct> ls = db.Lichsusudungcts.OrderByDescending(p => p.Thoigian)
                .Where(p=>p.Thoigian <= currentDate && p.Thoigian >= aWeekAgoFromCurrent).ToList();

            #region ViewBag
            ViewBag.nhomChucNang = db.Nhomchucnangs.ToList();
            ViewBag.lichSu = ls;
            ViewBag.startTime = "";
            ViewBag.endTime = "";
            #endregion
            return View();
        }

        /// <summary>
        /// Hàm xóa toàn bộ lịch sử hệ thống
        /// </summary>
        /// <returns></returns>
        public ActionResult clearAllLog()
        {
            db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Lichsusudungct]");
            return RedirectToAction("Index");
        }

        public ActionResult Listmodule()
        {
            //lưu danh sách controller vào nhóm
            List<Nhomchucnang> ls = db.Nhomchucnangs.ToList();
            ViewBag.logList = ls;
            return View();
        }

        /// <summary>
        /// Lưu tên controller vào db
        /// </summary>
        public void saveControllerName(int RowID, String RowValue)
        {
            Nhomchucnang _nhom = db.Nhomchucnangs.Find(RowID);
            if (_nhom != null)
            {
                _nhom.Ten = RowValue;
                db.Entry(_nhom).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RowID"></param>
        /// <param name="RowValue"></param>
        public void saveActionName(int RowID, String RowValue)
        {
            Chucnangchuongtrinh _chucNang = db.Chucnangchuongtrinhs.Find(RowID);
            if (_chucNang != null)
            {
                _chucNang.Ten = RowValue;
                db.Entry(_chucNang).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public ActionResult Listfunctionmodule(int id, String controllerName)
        {
            insertChucNangChuongTrinhIfNotExist(id, controllerName);
            ViewBag.actionList = db.Chucnangchuongtrinhs.Where(p => p.NhomchucnangID == id).ToList();
            return View();
        }

        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        /// <summary>
        /// Lưu danh sách controller vào db
        /// </summary>
        private void saveControllerList()
        {
            List<String> danhSachController = GetControllerNames();
            foreach (var item in danhSachController)
            {
                Nhomchucnang _controller = db.Nhomchucnangs.FirstOrDefault(p => p.TenController == item);
                if (_controller != null)
                {
                    int nhomChucNangID = _controller.NhomchucnangID;
                    if (_controller == null)
                    {
                        Nhomchucnang _chucNang = new Nhomchucnang();
                        _chucNang.TenController = item;
                        _chucNang.Ten = "";
                        db.Nhomchucnangs.Add(_chucNang);
                        db.SaveChanges();
                        nhomChucNangID = _chucNang.NhomchucnangID;
                    }
                    insertChucNangChuongTrinhIfNotExist(nhomChucNangID, item);
                }
            }
        }


        private void insertChucNangChuongTrinhIfNotExist(int id, String controllerName)
        {
            List<String> actionName = getActionNames(controllerName);
            foreach (var item in actionName)
            {
                Chucnangchuongtrinh _cN = db.Chucnangchuongtrinhs.FirstOrDefault(p => p.NhomchucnangID == id && p.TenAction == item);
                if (_cN == null)
                {
                    //nếu chưa có danh sách action trong controller thì thêm vào CSDL
                    Chucnangchuongtrinh cN = new Chucnangchuongtrinh();
                    cN.NhomchucnangID = id;
                    cN.TenAction = item;
                    cN.Ten = "";
                    db.Chucnangchuongtrinhs.Add(cN);
                    db.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Hàm để lấy danh sách controller trong dự án
        /// </summary>
        /// <returns>Danh sách controllers</returns>
        public List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }

        /// <summary>
        /// Hàm để lấy danh sách action thuộc Controller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public List<String> getActionNames(String controllerName)
        {
            var types =
        from a in AppDomain.CurrentDomain.GetAssemblies()
        from t in a.GetTypes()
        where typeof(IController).IsAssignableFrom(t) &&
                string.Equals(controllerName, t.Name, StringComparison.OrdinalIgnoreCase)
        select t;

            var controllerType = types.FirstOrDefault();

            if (controllerType == null)
            {
                return Enumerable.Empty<string>().ToList();
            }
            return new ReflectedControllerDescriptor(controllerType)
                .GetCanonicalActions().Select(x => x.ActionName)
                .ToList();
        }

        /// <summary>
        /// Lọc log dựa trên tiêu chí cho trước
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FilterLog(FormCollection form)
        {
           
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            String startTime = form["startTime"];
            String endTime = form["endTime"];
            IEnumerable<Lichsusudungct> lichsusudungcts = db.Lichsusudungcts;

            //Nếu cả startTime và endTime empty thì ko thông qua lọc
            if (!String.IsNullOrEmpty(startTime))
            {
                startDate = DateTime.Parse(form["startTime"].ToString());
                DateTime dTStart = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 1);
                startDate = dTStart;
                lichsusudungcts = lichsusudungcts.Where(p => p.Thoigian >= startDate);
            }           


            if (!String.IsNullOrEmpty(endTime))
            {
                endDate = DateTime.Parse(form["endTime"].ToString());
                DateTime dTEnd = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                endDate = dTEnd;
                lichsusudungcts = lichsusudungcts.Where(p => p.Thoigian <= endDate);
            }

            String username = form["username"].ToString();
            String keyword = form["keyword"].ToString();
            int module = form["category"].ToString() == null ? 0 : Convert.ToInt32(form["category"]);
            //lọc log dựa theo start và end
            if (!String.IsNullOrEmpty(username))
            {
                var lichSuSuDung = (from i in lichsusudungcts
                                    join r in db.Nguoidungs on i.NguoidungID equals r.NguoidungID
                                    where r.Taikhoan.Contains(username)
                                    select new
                                    {
                                        LichSuSD = i,
                                    });
                int count = lichSuSuDung.Count();
                lichsusudungcts = lichSuSuDung.Select(p => p.LichSuSD);
                
            }

            if (!String.IsNullOrEmpty(keyword))
            {
                var lichSuSuDung = (from i in lichsusudungcts
                                    join r in db.Chucnangchuongtrinhs on i.ChucnangID equals r.ChucnangID
                                    where r.Ten.Contains(keyword)
                                    select new
                                    {
                                        LichSuSD = i,
                                    });
                lichsusudungcts = lichSuSuDung.Select(p => p.LichSuSD);
            }

            if (module > 0)
            {
                var lichSuSuDung = (from i in lichsusudungcts
                                    join s in db.Chucnangchuongtrinhs on i.ChucnangID equals s.ChucnangID
                                    join r in db.Nhomchucnangs on s.NhomchucnangID equals r.NhomchucnangID
                                    where r.NhomchucnangID == Convert.ToInt32(module)
                                    select new
                                    {
                                        LichSuSD = i,
                                    });
                lichsusudungcts = lichSuSuDung.Select(p => p.LichSuSD);
            }

            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.ngDung = username;
            ViewBag.tuKhoa = keyword;
            ViewBag.nhomChucNang = db.Nhomchucnangs.ToList();
            ViewBag.lichSu = lichsusudungcts;

            return View("Index");
        }
    }
}