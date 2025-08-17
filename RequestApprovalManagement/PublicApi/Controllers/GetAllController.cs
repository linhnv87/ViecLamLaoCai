using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PublicApi.Controllers
{
    [Route("api/get-all")]
    [ApiController]
    public class GetAllController : ControllerBase
    {
        private static readonly List<Document> Documents = new()
        {
            new Document
            {
                Id = 1,
                TieuDe = "Xây dựng cầu thủy lợi ABC",
                NoiDungChinh = "Xây dựng cầu thủy lợi ABC cho xã AAA, huyện BBB.....",
                LoaiVanBan = "cong-van",
                LinhVuc = "linh-vuc-khac",
                HanPheDuyet = "12/21/2024",
                DinhKemVanBanTrinh = new List<string>
                {
                    "https://btv-api.blueskytech.vn/Files/Document_Attachments/test 18.11_20241125090402619AM.doc"
                },
                DinhKemVanBanLienQuan = new List<string>(),
                Status = "du-thao"
            },
            new Document
            {
                Id = 2,
                TieuDe = "Xây dựng nhà tình thương",
                NoiDungChinh = "Xây dựng nhà tình thương...",
                LoaiVanBan = "cong-van",
                LinhVuc = "dat-dai",
                HanPheDuyet = "12/22/2024",
                DinhKemVanBanTrinh = new List<string>
                {
                    "https://btv-api.blueskytech.vn/Files/Document_Attachments/test 18.11_20241125090402619AM.doc"
                },
                DinhKemVanBanLienQuan = new List<string>(),
                Status = "xin-y-kien"
            },
            new Document
            {
                Id = 3,
                TieuDe = "Xây dựng cầu thủy lợi DEF",
                NoiDungChinh = "Xây dựng cầu thủy lợi DEF...",
                LoaiVanBan = "cong-van",
                LinhVuc = "linh-vuc-khac",
                HanPheDuyet = "12/23/2024",
                DinhKemVanBanTrinh = new List<string>
                {
                    "https://btv-api.blueskytech.vn/Files/Document_Attachments/test 18.11_20241125090402619AM.doc"
                },
                DinhKemVanBanLienQuan = new List<string>(),
                Status = "phe-duyet"
            }
        };
        [HttpGet]
        public IActionResult GetDocuments()
        {
            return Ok(new BaseResponseModel(Documents));
        }
    }

    public class Document
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string NoiDungChinh { get; set; }
        public string LoaiVanBan { get; set; }
        public string LinhVuc { get; set; }
        public string HanPheDuyet { get; set; }
        public List<string> DinhKemVanBanTrinh { get; set; }
        public List<string> DinhKemVanBanLienQuan { get; set; }
        public string Status { get; set; }
    }
}
