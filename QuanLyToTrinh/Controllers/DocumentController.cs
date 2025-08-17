using Core.Domains;
using Core.QueryModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using QuanLyToTrinh.SMSService;
using Services;
using Services.DTO;
using System.Security.Policy;

namespace QuanLyToTrinh.Controllers
{
    public class DocumentUploadModel
    {
        public byte[] fileData { get; set; }
        public string fileName { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService documentService;
        private readonly INotificationService notificationService;
        private readonly ISMSService smsService;

        public DocumentController(IDocumentService documentService, INotificationService notificationService, ISMSService smsService)
        {
            this.documentService = documentService;
            this.notificationService = notificationService; 
            this.smsService = smsService;
        }        
        [HttpPost("UploadDocument")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentDTO payload, IFormFile document)
        {
            try
            {
                var filePathResult = await documentService.SaveFile(document, payload);
                return Ok(new BaseResponseModel(filePathResult));
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
            }
        }        
        [HttpGet("GetDocumentList")]
        public async Task<IActionResult> GetAllDocumentByStatusCode([FromQuery] GetDocumentListQueryModel query)
        {
            try
            {
                var result = await documentService.GetDocumentsList(query);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
        [HttpGet("GetDocumentById/{id}")]
        public async Task<IActionResult> GetDocumentById(int id, [FromQuery] string? assigneeId)
        {
            try
            {
                var result = await documentService.GetDocumentById(id, assigneeId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("GetDocumentAttachments/{docId}")]
        public async Task<IActionResult> GetDocumentAttachments(int docId)
        {
            try
            {
                var result = await documentService.GetDocumentAttachments(docId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
        [HttpGet("GetDocumentApprovals/{docId}")]
        public async Task<IActionResult> GetDocumentApprovals(int docId)
        {
            try
            {
                var result = await documentService.GetDocumentApprovals(docId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
        [HttpGet("GetDocumentsPie")]
        public async Task<IActionResult> GetDocumentsPie()
        {
            try
            {
                var result = await documentService.pieData();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("GetDocumentsChartMonth")]
        public async Task<IActionResult> GetDocumentsChartMonth()
        {
            try
            {

                var result = await documentService.GetDocumentsByCharMonth();

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("GetDocumentsChartField")]
        public async Task<IActionResult> GetDocumentsChartField()
        {
            try
            {
                var result = await documentService.GetDocumentsByChartField();

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("CountDocumentByStatus")]
        public async Task<IActionResult> CountDocumentByStatus()
        {
            try
            {
                var result = await documentService.CountDocumentByStatus();

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllDocument()
        {
            try
            {
                var result = await documentService.GetAll();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
        [HttpPost("CreateAndSend")]
        public async Task<IActionResult> CreateAndSend([FromForm] DocumentDTO payload)
        {
            try
            {
                var result = await documentService.CreateAndSend(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("CreateDraft")]
        public async Task<IActionResult> CreateDraft([FromForm] DocumentDTO payload)
        {
            try
            {
                var result = await documentService.CreateDraft(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("SignedFile")]
        public async Task<IActionResult> SignedFile(int docId, string userId)
        {
            try
            {
                var result = await documentService.SignedFile(docId, userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("ResultSignedFiles")]
        public async Task<IActionResult> ResultSignedFiles(int docId,string userId, int submitCount)
        {
            try
            {
                var result = await documentService.ResultSignedFiles(docId, userId, submitCount);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("UpdatePriorityDocument")]
        public async Task<IActionResult> UpdatePriorityDocument(int docId,int priorityNumber)
        {
            try
            {
                var result = await documentService.UpdatePriorityDocument(docId, priorityNumber);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("GDSignedFile")]
        public async Task<IActionResult> GDSignedFile(DocumentFileDTO document)
        {
            try
            {
                var result = await documentService.GDSignedFile(document);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("GDSignedFileForce")]
        public async Task<IActionResult> GDSignedFileForce(DocumentFileDTO document)
        {
            try
            {
                var result = await documentService.GDSignedFileForce(document);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("RetrieveDocument")]
        public async Task<IActionResult> Retrieve([FromBody] DocumentRetrievalRequest payload)
        {
            try
            {
                var result = await documentService.RetrieveDocument(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("ReturnDocument")]
        public async Task<IActionResult> ReturnDocument([FromBody] DocumentRetrievalRequest payload)
        {
            try
            {
                var result = await documentService.ReturnDocument(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] DocumentDTO payload, IFormFile[] files)
        {
            try
            {
                var result = await documentService.UpdateDocument(payload, files);

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("Publish/{docId}")]
        public async Task<IActionResult> PublishDocument(int docId, IFormFile[] files)
        {
            try
            {
                var result = await documentService.PublishDocument(docId, files);
                var smsRes = await this.smsService.SendSMS(docId, 7);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        //[HttpPut("UpdateStatus/{docId}/{status}/{handler}")]
        //public async Task<IActionResult> UpdateStatus(int docId, int status, int handler)
        //{
        //    try
        //    {
        //        var result = await documentService.UpdateDocumentStatus(docId, status, handler);

        //        return Ok(new BaseResponseModel(result));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
        //    }
        //}

        [HttpDelete("Delete/{docId}")]
        public async Task<IActionResult> Delete(int docId)
        {
            try
            {
                var result = await documentService.DeleteDocument(docId);

                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        
        [HttpPost("GetFile")]        
        public async Task<IActionResult> GetPdf(GetFileDTO fileInfo)
        {
            if (!System.IO.File.Exists(fileInfo.FilePath))
            {
                // Optionally, you can throw an exception or return null
                // depending on how you want to handle missing files.
                return null;
            }

            // Read the file contents asynchronously
            byte[] fileContents;
            using (FileStream fileStream = new FileStream(fileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                fileContents = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileContents, 0, (int)fileStream.Length);
            }

            //return fileContents;

            if (fileContents == null)
            {
                return NotFound();
            }

            return File(fileContents, "application/pdf", fileInfo.FilePath);
        }

        [HttpPost("PrintResult")]
        public async Task<IActionResult> PrintResult(int docId, string userId,string comment)
        {
            try
            {
                var uri = new System.Uri(HttpContext.Request.GetDisplayUrl());
                var result = await documentService.PrintResult(docId, userId,comment, "https://" + uri.Authority);

                return File(result, "application/pdf", "KetQua.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
        [HttpGet("ProcessApproving")]
        public async Task<IActionResult> ProcessApproving()
        {
            try
            {
                var result = await documentService.ProcessApproving();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
