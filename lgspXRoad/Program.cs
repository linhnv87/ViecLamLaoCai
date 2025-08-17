using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using VPCPXRoadSdk;
using VPCPXRoadSdk.eDocument;
using VPCPXRoadSdk.eDocument.Model;
using VPCPXRoadSdk.Execute;
using System.Linq;
using System.Xml.Linq;
using LgspXRoadSdk;
using System.Configuration;
using EdXML102.Header;
using System.IO;
using EdXML102;
using System.Data.Linq;
using EdStatus;
using System.Net;
using System.Xml;

namespace LgspXRoadSdk
{

  class Program
  {
    const string TypeEdoc = "eDoc";
    const string TypeStatus = "status";
    private static string StorePathDirUnit = ConfigurationManager.AppSettings["StorePathDir"];
    static void Main(string[] args)
    {
      Console.InputEncoding = System.Text.Encoding.UTF8;
      Console.OutputEncoding = System.Text.Encoding.UTF8;

      // out docs
      //string sysid = "001.00.99.H44";
      //string key = "AlSRaGBAdFA3oYElJ1S60ySYBg1bGmRluifm4eUDLdan";          



      // Indoc
      string sysid = ConfigurationManager.AppSettings["SystemId"];// "002.00.99.H44";
      string key = ConfigurationManager.AppSettings["SecretKey"];// "AzWgHDGEwNi9MnE6zpYdi/CLf6DjFguDWRm3TEE9gcks";
                                                                 //string storePathDir = ConfigurationManager.AppSettings["StorePathDir"]; 

      //StorePathDirUnit = storePathDir;

      try
      {
        // Tao thu muc chua file neu khong ton tai
        CreateFolderXroa(StorePathDirUnit);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message + ex.StackTrace);
      }

      try
      {
        Console.WriteLine("==================Start InDocsRevoke=========");
        InDocsRevoke(sysid, key);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message + ex.StackTrace);
      }

      try
      {
        Console.WriteLine("==================Start OutDocsRevoke=========");
        OutDocsRevoke(sysid, key);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message + ex.StackTrace);
      }
      try
      {
        Console.WriteLine("==================Start OutDocs=========");
        OutDocs(sysid, key);
      }
      catch (Exception ex)
      {

        Console.WriteLine(ex.Message + ex.StackTrace);
      }
      try
      {
        Console.WriteLine("==================Start InDocs=========");
        InDocs(sysid, key);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message + ex.StackTrace);

      }

      try
      {
        Console.WriteLine("==================Start UpdateOutDocs=========");
        UpdateOutDocs(sysid, key);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message + ex.StackTrace);

      }






      // - trả lại
      // - đã nhận, 
      // - đồng ý thu hồi
      //UpdateInDocs(sysid, key);

      // - thu hồi
      // - trạng thái đã nhận
      // - đồng ý nhận trả lại
      //UpdateOutDocs(sysid, key);


      ////Tao doi tuong config doc tu file app.config
      //ClientConf conf = new ClientConf(true); //create config from App.config

      ////ClientConf conf = new ClientConf(false); //manual asign config parram
      ////conf.Endpoint = "http://10.252.9.41:8280";
      ////conf.Protocol = NetworkProtocol.HTTPS;
      ////conf.
      ////Tao doi tuong VPCPXRoadSdkClient dung de tao các service can su dung
      //VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
      //vpcp.type = ServiceType.eDoc; //create DocumentService

      ////Tao doi tuong service loai DocumentService
      //DocumentService service = (DocumentService)vpcp.CreateService(conf);



      ////int function = 4;
      ////switch (function)
      ////{
      ////    //Lay danh sach don vi
      ////    case 0: GetAgencyList(service); break;
      ////    //Lay danh sach don vi lien thong
      ////    case 1: GetListUnit(service); break;
      ////    //Dang ky don vi lien thong
      ////    case 2: RegisterAgency(service); break;
      ////    //Xoa dang ky don vi lien thong
      ////    case 3: DeleteAgency(service); break;
      ////    //Lay danh sach van ban nhan
      ////    case 4: GetReceivedEdocList(service); break;
      ////    //Lay danh sach van ban gui
      ////    case 5: GetSentEdocList(service); break;
      ////    //Gui goi tin van ban
      ////    //case 6: SendEdoc(service); break;
      ////    //Nhan goi tin van ban
      ////    case 7: GetEdoc(service); break;
      ////    //Cap nhat trang thai cho goi tin van ban
      ////    case 8: UpdateStatus(service); break;
      ////}

      //Console.ReadKey();
    }

    private static void CreateFolderXroa(string storePathDir)
    {
      if (!System.IO.Directory.Exists(storePathDir))
      {
        System.IO.Directory.CreateDirectory(storePathDir);
      }
    }

    private static void UpdateOutDocs(string sysid, string key)
    {
      // - thu hồi Lgsp 13
      UpdateByStatusOutDocs(sysid, key, "13");
      //// - trạng thái đã nhận 
      //UpdateCancel02OutDocs(sysid, key);
      //// - đồng ý nhận trả lại
      //UpdateStatus16OutDocs(sysid, key);
    }

    private static void UpdateByStatusOutDocs(string sysid, string key, string v)
    {
      ClientConf conf = new ClientConf(true);
      conf.SystemId = sysid;
      conf.SecretKey = key;
      VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
      vpcp.type = ServiceType.eDoc; //create DocumentService

      //Tao doi tuong service loai DocumentService
      DocumentService service = (DocumentService)vpcp.CreateService(conf);

      List<string> listDocId = GetReceivedEdocList(service, eDocType.StatusDocument);

      if (listDocId.Any())
      {
        foreach (var item in listDocId)
        {
          try
          {
            string fileEdxml = GetEdoc(service, item);
            EdXML102.EdXml102 itemXml = new EdXML102.EdXml102();
            itemXml.FromFile(fileEdxml);

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(fileEdxml);
            XmlNodeList nodeList = xmldoc.GetElementsByTagName("edXML:StatusCode");
            string StatusCode = string.Empty;
            foreach (XmlNode node in nodeList)
            {
              StatusCode = node.InnerText;
            }

            if (itemXml.Bussiness.BussinessDocType == 0)
            {
              string OrgSend = itemXml.From.OrganId;
              var CurrentOrg = itemXml.ResponseFor;
              foreach (var curr in CurrentOrg)
              {
                if (curr.OrganId == sysid)
                {
                  var PromulgationDate = curr.PromulgationDate;
                  var NotationFull = curr.DocumentId;
                  try
                  {
                    NotationFull = NotationFull.Split(',')[2];
                  }
                  catch
                  {
                    // continue when error structure
                    break;
                  }

                  // mapping status LGSP to QLVB 1;2;3;4;5;6 EnumStatusUpdate.inbox tuowng unng duoc lay tu StatusCode truc lgsp tra ve
                  bool isUpdated = UpdateDocStatus(sysid, OrgSend, PromulgationDate, NotationFull, getStat(StatusCode), itemXml, xmldoc);

                  string pathFolder = StorePathDirUnit + @"/" + item;
                  DeleteDirectory(pathFolder);

                  if (isUpdated)
                  {
                    UpdateStatus(service, item, "done");
                    Console.WriteLine("Cap nhat trang thai van ban thanh cong: DocNotation: " + NotationFull);
                  }
                  else
                  {
                    Console.WriteLine("Cap nhat trang thai van ban KHONG thanh cong: DocNotation: " + NotationFull);
                  }
                  break;
                }
                else
                  continue;
              }
            }
          }
          catch { }
        }
      }
    }

    private static EnumStatusUpdate getStat(string code)
    {
      switch (code)
      {
        case "01":
          return EnumStatusUpdate.inbox; // 1 = "Đã tiếp nhận"; 
        case "02":
          return EnumStatusUpdate.reject;
        case "03":
          return EnumStatusUpdate.acceptance;
        case "04":
          return EnumStatusUpdate.assignment;
        case "05":
          return EnumStatusUpdate.processing;
        case "06":
          return EnumStatusUpdate.finish;
        default:
          return EnumStatusUpdate.inbox;
      }
    }
    private static void UpdateStatus16OutDocs(string sysid, string key)
    {
      throw new NotImplementedException();
    }

    private static void UpdateCancel02OutDocs(string sysid, string key)
    {
      throw new NotImplementedException();
    }

    private static void UpdateCancel13OutDocs(string sysid, string key)
    {
      throw new NotImplementedException();
    }

    private static void UpdateInDocs(string sysid, string key)
    {
      // - trả lại LGSP = 02 / QLVB CLient Status = 2 and IsLGSP=1 and LGSP Status # 2 at ESB.DocumentReceived 
      // - Trả lại thành công thì update LGSP Status = 2 at ESB.DocumentReceived
      UpdateReject02InDocs(sysid, key, "02");
      // - đã nhận,  LGSP = 03
      UpdateReject02InDocs(sysid, key, "03");
      // - đồng ý yc thu hồi LGSP = 15
      UpdateReject02InDocs(sysid, key, "15");
    }

    private static void UpdateReject02InDocs(string sysid, string key, string statusCodeSendToLgsp)
    {
      ClientConf conf = new ClientConf(true);
      conf.SystemId = sysid;
      conf.SecretKey = key;
      VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
      vpcp.type = ServiceType.eDoc; //create DocumentService

      string guid = Guid.NewGuid().ToString();
      //Tao doi tuong service loai DocumentService
      DocumentService service = (DocumentService)vpcp.CreateService(conf);
      string pathFolder = StorePathDirUnit + @"/" + guid;
      System.IO.Directory.CreateDirectory(pathFolder);
      ESBDTDataContext ct = new ESBDTDataContext();

      // get list to Rejected from DB ESB QLVB status = 03  ; LGSP Status = 02
      int codeQlvb = 0;
      switch (statusCodeSendToLgsp)
      {
        //Tao goi tin status da den
        case "01": codeQlvb = 1; break;
        //Tao goi tin status tu choi
        case "02": codeQlvb = 2; break;
        //Tao goi tin status tiep nhan
        case "03": codeQlvb = 1; break;
          //Tao goi tin status phan con xu ly
          //case "04":   break;
          //Tao goi tin status dang xu ly
          //case "05": ; break;
          //Tao goi tin status hoan thanh
          //case "06":  break;
          //Tao goi tin status lay lai
          //case "13": codeQlvb = 5; break;
          //Tao goi tin status dong y cap nhat lay lai/thu hoi
          //case "15": DongYCapNhatLayLai(pathFolder, objSender, itemRj, guid + "status_change-acceptance_15.edxml"); break;
          ////Tao goi tin status tu choi cap nhat lay lai/thu hoi
          //case "16": TuChoiCapNhatLayLai(pathFolder, objSender, itemRj, guid + "status_change-rejection_16.edxml"); break;

      }
      List<DocumentReceiver> listRejectd = GetListRejected(sysid, ct, codeQlvb, int.Parse(statusCodeSendToLgsp));

      if (listRejectd != null && listRejectd.Any())
      {
        foreach (var itemRj in listRejectd)
        {
          // get Send Doc Infor
          DocumentSender obSender = GetSenderByCode(itemRj.SendOrganizationID, ct);
          if (obSender != null)
          {
            // Build package edxml to send
            string pathPackage = LgspXRoadSdk.ultils.writeStatus(pathFolder, obSender, itemRj, guid, statusCodeSendToLgsp);
            // Send package
            SendEdoc(service, sysid, null, pathPackage, TypeStatus);

            //update Send to LGSP done  
            itemRj.IsLgsp = true;
            itemRj.StatusLgsp = int.Parse(statusCodeSendToLgsp);
            ct.SubmitChanges();
          }
        }
      }
    }

    private static DocumentSender GetSenderByCode(string sendOrganizationID, ESBDTDataContext ct)
    {
      try
      {
        DocumentSender list = ct.DocumentSenders.Where(p => p.SendOrganizationID == sendOrganizationID).FirstOrDefault();
        if (list != null) return list;
        return null;
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    private static List<DocumentReceiver> GetListRejected(string sysid, ESBDTDataContext ct, int StatusCodeQlvb, int StatusCodeLgsp)
    {
      try
      {
        // Lấy văn bản nào có Status = StatusCodeQlvb (là VB cần cập nhật gửi đi qua LGSP) và văn có StatusLgsp chưa được cập nhập = StatusCodeLgsp (nếu đã cập nhật = StatusCodeLgsp rồi thì là đã gửi LGSP rồi)
        List<DocumentReceiver> list = ct.DocumentReceivers.Where(p => p.ReceiveOrganizationID == sysid && p.IsLgsp == true && p.Status == StatusCodeQlvb && p.StatusLgsp != StatusCodeLgsp).ToList();
        if (list.Any()) return list;
        return null;
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    private static void InDocs(string sysid, string key)
    {
      ClientConf conf = new ClientConf(true);
      conf.SystemId = sysid;
      conf.SecretKey = key;
      VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
      vpcp.type = ServiceType.eDoc; //create DocumentService

      //Tao doi tuong service loai DocumentService
      DocumentService service = (DocumentService)vpcp.CreateService(conf);

      List<string> listDocId = GetReceivedEdocList(service, eDocType.ElectronicDocument);

      if (listDocId.Any())
      {
        foreach (var item in listDocId)
        {
          try
          {

            string fileEdxml = GetEdoc(service, item);
            EdXML102.EdXml102 itemXml = new EdXML102.EdXml102();
            itemXml.FromFile(fileEdxml);

            if (itemXml.Bussiness.BussinessDocType != 0)
            {
              Console.WriteLine("BussinessDocType:" + itemXml.Bussiness.BussinessDocType);

              //File.Delete(fileEdxml);
              string pathFolder = StorePathDirUnit + @"/" + item;
              DeleteDirectory(pathFolder);

              Console.WriteLine("Delete file edxml:");

              continue;
            }

            if (itemXml.Bussiness.BussinessDocType == 0)
            {
              var ExistSender = CheckExistSender(item, itemXml.Code.Number, itemXml.From.OrganId);
              bool IsExistRecei = CheckExistReceived(item, sysid, itemXml.Code.Number, itemXml.From.OrganId);

              if (ExistSender != null && IsExistRecei)
              {
                UpdateStatus(service, item, "done");

                continue;
              }
              else if (ExistSender != null && (!IsExistRecei))
              {
                DocumentSender objSend = ExistSender;
                Console.WriteLine("ExistSender:" + ExistSender.NotationNumber);
                InsertReceiverInfor(itemXml, item, objSend, sysid);
                Console.WriteLine("InsertReceiverInfor:" + item);

                InsertAttachments(itemXml, objSend, item);
                Console.WriteLine("InsertAttachments:" + item);
                try
                {
                  string pathFolder = StorePathDirUnit + @"/" + item;
                  DeleteDirectory(pathFolder);
                }
                catch (Exception ex)
                {
                  Console.WriteLine("DeleteDirectory:" + ex.Message);
                }
                UpdateStatus(service, item, "done");
                Console.WriteLine("Them moi van ban den thanh cong: DocId: " + item);
              }
              else
              {
                DocumentSender objSend = InsertSenderInfor(itemXml, item);
                Console.WriteLine("InsertSenderInfor:" + item);

                InsertReceiverInfor(itemXml, item, objSend, sysid);
                Console.WriteLine("InsertReceiverInfor:" + item);

                InsertAttachments(itemXml, objSend, item);
                Console.WriteLine("InsertAttachments:" + item);
                try
                {
                  string pathFolder = StorePathDirUnit + @"/" + item;
                  DeleteDirectory(pathFolder);
                }
                catch (Exception ex)
                {
                  Console.WriteLine("DeleteDirectory:" + ex.Message);
                }
                UpdateStatus(service, item, "done");
                Console.WriteLine("Them moi van ban den thanh cong: DocId: " + item);
              }
            }

          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            continue;
          }
        }
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sysid"></param>
    /// <param name="fromUnit"></param>
    /// <param name="promulgationDate"></param>
    /// <param name="notationFull"></param>
    /// <param name="status">1;2;3;4;5</param>
    /// <returns></returns>
    private static bool UpdateDocStatus(string sysid, string fromUnit, DateTime promulgationDate, string notationFull, EnumStatusUpdate status, EdXml102 itemXml, XmlDocument nodeMessageHeader)
    {
      ESBDTDataContext ct = new ESBDTDataContext();
      DocumentSender sendObj = ct.DocumentSenders.Where(p => p.SendOrganizationID == sysid && p.DocumentDate.Value.Date == promulgationDate.Date && p.NotationNumber.ToUpper() == notationFull.ToUpper()).FirstOrDefault();
      if (sendObj != null)
      {
        Console.WriteLine("Co ton tai Van ban gui :" + notationFull);
        var receiver = sendObj.DocumentReceivers.Where(p => p.ReceiveOrganizationID == fromUnit).FirstOrDefault();
        if (receiver != null)
        {
          string Description = string.Empty;
          XmlNodeList nodeList = nodeMessageHeader.GetElementsByTagName("edXML:Description");
          string StatusCode = string.Empty;
          foreach (XmlNode node in nodeList)
          {
            Description = node.InnerText;
          }
          Console.WriteLine("Da cap nhat trang thai VB mo ta: " + Description + " Don vi gui trong esb.DocumentReceivers:" + fromUnit);
          receiver.StatusLgsp = (int?)status; // trạng thái từ LGSP trả về
          receiver.ModifiedDate = DateTime.Now;
          receiver.Comment = Description;

          CheckESBDocumentStatus(ct, receiver, status);
          ct.SubmitChanges();
          Console.WriteLine("Da cap nhat trang thai VB: " + notationFull + " Don vi gui trong esb.DocumentReceivers:" + fromUnit);
        }
        else
        {
          if (sendObj.CreatedDate.Value <= DateTime.Now.AddDays(-90))
          {
            Console.WriteLine("Thoi gian qua 6 thang: Van ban gui DocumentSenders:" + notationFull + " Ngay tao: " + sendObj.CreatedDate);
            return true;
          }
          else
          {
            Console.WriteLine("Khong ton tai Don vi gui trong esb.DocumentReceivers:" + fromUnit + " Ngay tao: " + sendObj.CreatedDate);
            return false;
          }
        }
        return true;
      }
      else
      {
        DocumentSender sendObj1 = ct.DocumentSenders.Where(p => p.SendOrganizationID == sysid && p.DocumentDate == promulgationDate && p.NotationNumber.ToUpper() == notationFull.ToUpper() && p.CreatedDate.Value >= DateTime.Now.AddDays(-180)).FirstOrDefault();

        if (sendObj1 != null)
        {
          Console.WriteLine("Thoi gian qua 6 thang: Van ban gui DocumentSenders:" + notationFull + " Ngay tao: " + sendObj.CreatedDate);
          return true;
        }
        else
        {
          Console.WriteLine("Khong ton tai Van ban gui DocumentSenders:" + notationFull);
          return false;
        }
      }
    }

    private static void DeleteDirectory(string item)
    {
      string[] files = Directory.GetFiles(item);
      string[] dirs = Directory.GetDirectories(item);

      foreach (string file in files)
      {
        File.SetAttributes(file, FileAttributes.Normal);
        File.Delete(file);
      }

      foreach (string dir in dirs)
      {
        DeleteDirectory(dir);
      }

      Directory.Delete(item, false);
    }

    private static void InsertAttachments(EdXml102 itemXml, DocumentSender objSender, string docId)
    {
      ESBDTDataContext ct = new ESBDTDataContext();

      foreach (var objAtt in itemXml.FileAttachList)
      {
        DocumentAttachment objFile = new DocumentAttachment();
        objFile.SenderID = objSender.ID;
        objFile.FileName = objAtt.FileName;
        objFile.DocumentGuidID = objSender.DocumentGuidID.ToString();
        objFile.Type = 2;
        objFile.CreatedDate = DateTime.Now;
        objFile.CreatedBy = "LGSP Job";
        objFile.ModifiedDate = DateTime.Now;
        objFile.Attachment = GetAttachByEdxml(objAtt, docId);

        ct.DocumentAttachments.InsertOnSubmit(objFile);
      }
      ct.SubmitChanges();
    }

    private static Binary GetAttachByEdxml(EdXML102.Attachment.FileAttach objAtt, string docId)
    {
      string guidFolder = StorePathDirUnit + "/" + docId;
      string file = guidFolder + "/" + objAtt.FileName;
      objAtt.WriteFile(file);
      byte[] byteFile = File.ReadAllBytes(file);

      return (Binary)byteFile;
      //throw new NotImplementedException();
    }

    private static void InsertReceiverInfor(EdXml102 itemXml, string docId, DocumentSender objSender, string sysid)
    {
      ESBDTDataContext ct = new ESBDTDataContext();
      CASDataContext ctca = new CASDataContext();

      foreach (var objTo in itemXml.ToList)
      {
        if (sysid == objTo.OrganId)
        {
          DocumentReceiver objRec = new DocumentReceiver();
          objRec.SenderID = objSender.ID;
          objRec.ReceiveOrganizationGuidID = objTo.OrganId;
          objRec.ReceiveOrganizationID = objTo.OrganId;
          objRec.ReceiveOrganizationName = objTo.OrganName;
          objRec.SendOrganizationID = itemXml.From.OrganId;
          objRec.SendOrganizationName = itemXml.From.OrganName;
          objRec.Status = 0;
          objRec.DocumentGuidID = objSender.DocumentGuidID.ToString();
          objRec.CreatedDate = DateTime.Now;
          objRec.CreatedBy = "LGSP Job";
          objRec.ModifiedDate = DateTime.Now;
          objRec.IsLocal = false;
          objRec.IsEmail = false;
          objRec.DocumentLgspGuidID = docId;
          objRec.IsLgsp = true;
          objRec.StatusLgsp = 1;
          objRec.RowGuid = new Guid(docId);

          var org = ctca.ESOrganizations.Where(p => p.OrgID == objTo.OrganId).FirstOrDefault();

          if (org != null)
          {
            objRec.Key = org.Key;
            objRec.AccessKey = org.AccessKey;
          }
          //update status to table esb].[DocumentStatus
          ct.DocumentReceivers.InsertOnSubmit(objRec);
          break;
        }
      }
      ct.SubmitChanges();
      //ESBDTDataContext ct = new ESBDTDataContext();
      //CASDataContext ctca = new CASDataContext();

      //var listOrgA44 = ctca.ESOrganizations.Where(p => p.OrgID.ToLower().Contains(".a44")).Select(c => c.OrgID).ToList();
      //Console.WriteLine("list Org A44: " + listOrgA44.Count);
      //foreach (var objTo in itemXml.ToList)
      //{
      //    Console.WriteLine("objTo.OrganId: " + objTo.OrganId);
      //    if (listOrgA44.Any(str => str.Contains(objTo.OrganId)))
      //    {
      //        Console.WriteLine("objTo.OrganId: " + objTo.OrganId);
      //        DocumentReceiver objRec = new DocumentReceiver();
      //        objRec.SenderID = objSender.ID;
      //        objRec.ReceiveOrganizationGuidID = objTo.OrganId;
      //        objRec.ReceiveOrganizationID = objTo.OrganId;
      //        objRec.ReceiveOrganizationName = objTo.OrganName;
      //        objRec.SendOrganizationID = itemXml.From.OrganId;
      //        objRec.SendOrganizationName = itemXml.From.OrganName;
      //        objRec.Status = 0;
      //        objRec.DocumentGuidID = objSender.DocumentGuidID.ToString();
      //        objRec.CreatedDate = DateTime.Now;
      //        objRec.CreatedBy = "LGSP Job";
      //        objRec.ModifiedDate = DateTime.Now;
      //        objRec.IsLocal = false;
      //        objRec.IsEmail = false;
      //        objRec.DocumentLgspGuidID = docId;
      //        objRec.IsLgsp = true;
      //        objRec.StatusLgsp = 1;
      //        objRec.RowGuid = new Guid(docId);

      //        var org = ctca.ESOrganizations.Where(p => p.OrgID == objTo.OrganId).FirstOrDefault();

      //        if (org != null)
      //        {
      //            objRec.Key = org.Key;
      //            objRec.AccessKey = org.AccessKey;
      //        }
      //        //update status to table esb].[DocumentStatus
      //        ct.DocumentReceivers.InsertOnSubmit(objRec);
      //        break;
      //    }
      //}
      //ct.SubmitChanges();
    }

    private static DocumentSender InsertSenderInfor(EdXml102 itemXml, string docId)
    {
      ESBDTDataContext ct = new ESBDTDataContext();

      DocumentSender objSender = new DocumentSender();
      objSender.SendOrganizationGuidID = itemXml.From.OrganId;
      objSender.SendOrganizationID = itemXml.From.OrganId;
      objSender.SendOrganizationName = itemXml.From.OrganName;
      objSender.DocumentGuidID = Guid.NewGuid();
      objSender.NotationNumber = itemXml.Code.Number + "/" + itemXml.Code.Notation;
      objSender.DocumentNumber = itemXml.Code.Number;
      objSender.Title = itemXml.Subject.Value;
      objSender.DocumentDate = itemXml.Promulgation.Date != null ? itemXml.Promulgation.Date : DateTime.Parse("1900/01/01");
      objSender.DocumentTypeID = "/CV-";// itemXml.Document.Type;
      objSender.DocumentTypeName = "Công văn";// itemXml.Document.Name;
      objSender.UrgencyID = itemXml.OtherInfo.Priority == 0 ? "VB_Thuong" : "VB_khan";
      objSender.Status = 0;
      objSender.CreatedDate = DateTime.Now;
      objSender.CreatedBy = "LGSP_Job";
      objSender.ModifiedDate = DateTime.Now;
      objSender.Signers = itemXml.SignerInfo != null ? itemXml.SignerInfo.FullName : "";
      objSender.Type = "VB_DI";
      objSender.DocumentLgspId = docId;
      // objSender.ExpriedDate = itemXml.DueDate.Value != null ? itemXml.DueDate.Value : DateTime.Parse("1900/01/01");

      ct.DocumentSenders.InsertOnSubmit(objSender);
      ct.SubmitChanges();
      return objSender;
    }

    private static bool CheckExistReceived(string item, string Reciorg, string docNumber, string SendOrg)
    {
      Console.WriteLine("Org Rêci" + Reciorg + "docNumber:" + docNumber + "Org send" + SendOrg);
      ESBDTDataContext ct = new ESBDTDataContext();
      var docListRecei = ct.DocumentReceivers.Where(p => p.DocumentLgspGuidID == item && p.SendOrganizationID == SendOrg && p.ReceiveOrganizationID == Reciorg).FirstOrDefault();
      if (docListRecei != null)
      {
        Console.WriteLine("CheckExist rêci is true");
        return true;
      }
      else
      {
        Console.WriteLine("CheckExist rêci is false");
        return false;
      }
    }
    private static DocumentSender CheckExistSender(string item, string docNumber, string SendOrg)
    {
      Console.WriteLine("docNumber:" + docNumber + "Org send" + SendOrg);
      ESBDTDataContext ct = new ESBDTDataContext();
      var docListSender = ct.DocumentSenders.Where(p => p.DocumentLgspId == item && p.SendOrganizationID == SendOrg).FirstOrDefault();
      if (docListSender != null)
      {
        Console.WriteLine("CheckExist Sender is True");
        return docListSender;
      }
      else
      {
        Console.WriteLine("CheckExist Sender is false");
        return null;
      }
    }

    static void OutDocs(string SystemId, string SecretKey)
    {
      //Tao doi tuong config doc tu file app.config
      ClientConf conf = new ClientConf(true); //create config from App.config

      //ClientConf conf = new ClientConf(false); //manual asign config parram
      //conf.Endpoint = "http://10.252.9.41:8280";
      //conf.Protocol = NetworkProtocol.HTTPS;
      //conf.SystemId 
      //Tao doi tuong VPCPXRoadSdkClient dung de tao các service can su dung
      ESBDTDataContext ct = new ESBDTDataContext();
      CASDataContext ctCas = new CASDataContext();

      string idSettingOrg = SystemId.ToLower();
      if (idSettingOrg == "02.44.W00")
      {
        idSettingOrg = ".a44";
      }
      Console.WriteLine("Send Org:" + SystemId);
      var listOrgA44 = ctCas.ESOrganizations.Where(p => p.OrgID.ToLower().Contains(idSettingOrg)).Select(c => c.OrgID).ToList();

      if (idSettingOrg != "02.44.W00")
      {
        listOrgA44 = listOrgA44.Where(p => p.ToLower().Equals(idSettingOrg)).ToList();
      }
      Console.WriteLine("list Org A44:" + listOrgA44.Count + " " + listOrgA44.FirstOrDefault());

      var docListSend = ct.DocumentSenders.Where(p => listOrgA44.Contains(p.SendOrganizationID) && (p.Status == 0 && p.Type == "VB_DI" && p.DocumentLgspId == null && p.CreatedDate.Value >= DateTime.Now.AddDays(-27))).ToList();
      Console.WriteLine("Send Org:" + SystemId + "docListSend:" + docListSend);
      if (docListSend.Any())
      {
        Console.WriteLine("Send list:" + docListSend.Count);
        foreach (var docSend in docListSend)
        {
          try
          {
            Console.WriteLine("Send ID:" + docSend.DocumentGuidID + " " + docSend.Title);
            var orgLgsp = ctCas.ESOrganizations.Where(p => p.IsLgsp == true);
            if (!orgLgsp.Any()) continue;

            conf.SystemId = SystemId;
            conf.SecretKey = SecretKey;
            VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
            vpcp.type = ServiceType.eDoc; //create DocumentService

            //Tao doi tuong service loai DocumentService
            DocumentService service = (DocumentService)vpcp.CreateService(conf);

            var docRecei = docSend.DocumentReceivers.Where(p => orgLgsp.Select(c => c.OrgID).Contains(p.ReceiveOrganizationID));

            if (docRecei.Any())
            {
              string guidFolder = StorePathDirUnit + "/" + System.Guid.NewGuid().ToString();
              System.IO.Directory.CreateDirectory(guidFolder);

              string[] arrSendTos = ArrSendTo(docRecei.ToList());
              Console.WriteLine("Build edxml:");
              string filePathEdxml = EdXmlFile(docSend, guidFolder);
              Console.WriteLine("filePathEdxml:" + filePathEdxml);
              string docGuid = "";
              try
              {
                docGuid = SendEdoc(service, docSend.SendOrganizationID, arrSendTos, filePathEdxml, TypeEdoc);
              }
              catch (Exception ex)
              {
                if (docGuid == "" || docGuid == "0")
                {
                  docSend.DocumentLgspId = docGuid;
                  docSend.Status = -1; // Đã gửi lên trục có lỗi trả về

                  foreach (var item in docRecei)
                  {
                    item.StatusLgsp = -1; // Đã gửi lên trục lỗi
                    item.DocumentLgspGuidID = "";
                    item.Comment = "Có lỗi khi gửi trục LGSP, Hệ thống đang thử gửi lại: " + ex.Message;// "Có lỗi khi gửi trục LGSP"; //ex.Message;
                    item.IsLgsp = true;
                    bool isUpdated = UpdateDocStatus(SystemId, item.ReceiveOrganizationID, docSend.DocumentDate.Value, docSend.NotationNumber, 0, null, null); //0=dang xu ly

                  }
                  ct.SubmitChanges();
                }
              }
              Console.WriteLine("SendEdoc done:" + docGuid);
              if (docGuid != "" && docGuid != "0")
              {
                docSend.DocumentLgspId = docGuid;
                docSend.Status = 1; // Đã gửi lên trục thành công

                foreach (var item in docRecei)
                {
                  item.StatusLgsp = 1; // Đã gửi lên trục thành công inbox
                  item.DocumentLgspGuidID = "";
                  item.IsLgsp = true;
                  item.Comment = "Đã gửi thành công đến trục LGSP";
                  ct.SubmitChanges();

                  bool isUpdated = UpdateDocStatus(SystemId, item.ReceiveOrganizationID, docSend.DocumentDate.Value, docSend.NotationNumber, 0, null, null);

                }


              }
              else
              {
                Console.WriteLine("[ERROR] SendEdoc Fail:");
              }

              try
              {
                DeleteDirectory(guidFolder);
                Console.WriteLine("Delete folder done:" + guidFolder);
              }
              catch (Exception ex)
              {
                throw ex;
              }
            }

          }
          catch (Exception ex)
          { 
            //Continue
            //throw;
          }
        }
      }
      else
      {
        Console.WriteLine("Not found Sender Docs");
      }
    }

    static string[] ArrSendTo(List<DocumentReceiver> docRec)
    {
      List<string> list = new List<string>();
      foreach (var item in docRec)
      {
        if (!item.ReceiveOrganizationID.StartsWith("000."))
        {
          continue;
        }
        if (item.ReceiveOrganizationID.Contains(".A44"))
        {
          continue;
        }

        list.Add(item.ReceiveOrganizationID);
        Console.WriteLine("Send to:" + item.ReceiveOrganizationName);
      }

      return list.ToArray();
    }
    public static string EdXmlFile(DocumentSender docSend, string guidFolder)
    {
      string pathFile = "";
      EdXML102.EdXml102 itemXml = new EdXML102.EdXml102();
      //from
      //Thong tin don vi gui
      EdXML102.Header.From fr = new EdXML102.Header.From()
          .WithOrganId(docSend.SendOrganizationID)
          .WithOrganName(docSend.SendOrganizationName)
          .WithOrganizationInCharge(docSend.SendOrganizationName)
          .WithOrganAdd("")
          .WithEmail(docSend.Email)
          .WithTelephone(docSend.Phone)
          .WithFax(docSend.Fax)
          .WithWebsite(docSend.Website);
      itemXml.WithFrom(fr);

      //To
      List<To> listTo = new List<To>();
      var listDoc = docSend.DocumentReceivers.Where(p => !p.ReceiveOrganizationID.Contains(".A44")); //TODO: lấy tổ chức k nằm trong A44
      Console.WriteLine("listDoc TO:" + listDoc.Count());
      if (listDoc.Any())
      {
        foreach (var item in listDoc)
        {
          listTo.Add(
      new To()
      .WithOrganId(item.ReceiveOrganizationID)
      .WithOrganName(item.ReceiveOrganizationName)
      .WithOrganizationInCharge("")
      .WithOrganAdd("")
      .WithEmail(item.Email)
      .WithTelephone(item.Phone)
      .WithFax(item.Fax)
      .WithWebsite(item.Website));
        }
      }
      itemXml.WithToList(listTo.ToArray());

      string temNumberDoc = docSend.NotationNumber;
      Console.WriteLine("temNumberDoc:" + temNumberDoc);
      var temSplit = temNumberDoc.Split('/');
      string codeNum = "";
      string codeNotation = "";
      if (temSplit != null && temSplit.Length > 0)
      {
        Console.WriteLine("temSplit leng:" + temSplit.Length);
        codeNum = temSplit[0] ?? "";
        if (temSplit.Length > 1)
        {
          codeNotation = temSplit[1] ?? "";
        }
      }


      Console.WriteLine("set Code:");
      Code code = new Code()
      {


        Number = codeNum,
        Notation = codeNotation
      };
      itemXml.WithCode(code);

      //Thong tin id van ban
      itemXml.WithDocumentId(new DocumentId()
      {
        Value = docSend.SendOrganizationID + "," + docSend.DocumentDate?.ToString("yyyy/MM/dd") + "," + docSend.NotationNumber
      });

      //Thong tin ban hanh van ban
      itemXml.Promulgation = new Promulgation()
          .WithPlace(docSend.SendOrganizationName)
          .WithDate(docSend.DocumentDate.Value);

      //Thong tin loai van ban
      ////itemXml.Document = new Document()
      ////    .WithType("18")
      ////    .WithName("Công văn");

      //Thong tin trich yeu noi dung van ban
      itemXml.Subject.Value = docSend.Title;

      //Thong tin loai chi dao
      itemXml.SteeringType = 0;

      //Thong tin noi dung van ban can ban hanh
      itemXml.Content.Value = docSend.Title;

      //Thong tin nguoi ki
      itemXml.SignerInfo = new SignerInfo()
          .WithCompetence(docSend.Signers) ///TODO: need add code
                .WithPosition("")//PHÓ CHỦ NHIỆM
          .WithFullName(docSend.Signers);

      //Thong tin han tra loi van ban
      if (docSend.DueDate != null)
      {
        itemXml.DueDate.Value = docSend.DueDate.Value;
      }

      //Thong tin danh sach noi nhan va luu van ban
      List<string> lstToPlaces = new List<string>();
      if (listDoc.Any())
      {
        foreach (var item in listDoc)
        {
          lstToPlaces.Add(item.ReceiveOrganizationName);
        }
      }
      itemXml.ToPlaces = new ToPlaces()
      .WithPlace(lstToPlaces.ToArray());


      //get file to folder path from DB sender attachments file
      List<string> atts = SaveFileToFolder(guidFolder, docSend.DocumentAttachments.ToList());
      //atts.Add(@"Banthuyetminhvehienvat.pdf");
      // send with edxml

      string guid = System.Guid.NewGuid().ToString();
      Console.WriteLine("writeEdXML:" + guid);
      pathFile = ultils.writeEdXML(true, atts.ToArray(), true, 0, guidFolder, guid, itemXml);
      Console.WriteLine("Return pathFile:" + pathFile);
      return pathFile;
    }
    public static string EdXmlFileOutRevoke(DocumentSender docSend, string guidFolder)
    {
      string pathFile = "";
      EdXML102.EdXml102 itemXml = new EdXML102.EdXml102();
      //from
      //Thong tin don vi gui
      EdXML102.Header.From fr = new EdXML102.Header.From()
          .WithOrganId(docSend.SendOrganizationID)
          .WithOrganName(docSend.SendOrganizationName)
          .WithOrganizationInCharge(docSend.SendOrganizationName)
          .WithOrganAdd("")
          .WithEmail(docSend.Email)
          .WithTelephone(docSend.Phone)
          .WithFax(docSend.Fax)
          .WithWebsite(docSend.Website);
      itemXml.WithFrom(fr);

      //To
      List<To> listTo = new List<To>();
      var listDoc = docSend.DocumentReceivers.Where(p => !p.ReceiveOrganizationID.Contains(".A44")); //TODO: lấy tổ chức k nằm trong A44
      Console.WriteLine("listDoc TO:" + listDoc.Count());
      if (listDoc.Any())
      {
        foreach (var item in listDoc)
        {
          listTo.Add(
      new To()
      .WithOrganId(item.ReceiveOrganizationID)
      .WithOrganName(item.ReceiveOrganizationName)
      .WithOrganizationInCharge("")
      .WithOrganAdd("")
      .WithEmail(item.Email)
      .WithTelephone(item.Phone)
      .WithFax(item.Fax)
      .WithWebsite(item.Website));
        }
      }
      itemXml.WithToList(listTo.ToArray());

      string temNumberDoc = docSend.NotationNumber;
      Console.WriteLine("temNumberDoc:" + temNumberDoc);
      var temSplit = temNumberDoc.Split('/');
      string codeNum = "";
      string codeNotation = "";
      if (temSplit != null && temSplit.Length > 0)
      {
        Console.WriteLine("temSplit leng:" + temSplit.Length);
        codeNum = temSplit[0] ?? "";
        if (temSplit.Length > 1)
        {
          codeNotation = temSplit[1] ?? "";
        }
      }


      Console.WriteLine("set Code:");
      Code code = new Code()
      {


        Number = codeNum,
        Notation = codeNotation
      };
      itemXml.WithCode(code);

      //Thong tin id van ban
      itemXml.WithDocumentId(new DocumentId()
      {
        Value = docSend.SendOrganizationID + "," + docSend.DocumentDate?.ToString("yyyy/MM/dd") + "," + docSend.NotationNumber
      });

      //Thong tin ban hanh van ban
      itemXml.Promulgation = new Promulgation()
          .WithPlace(docSend.SendOrganizationName)
          .WithDate(docSend.DocumentDate.Value);

      //Thong tin loai van ban
      ////itemXml.Document = new Document()
      ////    .WithType("18")
      ////    .WithName("Công văn");

      //Thong tin trich yeu noi dung van ban
      itemXml.Subject.Value = docSend.Title;

      //Thong tin loai chi dao
      itemXml.SteeringType = 0;

      //Thong tin noi dung van ban can ban hanh
      itemXml.Content.Value = docSend.Title;

      //Thong tin nguoi ki
      itemXml.SignerInfo = new SignerInfo()
          .WithCompetence(docSend.Signers) ///TODO: need add code
                .WithPosition("")//PHÓ CHỦ NHIỆM
          .WithFullName(docSend.Signers);

      //Thong tin han tra loi van ban
      if (docSend.DueDate != null)
      {
        itemXml.DueDate.Value = docSend.DueDate.Value;
      }

      //Thong tin danh sach noi nhan va luu van ban
      List<string> lstToPlaces = new List<string>();
      if (listDoc.Any())
      {
        foreach (var item in listDoc)
        {
          lstToPlaces.Add(item.ReceiveOrganizationName);
        }
      }
      itemXml.ToPlaces = new ToPlaces()
      .WithPlace(lstToPlaces.ToArray());


      //get file to folder path from DB sender attachments file
      List<string> atts = SaveFileToFolder(guidFolder, docSend.DocumentAttachments.ToList());
      //atts.Add(@"Banthuyetminhvehienvat.pdf");
      // send with edxml

      string guid = System.Guid.NewGuid().ToString();
      Console.WriteLine("writeEdXML:" + guid);
      pathFile = ultils.writeEdXML(true, atts.ToArray(), true, 1, guidFolder, guid, itemXml);
      Console.WriteLine("Return pathFile:" + pathFile);
      return pathFile;
    }
    private static List<string> SaveFileToFolder(string saveDirectory, List<DocumentAttachment> documentAttachments)
    {
      List<string> paths = new List<string>();
      foreach (var a in documentAttachments)
      {
        var binaryStr = a.Attachment.ToString();
        var bytes = a.Attachment.ToArray();
        string path = saveDirectory + @"\" + a.FileName;
        File.WriteAllBytes(path, bytes);
        paths.Add(path);
      }
      return paths;
    }

    static void GetAgencyList(DocumentService service)
    {
      try
      {
        //Lay danh sach don vi
        GetAgenciesListResponse res = service.getAgenciesList();
        Console.WriteLine(res.ToString());
        foreach (var item in res.data)
        {
          Console.WriteLine(item.ToString());
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    static void GetListUnit(DocumentService service)
    {
      GetListUnitResponse res = service.getListUnit();
      Console.WriteLine(res.ToString());
      foreach (var item in res.data)
      {
        Console.WriteLine(item.ToString());
      }
    }

    static void RegisterAgency(DocumentService service)
    {
      CASDataContext ctca = new CASDataContext();

      List<Models.DTExportDV> exp = new List<Models.DTExportDV>();

      //List<CASDataContext.>

      //RegisterAgencyRequest req = new RegisterAgencyRequest()
      //{
      //    code = "001.00.41.W00",
      //    pcode = "5b864a15e4b03a0fe7d685b0",
      //    name = "Đơn vị test (Cap 1 M01020918)",
      //    mail = "test@example.com"
      //};

      //RegisterAgencyRequest req = new RegisterAgencyRequest()
      //{
      //    code = "001.03.41.W00",
      //    pcode = "001.00.41.W00",
      //    name = "Đơn vị test (Cap 2 M03020918)",
      //    mail = "test@example.com"
      //};

      //RegisterAgencyRequest req = new RegisterAgencyRequest()
      //{
      //    code = "002.03.41.W00",
      //    pcode = "001.03.41.W00",
      //    name = "Đơn vị test (Cap 3 M01020918)",
      //    mail = "test@example.com"
      //};

      //Tao doi tuong dang ki don vi
      RegisterAgencyRequest req = new RegisterAgencyRequest()
      {
        code = "000.46.24.H44", //ma dinh danh
        pcode = "000.00.24.H44", //ma dinh danh don vi cha
        name = "UBND xã Tiên Lương", //ten don vi
        mail = "test@example.com" //email
      };
      //Tao doi tuong json request
      jsonHeaderInfo header = new jsonHeaderInfo();
      try
      {
        //Dang ki don vi
        RegisterAgencyResponse res = service.registerAgency(header.getJson(), req);
        Console.WriteLine(res.ToString());
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }

    }

    static void DeleteAgency(DocumentService service)
    {
      //Tao doi tuong json request
      jsonHeaderInfo req = new jsonHeaderInfo();
      req.Add("AgencyCode", "000.46.24.H44"); //dinh danh don vi van huy dang ky
      try
      {
        //Huy dang ky don vi lien thong
        DeleteAgencyResponse res = service.deleteAgency(req.getJson());
        Console.WriteLine(res.ToString());
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }

    }

    static List<string> GetReceivedEdocList(DocumentService service, string messagetype)
    {
      List<string> ret = new List<string>();
      //Tao doi tuong json request
      jsonHeaderInfo req = new jsonHeaderInfo();
      if (messagetype == eDocType.ElectronicDocument)
      {
        req.Add("messagetype", eDocType.ElectronicDocument);
      }
      else
      {
        req.Add("messagetype", eDocType.StatusDocument);
      }
      //loai goi tin edoc/status
      //req.Add("fromDate", "01/01/2020 00:00:00"); //tu ngay
      //req.Add("toDate", "29/02/2022 00:00:00"); //den ngay
      try
      {
        //Lay danh sach goi tin gui trong khoang thoi gian
        GetReceivedEdocListResponse res = service.getReceivedEdocList(req.getJson());

        Console.WriteLine(res.ToString());
        foreach (var item in res.data)
        {
          ret.Add(item.DocId);
          Console.WriteLine(item.ToString());
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      return ret;
    }


    static string SendEdoc(DocumentService service, string from, string[] to, string filePath, string type)
    {
      //Tao doi tuong json request
      jsonHeaderInfo req = new jsonHeaderInfo();

      if (type == "status")
      {
        req.Add("messagetype", eDocType.StatusDocument); //loai goi tin edoc/status
        req.Add("from", from); //don vi gui 
      }
      else
      {
        req.Add("messagetype", eDocType.ElectronicDocument); //loai goi tin edoc/status
        req.Add("servicetype", "edoc");
        req.Add("from", from); //don vi gui
        req.Add("to", Util.getHeaderTo(to)); //danh sach don vi nhan 
      }

      try
      {
        //Gui goi tin edoc
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        SendEdocResponse res = service.sendEdoc(req.getJson(), filePath);
        Console.WriteLine(res.ToString());
        if (res.status == "FAIL")
        {
          Exception exx = new Exception(res.ToString());
          throw exx;
        }
        return res.DocId;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw e;
        //return "";
      }
    }

    static string GetEdoc(DocumentService service, string docId)
    {
      string ret = "";
      string pathFolder = StorePathDirUnit + @"/" + docId;
      System.IO.Directory.CreateDirectory(pathFolder);
      //Tao doi tuong json request
      jsonHeaderInfo req = new jsonHeaderInfo();
      req.Add("docId", docId); //ma van ban nhan
      req.Add("filePath", pathFolder); //Duong dan luu file tai ve
      try
      {
        //Lay goi tin van ban
        GetEdocResponse res = service.getEdoc(req.getJson());
        //
        ret = pathFolder + @"/" + docId + ".edxml";
        Console.WriteLine(res.ToString());
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return ret;
    }

    static void UpdateStatus(DocumentService service, string docId, string status)
    {
      //Tao doi tuong json request
      jsonHeaderInfo req = new jsonHeaderInfo();
      req.Add("docId", docId); //ma van ban nhan
      req.Add("status", status); //trang thai (done, fail, processing)
      try
      {
        //Gui trang thai goi tin van ban
        UpdateStatusResponse res = service.updateStatus(req.getJson());
        Console.WriteLine(res.ToString());
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    /// <summary>
    /// Check ESB.DocumentStatus, nếu chưa có record nào cùng Status thì thêm
    /// </summary>
    static void CheckESBDocumentStatus(ESBDTDataContext ct, DocumentReceiver receiver, EnumStatusUpdate status)
    {
      var docStatus = ct.DocumentStatus.Where(x => x.DocumentSendID == receiver.DocumentSendID && x.Status == receiver.Status && x.IsGet == false)?.OrderByDescending(x => x.ID)?.FirstOrDefault();
      if (docStatus != null)
      {
        //docStatus.Note = notationFull;
      }
      else
      {
        int StatStatus = 0;
        switch (status)
        {
          case EnumStatusUpdate.inbox:
            StatStatus = 1; // 1 = "Đã tiếp nhận";
                            //receiver.Comment = "Đã gửi thành công đến trục LGSP";
            break;
          case EnumStatusUpdate.finish:
            StatStatus = 3; // 3 = "Hoàn thành";
            break;
          case EnumStatusUpdate.acceptance:
            StatStatus = 1; // 1 = "Đã tiếp nhận";//receiver.Comment = "Đã gửi thành công đến trục LGSP";
            break;
          case EnumStatusUpdate.assignment:
            StatStatus = 2; //2 = "Đang xử lý"; //receiver.Comment = "Đã gửi thành công đến trục LGSP";
            break;
          case EnumStatusUpdate.processing:
            StatStatus = 3; // 3 = "Hoàn thành";
            break;
          case EnumStatusUpdate.reject:
            StatStatus = 5; // 5 = "Trả lại";
            break;
          default:
            break;
        }
        var newDocStatus = new DocumentStatus
        {
          OrganizationID = receiver.SendOrganizationID,
          EmployeeID = String.IsNullOrEmpty(receiver.EmployeeID) ? "SystemAccount" : receiver.EmployeeID,
          EmployeeName = String.IsNullOrEmpty(receiver.EmployeeName) ? "Hệ thống" : receiver.EmployeeName,
          DepartmentID = String.IsNullOrEmpty(receiver.DepartmentID) ? "TLH" : receiver.DepartmentID,
          DepartmentName = String.IsNullOrEmpty(receiver.DepartmentName) ? "Trục liên thông" : receiver.DepartmentName,
          Date = DateTime.Now,
          Status = StatStatus,
          IsGet = false,
          Key = receiver.Key,
          Type = "VB_DI",
          AccessKey = receiver.AccessKey,
          SecretkeyOther = receiver.SecretkeyOther,
          DocumentSendID = receiver.DocumentSendID,
          CreatedDate = DateTime.Now,
          ModifiedDate = DateTime.Now,
          CreatedBy = receiver.EmployeeName,
          ModifiedBy = receiver.EmployeeName,
          Note = receiver.Comment
        };
        ct.DocumentStatus.InsertOnSubmit(newDocStatus);
        Console.WriteLine("Da cap nhat trang thai VB DocumentStatus DocumentSendID: " + receiver.DocumentSendID + " Don vi gui trong receiver.Comment:" + receiver.Comment);
      }
    }

    static string StatusPackageName(string statusCode)
    {
      switch (statusCode)
      {
        //Da den
        case "01": return "status_inbox_01.edxml";
        //Tu choi
        case "02": return "status_rejection_02.edxml";
        //Tiep nhan
        case "03": return "status_acceptance_03.edxml";
        //Phan con xu ly
        case "04": return "status_assignment_04.edxml";
        //Dang xu ly
        case "05": return "status_processing_05.edxml";
        //Hoan thanh
        case "06": return "status_finish_06.edxml";
        //Lay lai
        case "13": return "status_cancellation_13.edxml";
        //Dong y cap nhat lay lai/thu hoi
        case "15": return "status_change-acceptance_15.edxml";
        //Tu choi cap nhat lay lai/thu hoi
        case "16": return "status_change-rejection_16.edxml";
        //
        default: return "status.edxml";
      }
    }

    /// <summary>
    /// Tạo gói EdmxlStatus102
    /// </summary>
    public static string EdXmlStatus102File(DocumentSender docSend, string guidFolder, string statusCode, string desc = "Cập nhật trạng thái")
    {
      EdXmlStatus102 statusXml = new EdXmlStatus102();
      //From
      statusXml.WithFrom(new EdStatus.Header.From()
      {
        OrganId = docSend.SendOrganizationID,
        OrganName = docSend.SendOrganizationName,
        OrganizationInCharge = docSend.SendOrganizationName,
        OrganAdd = "",
        Email = docSend.Email,
        Telephone = docSend.Phone,
        Fax = docSend.Fax,
        Website = docSend.Website
      });
      //ResponseFor
      statusXml.WithResponseFor(new EdStatus.Header.ResponseFor
      {
        OrganId = docSend.SendOrganizationID,
        DocumentId = $"{docSend.SendOrganizationID},{docSend.DocumentDate.Value.ToString("yyyy-MM-dd")},{docSend.NotationNumber}",
        Code = docSend.NotationNumber,
        PromulgationDate = docSend.DocumentDate.Value,
      });
      statusXml.StatusCode = statusCode;
      statusXml.WithDescription(desc);
      statusXml.WithTimestamp(DateTime.Now);

      //Tạo file edxml status
      string guid = System.Guid.NewGuid().ToString();
      Console.WriteLine("writeEdXML:" + guid);
      string filePath = "";
      EdStatusInfo statusXmlInfo = statusXml.ToFile(@".\" + guid + StatusPackageName(statusCode));
      filePath = statusXmlInfo.FilePath;
      statusXml.Dispose();
      Console.WriteLine("Return filePath: " + filePath);
      return filePath;
    }

    /// <summary>
    /// Thu hồi văn bản đi
    /// </summary>
    private static void OutDocsRevoke(string SystemId, string SecretKey)
    {
      ClientConf conf = new ClientConf(true);
      ESBDTDataContext ct = new ESBDTDataContext();
      CASDataContext ctCas = new CASDataContext();

      string idSettingOrg = SystemId.ToLower();
      if (idSettingOrg == "02.44.W00")
      {
        idSettingOrg = ".a44";
      }
      Console.WriteLine("Send Org:" + SystemId);
      var listOrgA44 = ctCas.ESOrganizations.Where(p => p.OrgID.ToLower().Contains(idSettingOrg)).Select(c => c.OrgID).ToList();

      if (idSettingOrg != "02.44.W00")
      {
        listOrgA44 = listOrgA44.Where(p => p.ToLower().Equals(idSettingOrg)).ToList();
      }
      Console.WriteLine("list Org A44:" + listOrgA44.Count + " " + listOrgA44.FirstOrDefault());
      var docListRevoke = ct.DocumentReceivers.Where(p => listOrgA44.Contains(p.SendOrganizationID) && (p.Status == 4 && p.IsLgsp == true && p.CreatedDate.Value.Year >= 2023)).ToList();
      if (docListRevoke.Any())
      {
        Console.WriteLine($"Văn bản yêu cầu thu hồi: {docListRevoke.Count()}");
        foreach (var doc in docListRevoke)
        {
          var docSend = ct.DocumentSenders.FirstOrDefault(x => x.ID == doc.SenderID);
          Console.WriteLine($"[{doc.DocumentLgspGuidID}] {docSend.Title}");
          var orgLgsp = ctCas.ESOrganizations.Where(p => p.IsLgsp == true);
          if (!orgLgsp.Any()) continue;

          conf.SystemId = SystemId;
          conf.SecretKey = SecretKey;
          VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
          vpcp.type = ServiceType.eDoc; //create DocumentService

          //Tao doi tuong service loai DocumentService
          DocumentService service = (DocumentService)vpcp.CreateService(conf);

          string guid = System.Guid.NewGuid().ToString();
          string folder = StorePathDirUnit;
          string guidFolder = folder + "/" + guid;
          System.IO.Directory.CreateDirectory(guidFolder);

          string[] arrSendTos = ArrSendTo(new List<DocumentReceiver>() { doc });
          Console.WriteLine("  - Build edxml:");
          //string filePathEdxml = EdXmlFileOutRevoke(docSend, guidFolder);
          string filePathEdxml = LgspXRoadSdk.ultils.writeStatus(folder, doc.DocumentSender, doc, guid, "13");
          // Build package edxml to send 

          Console.WriteLine("  - filePathEdxml:" + filePathEdxml);
          string docGuid = "";
          try
          {
            // Gửi eDoc với BussinessDocType = 1 = Thu hồi
            docGuid = SendEdoc(service, docSend.SendOrganizationID, arrSendTos, filePathEdxml, TypeStatus);
          }
          catch (Exception ex)
          {
            if (docGuid == "" || docGuid == "0")
            {
              //docSend.DocumentLgspId = docGuid;
              docSend.Status = -1; // Đã gửi yêu cầu thu hồi lên trục, có lỗi trả về

              doc.StatusLgsp = -1; // Đã gửi lên trục lỗi
              doc.Comment = ex.Message; // "Có lỗi khi gửi yêu cầu thu hồi trục LGSP"; //ex.Message;

              ct.SubmitChanges();
            }
          }

          Console.WriteLine("  - SendEdoc done:" + docGuid);
          if (docGuid != "" && docGuid != "0")
          {
            docSend.DocumentLgspId = docGuid;
            docSend.Status = 13; // Đã gửi yêu cầu thu hồi lên trục thành công

            doc.StatusLgsp = 13; // Đã gửi yêu cầu thu hồi lên trục thành công
            doc.Status = 13; //Tam dat trung voi trang thai truc LGSP; chua dinh nghia trang thai DA GUI GOI TIN THU HOI - (Cho dau nhan dong y thu hoi)

            ct.SubmitChanges();
          }
          else
          {

          }

          try
          {
            DeleteDirectory(guidFolder);
            Console.WriteLine("Delete folder done:" + guidFolder);
          }
          catch (Exception ex)
          {
            throw ex;
          }
        }
      }
      else
      {
        Console.WriteLine("Không có văn bản yêu cầu thu hồi nào");
      }
    }

    /// <summary>
    /// Phản hồi các yêu cầu thu hồi văn bản
    /// </summary>
    private static void InDocsRevoke(string sysid, string key)
    {
      ClientConf conf = new ClientConf(true);
      conf.SystemId = sysid;
      conf.SecretKey = key;
      VPCPXRoadSdkClient vpcp = new VPCPXRoadSdkClient();
      vpcp.type = ServiceType.eDoc; //create DocumentService

      //Tao doi tuong service loai DocumentService
      DocumentService service = (DocumentService)vpcp.CreateService(conf);

      // Các văn bản đến được đơn vị trên LGSP yêu cầu thu hồi
      // "StatusCode = 13 = Lấy lại - Đơn vị gửi lấy lại văn bản điện tử đã ban hành"
      // Do local chưa có nghiệp vụ này nên quy ước ESB Status = 15 là đồng ý, 16 là từ chối (giống của LGSP)
      // TODO: Hàm update status Status văn bản đến
      BCSDDataContext bcsd = new BCSDDataContext();
      var documents = bcsd.TblDocuments.Where(m => m.StatusCode == 12).ToList();
      #region listOrgA44
      string idSettingOrg = sysid.ToLower();
      if (idSettingOrg == "02.44.W00")
      {
        idSettingOrg = ".a44";
      }
      Console.WriteLine("Recei Org:" + sysid);
      var listOrgA44 = ctCas.ESOrganizations.Where(p => p.OrgID.ToLower().Contains(idSettingOrg)).Select(c => c.OrgID).ToList();

      if (idSettingOrg != "02.44.W00")
      {
        listOrgA44 = listOrgA44.Where(p => p.ToLower().Equals(idSettingOrg)).ToList();
      }
      Console.WriteLine("list Org A44:" + documents.Count + " " + listOrgA44.FirstOrDefault());
      #endregion listOrgA44
      var docListRevoke = ct.DocumentReceivers.Where(p => listOrgA44.Contains(p.SendOrganizationID) && (p.IsLgsp == true && p.StatusLgsp == 13 && p.CreatedDate.Value.Year >= 2023)).ToList();
      if (docListRevoke.Any())
      {
        Console.WriteLine($"Văn bản từ trục LGSP được yêu cầu thu hồi: {docListRevoke.Count()}");
        // Trả status đồng ý thu hồi
        var docListRevoke_Accept = docListRevoke.Where(x => x.Status == 15)?.ToList() ?? new List<DocumentReceiver>();
        foreach (var doc in docListRevoke_Accept)
        {
          SendStatusEdoc(doc, "15");
        }
        // Trả status từ chối thu hồi
        var docListRevoke_Reject = docListRevoke.Where(x => x.Status == 16)?.ToList() ?? new List<DocumentReceiver>();
        foreach (var doc in docListRevoke_Reject)
        {
          SendStatusEdoc(doc, "16");
        }
      }
      else
      {
        Console.WriteLine("Không có văn bản yêu cầu thu hồi nào");
      }
      #region Xử lý gửi status
      bool SendStatusEdoc(DocumentReceiver doc, string statusCode)
      {
        var docSend = ct.DocumentSenders.FirstOrDefault(x => x.ID == doc.SenderID);
        Console.WriteLine($"[{doc.DocumentLgspGuidID}] {docSend.Title}");
        var orgLgsp = ctCas.ESOrganizations.Where(p => p.IsLgsp == true);
        if (!orgLgsp.Any()) return false;

        string guidFolder = StorePathDirUnit + "/" + System.Guid.NewGuid().ToString();
        System.IO.Directory.CreateDirectory(guidFolder);

        string[] arrSendTos = ArrSendTo(new List<DocumentReceiver>() { doc });
        Console.WriteLine("  - Build edxml:");
        var desc = "";
        switch (statusCode)
        {
          case "15": desc = "Văn thư đồng ý yêu cầu thu hồi văn bản"; break;
          case "16": desc = "Văn thư từ chối yêu cầu thu hồi văn bản"; break;
          default: desc = "Cập nhật trạng thái văn bản"; break;
        }
        var filePathEdxml = EdXmlStatus102File(docSend, guidFolder, statusCode, desc);
        Console.WriteLine("  - filePathEdxml:" + filePathEdxml);
        string docGuid = "";
        try
        {
          // Gửi eDoc với BussinessDocType = 1 = Thu hồi
          docGuid = SendEdoc(service, docSend.SendOrganizationID, arrSendTos, filePathEdxml, TypeEdoc);
        }
        catch (Exception ex)
        {
          if (docGuid == "" || docGuid == "0")
          {
            //docSend.DocumentLgspId = docGuid;
            docSend.Status = -1; // Đã gửi yêu cầu thu hồi lên trục, có lỗi trả về
            doc.StatusLgsp = -1; // Đã gửi lên trục lỗi
            doc.Comment = ex.Message; // "Có lỗi khi gửi yêu cầu thu hồi trục LGSP"; //ex.Message;
            ct.SubmitChanges();
          }
        }
        Console.WriteLine("  - SendEdoc done:" + docGuid);
        if (docGuid != "" && docGuid != "0")
        {
          docSend.DocumentLgspId = docGuid;
          docSend.Status = 13; // Đã gửi yêu cầu thu hồi lên trục thành công
          doc.StatusLgsp = 13; // Đã gửi yêu cầu thu hồi lên trục thành công
          ct.SubmitChanges();
        }
        else
        {

        }

        try
        {
          DeleteDirectory(guidFolder);
          Console.WriteLine("Delete folder done:" + guidFolder);
        }
        catch (Exception ex)
        {
          throw ex;
        }
        return true;
      }
      #endregion Xử lý gửi status
    }
  }
  public class ExtendedEdXml102 : EdXml102
  {
    // Thuộc tính mới
    public string StatusCode { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
  }
  public enum EnumStatusUpdate
  {
    inbox = 1,   // 1 Đã đến
    reject = 2,    // 2 Không tiếp nhận
    acceptance = 3,      // Đã tiếp nhận
    assignment = 4,        // Phân xử lý
    processing = 5,       // Đang xử lý
    finish = 6       // Hoàn thành

  }
  //ESB.DocumentStatus
  // 0 = "Hệ thống đã gửi";
  // 1 = "Đã tiếp nhận";
  // 2 = "Đang xử lý";
  // 3 = "Hoàn thành";
  // 5 = "Trả lại";
  // 6 = "Thu hồi";
  // Còn lại = "Đang xử lý";

  // Trạng thái nhận hay chưa:
  //=0 Chưa nhận,
  //= 1 Đã nhận thành công,
  // = 2 Yêu cầu Trả lại; 
  //=3 Trả lại thành công.
  //4.=Yêu cầu thu hổi. 
  //5=Thu hồi thành công

  // Truc LGSP ma code trang thai
  //  1 01 Đã đến Phần mềm QLVB đã nhận nhưng
  //văn thư chưa xử lý
  //2 02 Từ chối tiếp
  //nhận
  //Văn thư phát hiện văn bản gửi nhầm,
  //sai sót, không đúng thẩm quyền, từ
  //chối nhận văn bản, kèm theo lý do từ
  //chối
  //3 03 Đã tiếp nhận Văn thư đã nhận trên phần mềm
  //4 04 Phân công Phân công xử lý
  //5 05 Đang xử lý Thực hiện xử lý
  //6 06 Hoàn thành Hoàn thành xử lý văn bản

}
