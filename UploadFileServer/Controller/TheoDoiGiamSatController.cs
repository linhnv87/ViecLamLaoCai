using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UploadFileServer.Controller
{
    public class TheoDoiGiamSatController : ApiController
    {
        vn.gov.yenbai.tdgs.WebserviceGiamSat _GiamSat = new vn.gov.yenbai.tdgs.WebserviceGiamSat();
        public object GetStatus(string username)
        {
            try
            {
                string a = _GiamSat.CountAll(username);
                return Json(a);
            }
            catch (Exception ex)
            {
                return Json(new JMessage { Title = "Lấy dữ liệu không thành công", Object = ex.Message, Error = true });
            }
        }
        [HttpPost]
        public object AddDocument(VanbanWebserviceInfo data)
        {
            try
            {
                var obj = JsonConvert.SerializeObject(data);
                string a = _GiamSat.AddDocument(obj);
                return Json(new JMessage { Title = "Thêm nhiệm vụ thành công", Error = false, idvanban = a });
            }
            catch (Exception ex)
            {
                return Json(new JMessage { Title = "Lấy dữ liệu không thành công", Object = ex.Message, Error = true });
            }
        }
        public class VanbanWebserviceInfo
        {
            public string Sokyhieu { get; set; }
            public string Trichyeu { get; set; }
            public DateTime? Ngayvanban { get; set; }

            public string Nguoiky { get; set; }
            public string Nguoitao { get; set; }
            public List<FileWebserviceInfo> FileAttachment { get; set; }
        }
        public class FileWebserviceInfo
        {
            public string Duongdan { get; set; }
            public string Tenfile { get; set; }
            public byte[] Bytefile { get; set; }
            public string LoaiFile { get; set; }
            public double? Dungluong { get; set; }
        }

    }
}
