using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.OfficeChartToImageConverter;
using Syncfusion.Pdf;
using Syncfusion.Presentation;
using Syncfusion.PresentationToPdfConverter;
using Syncfusion.XlsIO;

using UploadFileServer.Models;
using UploadFileServer.Models.Entity;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using WordApplication = Microsoft.Office.Interop.Word.Application;
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using PowerPointApplication = Microsoft.Office.Interop.PowerPoint.Application;

namespace UploadFileServer.Controller
{
    public class WordToPdfController : ApiController
    {
        private readonly EIMEntities _contextEIM = new EIMEntities();
        public class FormDocument : FormCollection
        {
            public Entity ObjName { set; get; }
        }

        [HttpGet]
        public async Task<byte[]> GetItem(int docId)
        {
            var esApi = System.Configuration.ConfigurationManager.AppSettings["EsQLVBUrl"];
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                var result = await client.GetAsync(esApi + "/api/attachment/getattachment/" + docId);
                var contents = await result.Content.ReadAsStringAsync();
                var o = JsonConvert.DeserializeObject<byte[]>(contents);
                return o;
            }
        }

        [HttpGet]
        public async Task<byte[]> GetItemOut(int docId)
        {
            var esApi = System.Configuration.ConfigurationManager.AppSettings["EsQLVBUrl"];
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(30);
                var result = await client.GetAsync(esApi + "/api/attachment/getoutattachment/" + docId);
                var contents = await result.Content.ReadAsStringAsync();
                var o = JsonConvert.DeserializeObject<byte[]>(contents);
                return o;
            }
        }

        [HttpGet]
        public async Task<object> ConvertTo(int? id, string org)
        {
            var retJson = Json(new JMessage());
            var cStr = "Trống";
            var oStr = "Trống";
            try
            {
                if (!string.IsNullOrWhiteSpace(org))
                {
                    oStr = _contextEIM.ConfigDatabases.FirstOrDefault(x => x.OrganizationCode.Trim() == org.Trim())?.Database;
                    cStr = _contextEIM.ConfigDatabases.FirstOrDefault(x => x.OrganizationCode.Trim() == org.Trim())?.ConnectString;
                    int size = 0;
                    if (!string.IsNullOrWhiteSpace(cStr))
                    {
                        DynamicEntities _contextDynamic = new DynamicEntities(cStr);
                        var doc = _contextDynamic.DocumentOutAttachments.FirstOrDefault(x => x.DocumentOutAttachmentID == id);
                        if (doc != null)
                        {
                            var fileBytes = await GetItemOut(doc.DocumentOutAttachmentID);
                            string fileName = Path.GetFileNameWithoutExtension(doc.FileName);
                            string extension = Path.GetExtension(doc.FileName);
                            string destFolder = "/file/";
                            string sourcePath = destFolder + doc.FileName;
                            string destPath = destFolder + fileName + ".signed.pdf";
                            string sourceFullPath = HttpContext.Current.Server.MapPath("~" + sourcePath);
                            string destFullPath = HttpContext.Current.Server.MapPath("~" + destPath);
                            size += fileBytes.Length;
                            //Read
                            Stream sourceStream = new MemoryStream(fileBytes);
                            sourceStream.Position = 0;
                            //Convert
                            List<string> wordExtensions = new List<string>() { ".doc", ".docx", ".dot", ".dotx", ".dotm", ".docm", ".rtf" };
                            List<string> powerpointExtensions = new List<string>() { ".ppt", ".pptx", ".pot", ".potx", ".potm", ".pptm" };
                            List<string> excelExtensions = new List<string>() { ".xls", ".xlsx", ".xlt", ".xltx", ".xltm", ".xlsm" };
                            if (wordExtensions.Contains(extension))
                            {
                                TransformToOriginalFile(sourceFullPath, fileBytes);
                                ConvertWordToPdf(sourceFullPath, destFullPath);
                            }
                            else if (excelExtensions.Contains(extension))
                            {
                                TransformToOriginalFile(sourceFullPath, fileBytes);
                                ConvertExcelToPdf(sourceFullPath, destFullPath);
                            }
                            else if (powerpointExtensions.Contains(extension))
                            {
                                TransformToOriginalFile(sourceFullPath, fileBytes);
                                ConvertPowerPointToPdf(sourceFullPath, destFullPath);
                            }
                            else if (extension.Contains("pdf"))
                            { 
                                using (FileStream fs = new FileStream(destFullPath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    fs.Write(fileBytes, 0, (int)fileBytes.Length);
                                    fs.Close();
                                }
                            }
                            else
                            {
                                return Json(new JMessage { idvanban = fileName + extension, Title = $"File không đúng định dạng Office", Error = true });
                            }
                            retJson = Json(new JMessage { idvanban = fileName, Title = destPath, Error = false });
                        }
                        else
                        {
                            return Json(new JMessage { idvanban = $"Không tìm thấy tài liệu có ID = {id} của đơn vị {org}", Title = $"Vui lòng kiểm tra lại thông tin 🗄 {oStr}", Error = true });
                        }
                    }
                    else
                    {
                        if (oStr == null || oStr == "null") oStr = "Không lấy được thông tin đơn vị từ CAS";
                        retJson = Json(new JMessage { idvanban = $"Không tìm thấy cấu hình chuỗi kết nối SQL cho mã đơn vị {org}", Title = $"Vui lòng kiểm tra lại thông tin mã đơn vị 🗄 {oStr}", Error = true });
                    }
                }
                else
                {
                    retJson = Json(new JMessage { idvanban = $"Không tìm thấy đơn vị có mã {org} từ CAS", Title = "Vui lòng kiểm tra lại thông tin mã đơn vị", Error = true });
                }
            }
            catch (Exception ex)
            {
                retJson = Json(new JMessage { idvanban = $"ConnectionString = {cStr}", Title = Newtonsoft.Json.JsonConvert.SerializeObject(ex), Error = true });
            }
            return retJson;
        }

        public class DocumentOut
        {
            public string FileName { set; get; }
            public byte[] Attachment { set; get; }
            public int? DocumentOutAttachmentID { set; get; }
        }

        private void TransformToOriginalFile(string fileLocation, byte[] fileData)
        {
            try
            {
                //checking if file exists
                if (File.Exists(fileLocation))
                {
                    File.Delete(fileLocation);
                }
                FileStream fileStream = new FileStream(fileLocation, FileMode.Create,
                FileAccess.Write, FileShare.ReadWrite);
                //creating binary file using BinaryWriter
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    //writing data using different Write() methods of BinaryWriter
                    binaryWriter.Write(fileData);
                    binaryWriter.Flush();
                    binaryWriter.Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void ConvertWordToPdf(string sourceFile, string destinationFile)
        {
            var wordApp = new WordApplication();
            try
            {
                var wordDocument = wordApp.Documents.Open(sourceFile);
                wordDocument.ExportAsFixedFormat(destinationFile, WdExportFormat.wdExportFormatPDF);
                wordDocument.Close(WdSaveOptions.wdDoNotSaveChanges, WdOriginalFormat.wdOriginalDocumentFormat, false); //Close document
                wordApp.Quit(); //Important: When you forget this Word keeps running in the background
            }
            catch (Exception ex)
            {
                var exMess = ex.Message;
            }
        }

        public void ConvertExcelToPdf(string sourceFile, string destinationFile)
        {
            var excelApp = new ExcelApplication();
            var excelDocument = excelApp.Workbooks.Open(sourceFile);
            excelDocument.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, destinationFile);
            excelDocument.Close(false, "", false); //Close document
            excelApp.Quit(); //Important: When you forget this Excel keeps running in the background
        }

        public void ConvertPowerPointToPdf(string sourceFile, string destinationFile)
        {
            var powerpointApp = new PowerPointApplication();
            //var powerpointDocument = powerpointApp.Presentations.Open(sourceFile,
            //    Microsoft.Office.Core.MsoTriState.msoTrue, //ReadOnly
            //    Microsoft.Office.Core.MsoTriState.msoFalse, //Untitled
            //    Microsoft.Office.Core.MsoTriState.msoFalse); //Window not visible during converting

            //powerpointDocument.ExportAsFixedFormat(destinationFile, PpFixedFormatType.ppFixedFormatTypePDF);
            //powerpointDocument.Close(); //Close document
            //powerpointApp.Quit(); //Important: When you forget this PowerPoint keeps running in the background
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }
        public class Entity
        {
            public int ID { set; get; }
            public string OrganizationID { set; get; }
        }
        public class FormCollection
        {
            public FormData DataFile { set; get; }
        }
        public class FormData
        {
            public string Name { get; set; }
            public int? Type { set; get; }
            public int? Version { set; get; }
            public byte[] Datafile { get; set; }
            public FormData()
            { }
            public FormData(string _name, byte[] _data, int? type, int? version)
            {
                this.Name = _name; this.Datafile = _data; Type = type; Version = version;
            }
        }
    }
}
