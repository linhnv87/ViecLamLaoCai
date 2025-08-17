using EdXML102;
using EdXML102.Attachment;
using EdXML102.Header;
using EdXML102.Util;
using System;
using System.Collections.Generic;
using System.IO;
using EdStatus;
using EdXML102.Body;
using System.Globalization;

namespace LgspXRoadSdk
{
    class ultils
    {

        public static void readEdXML(int bussinessDocType, Boolean writeAttachment, string guid, string pathFolder)
        {
            string edXmlName = "";
            switch (bussinessDocType)
            {
                //Doc file edoc moi
                case 0: edXmlName = guid + "edoc_new.edxml"; break;
                //Doc file edoc thu hoi
                case 1: edXmlName = guid + "edoc_revocation.edxml"; break;
                //Doc file edoc cap nhat
                case 2: edXmlName = guid + "edoc_update.edxml"; break;
                //Doc file edoc thay the
                case 3: edXmlName = guid + "edoc_replace.edxml"; break;
            }

            EdXml102 EdXml = new EdXml102();
            string path = pathFolder + edXmlName;
            EdXml.FromFile(pathFolder + edXmlName);
            //Thong tin don vi gui
            Console.WriteLine("== MessageHeader ===========================================\n");
            Console.WriteLine("From: ---------");
            Console.WriteLine("  + OrganId: {0}", EdXml.From.OrganId);
            Console.WriteLine("  + OrganizationInCharge: {0}", EdXml.From.OrganizationInCharge);
            Console.WriteLine("  + OrganName: {0}", EdXml.From.OrganName);
            Console.WriteLine("  + OrganAdd: {0}", EdXml.From.OrganAdd);
            Console.WriteLine("  + Telephone: {0}", EdXml.From.Telephone);
            Console.WriteLine("  + Website: {0}", EdXml.From.Website);
            Console.WriteLine("  + Fax: {0}", EdXml.From.Fax);
            Console.WriteLine("  + Email: {0}", EdXml.From.Email);
            Console.WriteLine("\n");
            //Thong tin danh sach don vi nhan
            if (EdXml.ToList != null)
            {
                foreach (To t in EdXml.ToList)
                {
                    if (t != null)
                    {
                        Console.WriteLine("To: ---------");
                        Console.WriteLine("  + OrganId: {0}", t.OrganId);
                        Console.WriteLine("  + OrganName: {0}", t.OrganName);
                        Console.WriteLine("  + OrganAdd: {0}", t.OrganAdd);
                        Console.WriteLine("  + Telephone: {0}", t.Telephone);
                        Console.WriteLine("  + Website: {0}", t.Website);
                        Console.WriteLine("  + Fax: {0}", t.Fax);
                        Console.WriteLine("  + Email: {0}", t.Email);
                        Console.WriteLine("\n");
                    }
                }
            }
            //Thong tin so hieu van ban
            Console.WriteLine("Code: ---------");
            Console.WriteLine(" + Number: {0}", EdXml.Code.Number);
            Console.WriteLine(" + Notation: {0}", EdXml.Code.Notation);
            Console.WriteLine("\n");
            //Thong tin ban hành van ban
            Console.WriteLine("PromulgationInfo: ---------");
            Console.WriteLine(" + Place: {0}", EdXml.Promulgation.Place);
            Console.WriteLine(" + Date: {0}", EdXml.Promulgation.Date.ToString("dd/MM/yyyy"));
            Console.WriteLine("\n");
            //Thong tin loai van ban
            Console.WriteLine("DocumentType: ---------");
            Console.WriteLine(" + Type: {0}", EdXml.Document.Type);
            Console.WriteLine(" + Name: {0}", EdXml.Document.Name);
            Console.WriteLine("\n");
            //Thong tin chi dao
            Console.WriteLine("SteeringType: {0}\n", EdXml.SteeringType);
            Console.WriteLine("Subject: {0}\n", EdXml.Subject.Value);
            Console.WriteLine("Content: {0}\n", EdXml.Content.Value);
            //Thong tin nguoi ky van ban
            Console.WriteLine("SignerInfo: ---------");
            Console.WriteLine(" + Competence: {0}", EdXml.SignerInfo.Competence);
            Console.WriteLine(" + Position: {0}", EdXml.SignerInfo.Position);
            Console.WriteLine(" + FullName: {0}", EdXml.SignerInfo.FullName);
            Console.WriteLine("\n");
            //Thong tin hoi dap
            Console.WriteLine("ResponseFor: ---------");
            foreach (var item in EdXml.ResponseFor)
            {
                Console.WriteLine(" + OrganId: {0}", item.OrganId);
                Console.WriteLine(" + Code: {0}", item.Code);
                Console.WriteLine(" + PromulgationDate: {0}", item.PromulgationDate.ToString("yyyy/MM/dd"));
                Console.WriteLine(" + DocumentId: {0}", item.DocumentId);
                Console.WriteLine("\n");
            }
            //Thong tin han xu ly
            Console.WriteLine("DueDate: {0}\n", EdXml.DueDate.Value.ToString("dd/MM/yyyy"));
            //Thong tin noi gui van ban den
            Console.WriteLine("ToPlaces: ---------");
            if (EdXml.ToPlaces.Places != null)
            {
                foreach (string place in EdXml.ToPlaces.Places)
                {
                    if (!string.IsNullOrEmpty(place))
                    {
                        Console.WriteLine(" + Place: {0}", place);
                    }
                }
            }
            Console.WriteLine("\n");
            //Thong tin them
            Console.WriteLine("OtherInfo: ---------");
            Console.WriteLine(" + Priority: {0}", EdXml.OtherInfo.Priority);
            Console.WriteLine(" + SphereOfPromulgation: {0}", EdXml.OtherInfo.SphereOfPromulgation);
            Console.WriteLine(" + TyperNotation: {0}", EdXml.OtherInfo.TyperNotation);
            Console.WriteLine(" + PromulgationAmount: {0}", EdXml.OtherInfo.PromulgationAmount.ToString());
            Console.WriteLine(" + PageAmount: {0}", EdXml.OtherInfo.PageAmount.ToString());
            Console.WriteLine(" + Appendixes:");
            if (EdXml.OtherInfo.Appendixes != null)
            {
                foreach (string appendixe in EdXml.OtherInfo.Appendixes)
                {
                    if (!string.IsNullOrEmpty(appendixe))
                    {
                        Console.WriteLine("    o Appendixe: {0}", appendixe);
                    }
                }
            }
            Console.WriteLine("\n");

            Console.WriteLine("\n== TraceHeaderList =======================================\n");

            if (EdXml.TraceHeaderList != null)
            {
                //Thong tin luu vet don vi gui
                Console.WriteLine("TraceHeader: ---------");
                foreach (var item in EdXml.TraceHeaderList.TraceHeader)
                {
                    Console.WriteLine("#Timestamp: {0}", item.Timestamp);
                    Console.WriteLine("#OrganId : {0}", item.OrganId);
                    Console.WriteLine("\n");
                }
                //Thong tin nghiep vu
                Console.WriteLine("Business: ---------");
                Console.WriteLine("#Paper: {0}", EdXml.Bussiness.Paper);
                Console.WriteLine("#DocumentId: {0}", EdXml.Bussiness.DocumentId);
                Console.WriteLine("#BussinessDocType: {0}", EdXml.Bussiness.BussinessDocType);
                Console.WriteLine("#BussinessDocReason : {0}", EdXml.Bussiness.BussinessDocReason);
                Console.WriteLine("#Staff: ---------");
                Console.WriteLine(" + Department: {0}", EdXml.Bussiness.StaffInfo.Department);
                Console.WriteLine(" + Staff: {0}", EdXml.Bussiness.StaffInfo.Staff);
                Console.WriteLine(" + Mobile: {0}", EdXml.Bussiness.StaffInfo.Mobile);
                Console.WriteLine(" + Email : {0}", EdXml.Bussiness.StaffInfo.Email);
                Console.WriteLine("\n");
                //Thong tin cap nhat(truong hop goi tin edoc cap nhat)
                Console.WriteLine("UpdateList: ---------");
                if (EdXml.BussinessDocumentInfo.ReceiverList.Receiver.Count > 0)
                {
                    Console.WriteLine("#UpdateDocumentInfo: {0}", EdXml.BussinessDocumentInfo.DocumentInfo);
                    Console.WriteLine("#UpdateDocumentReceiver: {0}", EdXml.BussinessDocumentInfo.DocumentReceiver);
                    Console.WriteLine("\n");
                    foreach (var item in EdXml.BussinessDocumentInfo.ReceiverList.Receiver)
                    {
                        Console.WriteLine(" + UpdateReceiverType: {0}", item.ReceiverType);
                        Console.WriteLine(" + UpdateReceiverTo: {0}", item.OrganId);
                        Console.WriteLine("\n");
                    }
                }
                //Thong tin thay the (truong hop goi tin thay the)
                Console.WriteLine("ReplacementInfoList: ---------");
                foreach (var item in EdXml.ReplacementInfoList.ReplacementInfo)
                {
                    Console.WriteLine("#DocumentId: {0}", item.DocumentId);
                    Console.WriteLine("#OrganIdList: ---------");
                    foreach (var item2 in item.OrganIdList)
                    {
                        Console.WriteLine(" + OrganId: {0}", item2);
                    }
                    Console.WriteLine("\n");
                }

                Console.WriteLine("\n");
            }
            //Thong tin body (mo ta cac fil dinh kem)
            Console.WriteLine("\n== Body =======================================\n");
            if (EdXml.FileReferenceList != null)
            {
                foreach (FileReference fr in EdXml.FileReferenceList)
                {
                    if (fr != null)
                    {
                        Console.WriteLine("Reference: ---------");
                        Console.WriteLine("  + FileName: {0}", fr.FileName);
                        Console.WriteLine("  + Description: {0}", fr.Description);
                        Console.WriteLine("  + AttachmentName: {0}", fr.AttachmentName);
                        Console.WriteLine("\n");
                    }
                }
            }
            //Fil dinh kem
            Console.WriteLine("\n== Attachment =======================================\n");

            foreach (FileAttach fi in EdXml.FileAttachList)
            {
                if (fi != null)
                {
                    Console.WriteLine("FileName: {0}", fi.FileName);
                    Console.WriteLine("FileSize: {0}", fi.FileSize);
                    Console.WriteLine("OriginalName: {0}", fi.OriginalName);
                    Console.WriteLine("FileStream: {0}", fi.FileStream);
                    Console.WriteLine("\n--- ++++ ---\n");
                    if (writeAttachment) fi.WriteFile(fi.FileName);
                }
            }
            EdXml.Dispose();
        }

        public static void readStatus(string statusCode)
        {
            string fileName = "";
            switch (statusCode)
            {
                case "01": fileName = "status_inbox_01.edxml"; break;
                case "02": fileName = "status_rejection_02.edxml"; break;
                case "03": fileName = "status_acceptance_03.edxml"; break;
                case "04": fileName = "status_assignment_04.edxml"; break;
                case "05": fileName = "status_processing_05.edxml"; break;
                case "06": fileName = "status_finish_06.edxml"; break;
                case "13": fileName = "status_cancellation_13.edxml"; break;
                case "15": fileName = "status_change-acceptance_15.edxml"; break;
                case "16": fileName = "status_change-rejection_16.edxml"; break;
            }

            EdXmlStatus102 statusXml = new EdXmlStatus102();
            statusXml.FromFile(@".\" + fileName);

            Console.WriteLine("== Status ===========================================\n");
            //Thong tin hoi dap
            EdStatus.Header.ResponseFor responseFor = statusXml.ResponseFor;
            Console.WriteLine("ResponseFor: ---------");
            Console.WriteLine(" + Code: {0}", responseFor.Code);
            Console.WriteLine(" + OrganId: {0}", responseFor.OrganId);
            Console.WriteLine(" + PromulgationDate: {0}", responseFor.PromulgationDate.ToString("dd/MM/yyyy"));
            Console.WriteLine(" + DocumentId: {0}", responseFor.DocumentId);
            Console.WriteLine("\n");
            //Thon tin don vi gui
            EdStatus.Header.From from = statusXml.From;
            Console.WriteLine("From: ---------");
            Console.WriteLine("  + OrganId: {0}", from.OrganId);
            Console.WriteLine("  + OrganizationInCharge: {0}", from.OrganizationInCharge);
            Console.WriteLine("  + OrganName: {0}", from.OrganName);
            Console.WriteLine("  + OrganAdd: {0}", from.OrganAdd);
            Console.WriteLine("  + Telephone: {0}", from.Telephone);
            Console.WriteLine("  + Website: {0}", from.Website);
            Console.WriteLine("  + Fax: {0}", from.Fax);
            Console.WriteLine("  + Email: {0}", from.Email);
            Console.WriteLine("\n");
            //Thong tin trang thai
            Console.WriteLine("StatusCode: {0}\n", statusXml.StatusCode);
            //Thong tin mo ta trang thai
            Console.WriteLine("Description: {0}\n", statusXml.Description);
            //Thoi gian xu ly
            Console.WriteLine("Timestamp: {0}\n", statusXml.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"));
            //Thong tin nguoi xu ly
            EdStatus.Header.StaffInfo staffInfo = statusXml.StaffInfo;
            Console.WriteLine("StaffInfo: ---------");
            Console.WriteLine(" + Department: {0}", staffInfo.Department);
            Console.WriteLine(" + Staff: {0}", staffInfo.Staff);
            Console.WriteLine(" + Email: {0}", staffInfo.Email);
            Console.WriteLine(" + Mobile: {0}", staffInfo.Mobile);

            statusXml.Dispose();
        }

        public static string writeStatus(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string guid, string statusCode)
        {
            string pathFile = "";
            switch (statusCode)
            {
                //Tao goi tin status da den
                case "01": pathFile = DaDen(pathFolder, objSender, itemRj, guid + "status_inbox_01.edxml"); break;
                //Tao goi tin status tu choi
                case "02": pathFile = TuChoi(pathFolder, objSender, itemRj, guid + "status_rejection_02.edxml"); break;
                //Tao goi tin status tiep nhan
                case "03": pathFile = TiepNhan(pathFolder, objSender, itemRj, guid + "status_acceptance_03.edxml"); break;
                //Tao goi tin status phan con xu ly
                case "04": PhanCong(pathFolder, objSender, itemRj, guid + "status_assignment_04.edxml"); break;
                //Tao goi tin status dang xu ly
                case "05": DangXuLy(pathFolder, objSender, itemRj, guid + "status_processing_05.edxml"); break;
                //Tao goi tin status hoan thanh
                case "06": HoanThanh(pathFolder, objSender, itemRj, guid + "status_finish_06.edxml"); break;
                //Tao goi tin status lay lai
                case "13": pathFile = LayLai(pathFolder, objSender, itemRj, guid + "\\status_cancellation_13.edxml"); break;
                //Tao goi tin status dong y cap nhat lay lai/thu hoi
                case "15": DongYCapNhatLayLai(pathFolder, objSender, itemRj, guid + "status_change-acceptance_15.edxml"); break;
                //Tao goi tin status tu choi cap nhat lay lai/thu hoi
                case "16": TuChoiCapNhatLayLai(pathFolder, objSender, itemRj, guid + "status_change-rejection_16.edxml"); break;
            }
            return pathFile;
        }

        public static string DaDen(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = "000.00.00.H41",
                OrganName = "UBND Tỉnh Nghệ An",
                OrganizationInCharge = "UBND Tỉnh Nghệ An",
                OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                Email = "nghean@gov.vn",
                Telephone = "0383 840418",
                Fax = "0383 843049",
                Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = "7816/VPCP-TTĐT",
                OrganId = "000.00.00.G22",
                PromulgationDate = new DateTime(2018, 9, 26, 10, 12, 10),
                DocumentId = "000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                Department = "Phong hanh chinh",
                Staff = "Nguyen Thi Ngoc Tram",
                Mobile = "84912000002",
                Email = "ngoctram@nghean.vn",
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("01");
            //Thong tin mo ta trang thai
            statusXml.WithDescription("Văn thư - Phòng Văn thư - Lưu trữ: Đã đến - Phần mềm QLVB đã nhận nhưng văn thư chưa xử lý");
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(@".\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
            return "";
        }

        public static string TuChoi(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {

            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = itemRj.ReceiveOrganizationID,
                OrganName = itemRj.ReceiveOrganizationName,
                OrganizationInCharge = itemRj.ReceiveOrganizationName,
                //OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                //Email = "nghean@gov.vn",
                //Telephone = "0383 840418",
                //Fax = "0383 843049",
                //Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = objSender.NotationNumber,
                OrganId = objSender.SendOrganizationID,
                PromulgationDate = objSender.DocumentDate.Value,
                DocumentId = objSender.SendOrganizationID + "," + objSender.DocumentDate.Value.ToString("yyyy/MM/dd") + "," + objSender.NotationNumber, //"000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                // Department = itemRj.ModifiedBy,
                Staff = itemRj.ModifiedBy,//"Nguyen Thi Ngoc Tram",
                //Email = "vanthuvanphong@gov.vn",
                //Mobile = "84912000001"
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("02");
            //Thong tin mo ta trang thai
            statusXml.WithDescription(itemRj.Comment);
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(pathFolder + @"\" + statusXmlName);

            statusXml.Dispose();
            return pathFolder + @"\" + statusXmlName;
        }

        public static string TiepNhan(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = itemRj.ReceiveOrganizationID,
                OrganName = itemRj.ReceiveOrganizationName,
                OrganizationInCharge = itemRj.ReceiveOrganizationName,
                //OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                //Email = "nghean@gov.vn",
                //Telephone = "0383 840418",
                //Fax = "0383 843049",
                //Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = objSender.NotationNumber,
                OrganId = objSender.SendOrganizationID,
                PromulgationDate = objSender.DocumentDate.Value,
                DocumentId = objSender.SendOrganizationID + "," + objSender.DocumentDate.Value.ToString("yyyy/MM/dd") + "," + objSender.NotationNumber, //"000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                // Department = itemRj.ModifiedBy,
                Staff = itemRj.ModifiedBy,//"Nguyen Thi Ngoc Tram",
                //Email = "vanthuvanphong@gov.vn",
                //Mobile = "84912000001"
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("03");
            //Thong tin mo ta trang thai
            statusXml.WithDescription(itemRj.Comment);
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(pathFolder + @"\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
            return pathFolder + @"\" + statusXmlName;
        }

        public static string PhanCong(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = "000.00.00.H41",
                OrganName = "UBND Tỉnh Nghệ An",
                OrganizationInCharge = "UBND Tỉnh Nghệ An",
                OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                Email = "nghean@gov.vn",
                Telephone = "0383 840418",
                Fax = "0383 843049",
                Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = "7816/VPCP-TTĐT",
                OrganId = "000.00.00.G22",
                PromulgationDate = new DateTime(2018, 9, 26, 10, 12, 10),
                DocumentId = "000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                // Department = itemRj.ModifiedBy,
                Staff = itemRj.ModifiedBy,//"Nguyen Thi Ngoc Tram",
                //Email = "vanthuvanphong@gov.vn",
                //Mobile = "84912000001"
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("04");
            //Thong tin mo ta trang thai
            statusXml.WithDescription("Lãnh đạo: Phân công xử lý");
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(pathFolder + @"\" + statusXmlName);

            statusXml.Dispose();
            return pathFolder + @"\" + statusXmlName;
        }

        public static void DangXuLy(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = "000.00.00.H41",
                OrganName = "UBND Tỉnh Nghệ An",
                OrganizationInCharge = "UBND Tỉnh Nghệ An",
                OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                Email = "nghean@gov.vn",
                Telephone = "0383 840418",
                Fax = "0383 843049",
                Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = "7816/VPCP-TTĐT",
                OrganId = "000.00.00.G22",
                PromulgationDate = new DateTime(2018, 9, 26, 10, 12, 10),
                DocumentId = "000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                Department = "Phong hanh chinh",
                Staff = "Nguyen Thi Ngoc Tram",
                Mobile = "84912000002",
                Email = "ngoctram@nghean.vn",
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("05");
            //Thong tin mo ta trang thai
            statusXml.WithDescription("Đang xử lý văn bản đến");
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(@".\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
        }

        public static void HoanThanh(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = "000.00.00.H41",
                OrganName = "UBND Tỉnh Nghệ An",
                OrganizationInCharge = "UBND Tỉnh Nghệ An",
                OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                Email = "nghean@gov.vn",
                Telephone = "0383 840418",
                Fax = "0383 843049",
                Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = "7816/VPCP-TTĐT",
                OrganId = "000.00.00.G22",
                PromulgationDate = new DateTime(2018, 9, 26, 10, 12, 10),
                DocumentId = "000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                Department = "Phong hanh chinh",
                Staff = "Nguyen Thi Ngoc Tram",
                Mobile = "84912000002",
                Email = "ngoctram@nghean.vn",
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("06");
            //Thong tin mo ta trang thai
            statusXml.WithDescription("Văn thư: Ban hành văn bản đến, hoàn thành xử lý");
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(@".\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
        }

        public static string LayLai(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = objSender.SendOrganizationID,
                OrganName = objSender.SendOrganizationName,
                OrganizationInCharge = objSender.SendOrganizationName,
                //Email = "vpcp@gov.vn",
                //Telephone = "8043100",
                //Fax = "04",
                //Website = "vpcp.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = itemRj.ReceiveOrganizationID,
                OrganId = objSender.NotationNumber, //objSender.SendOrganizationID,
                PromulgationDate = objSender.DocumentDate.Value,
                DocumentId = itemRj.ReceiveOrganizationID + "," + objSender.DocumentDate.Value.ToString("yyyy/MM/dd") + "," + objSender.NotationNumber, //"000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"

            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                // Department = itemRj.ModifiedBy,
                Staff = "Văn thư " + objSender.SendOrganizationName,//"Nguyen Thi Ngoc Tram",
                //Email = "vanthuvanphong@gov.vn",
                //Mobile = "84912000001"
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("13");
            //Thong tin mo ta trang thai
            statusXml.WithDescription(itemRj.Comment);
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(pathFolder + @"\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
            return pathFolder + @"\" + statusXmlName;
        }

        public static void DongYCapNhatLayLai(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = "000.00.00.H41",
                OrganName = "UBND Tỉnh Nghệ An",
                OrganizationInCharge = "UBND Tỉnh Nghệ An",
                OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                Email = "nghean@gov.vn",
                Telephone = "0383 840418",
                Fax = "0383 843049",
                Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = "7816/VPCP-TTĐT",
                OrganId = "000.00.00.G22",
                PromulgationDate = new DateTime(2018, 9, 26, 10, 12, 10),
                DocumentId = "000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                Department = "Phong hanh chinh",
                Staff = "Nguyen Thi Ngoc Tram",
                Mobile = "84912000002",
                Email = "ngoctram@nghean.vn",
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("14");
            //Thong tin mo ta trang thai
            statusXml.WithDescription("Văn thư đồng ý yêu cầu cập nhật, lấy lại");
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(@".\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
        }

        public static void TuChoiCapNhatLayLai(string pathFolder, DocumentSender objSender, DocumentReceiver itemRj, string statusXmlName)
        {
            EdXmlStatus102 statusXml = new EdXmlStatus102();
            //Thong tin don vi gui
            statusXml.WithFrom(new EdStatus.Header.From()
            {
                OrganId = "000.00.00.H41",
                OrganName = "UBND Tỉnh Nghệ An",
                OrganizationInCharge = "UBND Tỉnh Nghệ An",
                OrganAdd = "Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam",
                Email = "nghean@gov.vn",
                Telephone = "0383 840418",
                Fax = "0383 843049",
                Website = "www.nghean.vn"
            });
            //Thong tin hoi dap
            statusXml.WithResponseFor(new EdStatus.Header.ResponseFor()
            {
                Code = "7816/VPCP-TTĐT",
                OrganId = "000.00.00.G22",
                PromulgationDate = new DateTime(2018, 9, 26, 10, 12, 10),
                DocumentId = "000.00.00.G22,2018/09/20,7816/VPCP-TTĐT"
            });
            //Thong tin nguoi xu ly
            statusXml.WithStaffInfo(new EdStatus.Header.StaffInfo()
            {
                Department = "Phong hanh chinh",
                Staff = "Nguyen Thi Ngoc Tram",
                Mobile = "84912000001",
                Email = "ngoctram@nghean.vn",
            });
            //Thong tin ma trang thai
            statusXml.WithStatusCode("16");
            //Thong tin mo ta trang thai
            statusXml.WithDescription("Văn thư từ chối yêu cầu cập nhật, lấy lại");
            //Thoi gian xu ly
            statusXml.WithTimestamp(DateTime.Now);
            //Tao file edxml status
            EdStatusInfo statusXmlInfo = statusXml.ToFile(@".\" + statusXmlName);
            Console.WriteLine("== Content ===========================================\n");
            Console.WriteLine("== FilePath: " + statusXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + statusXmlInfo.HashSha256);
            Console.WriteLine("== Content ===========================================\n");

            statusXml.Dispose();
        }

        public static string writeEdXML(bool readAttachment, string[] listFileAttachmentName, bool zipFile, int bussinessDocType, string pathFolder, string guid, EdXml102 outDoc)
        {
            string rt = "";
            switch (bussinessDocType)
            {
                //Tao goi tin edoc moi
                case 0: rt = edoc_new("edoc_new.edxml", readAttachment, listFileAttachmentName, zipFile, pathFolder, guid, outDoc); break;
                //Tao goi tin edoc thu hoi
                case 1: rt = edoc_revocation("edoc_revocation.edxml", readAttachment, listFileAttachmentName, zipFile, pathFolder, guid, outDoc); break;
                //Tao goi tin edoc update
                case 2: edoc_update("edoc_update.edxml", readAttachment, listFileAttachmentName, zipFile); break;
                //Tao goi tin edoc thay the
                case 3: edoc_replace("edoc_replace.edxml", readAttachment, listFileAttachmentName, zipFile); break;
            }
            return rt;
        }

        public static string edoc_new(string edXmlName, bool readAttachment, string[] listFileAttachmentName, bool zipFile, string pathFolder, string guid, EdXml102 outDoc)
        {
            Random rd = new Random();
            EdXml102 edXML1 = new EdXml102();
            edXML1.Protocol = EdXML102.Util.EdXMLEnum.Protocol.HTTP;
            //Thong tin tap tin dinh kem
            if (readAttachment)
            {
                int numFileAttachment = listFileAttachmentName.Length;

                FileAttach[] fas = new FileAttach[numFileAttachment];
                for (int i = 0; i <= numFileAttachment - 1; i++)
                {
                    if (System.IO.File.Exists(listFileAttachmentName[i]))
                    {
                        FileAttach fa = new FileAttach();
                        int Option = 2;


                        String fiPath = listFileAttachmentName[i];
                        String fiDescription = string.Format("File dinh kem {0}", (i + 1).ToString());
                        String fiContentID = Guid.NewGuid().ToString();
                        String folderPath = Path.GetTempPath();

                        if (Option == 1)
                        {
                            //OPTION 1:
                            try
                            {
                                //fa.FileStream = File.
                                //fa.ReadFile(fiPath, fiDescription, fiContentID, folderPath);
                                fas[i] = fa;
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            //OPTION 2:
                            using (FileStream fs = new FileStream(fiPath, FileMode.Open))
                            {
                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                                fa.FileName = fs.Name;
                                fa.FileSize = fs.Length;
                                fa.FileStream = new System.IO.MemoryStream(buffer);
                                fa.OriginalName = fiDescription;
                                fa.ContentId = fiContentID;
                                fs.Close();
                            }
                            FileInfo fi = new FileInfo(fa.FileName);
                            fa.ContentType = EdXMLUtil.MimeTypeFromExtension(fi.Extension);
                            if (zipFile)
                            {
                                try
                                {
                                    fa.FileStream = EdXMLUtil.Compressed(fa, folderPath);
                                }
                                catch
                                {

                                }

                                fa.FileSize = fa.FileStream.Length;
                            }

                            fas[i] = fa;
                        }
                    }
                }
                edXML1.FileAttachList = fas;
            }

            //Thong tin don vi gui
            EdXML102.Header.From fr = new EdXML102.Header.From()
                .WithOrganId(outDoc.From.OrganId)
                .WithOrganName(outDoc.From.OrganName)
                .WithOrganizationInCharge(outDoc.From.OrganizationInCharge)
                .WithOrganAdd(outDoc.From.OrganAdd)
                .WithEmail(outDoc.From.Email)
                .WithTelephone(outDoc.From.Telephone)
                .WithFax(outDoc.From.Fax)
                .WithWebsite(outDoc.From.Website);
            edXML1.WithFrom(fr);

            //Thong tin danh sach don vi nhan
            List<To> listTo = new List<To>();
            foreach (var toItem in outDoc.ToList)
            {
                listTo.Add(
               new To()
               .WithOrganId(toItem.OrganId)
               .WithOrganName(toItem.OrganName)
               .WithOrganizationInCharge(toItem.OrganizationInCharge)
               .WithOrganAdd(toItem.OrganAdd)
               .WithEmail(toItem.Email)
               .WithTelephone(toItem.Telephone)
               .WithFax(toItem.Fax)
               .WithWebsite(toItem.Website));
            }

            edXML1.WithToList(listTo.ToArray());

            //Thong tin so hieu van ban
            Code code = new Code()
            {
                Number = outDoc.Code.Number,
                Notation = outDoc.Code.Notation
            };
            edXML1.WithCode(code);

            //Thong tin id van ban
            edXML1.WithDocumentId(new DocumentId()
            {
                Value = outDoc.DocumentId.Value
            });

            //Thong tin ban hanh van ban
            edXML1.Promulgation = new Promulgation()
                .WithPlace(outDoc.Promulgation.Place)
                .WithDate(outDoc.Promulgation.Date);

            //Thong tin loai van ban
            edXML1.Document = new Document()
                .WithType(outDoc.Document.Type)
                .WithName(outDoc.Document.Name);

            //Thong tin trich yeu noi dung van ban
            edXML1.Subject.Value = outDoc.Subject.Value;

            //Thong tin loai chi dao
            edXML1.SteeringType = outDoc.SteeringType;

            //Thong tin noi dung van ban can ban hanh
            edXML1.Content.Value = outDoc.Content.Value;

            //Thong tin nguoi ki
            edXML1.SignerInfo = new SignerInfo()
                .WithCompetence(outDoc.SignerInfo.Competence)
                .WithPosition(outDoc.SignerInfo.Position)
                .WithFullName(outDoc.SignerInfo.FullName);

            //Thong tin han tra loi van ban
            edXML1.DueDate.Value = outDoc.DueDate.Value;

            //Thong tin danh sach noi nhan va luu van ban
            edXML1.ToPlaces = new ToPlaces()
                .WithPlace(outDoc.ToPlaces.Places);

            //Thong tin them
            edXML1.OtherInfo = new OtherInfo()
                .WithPriority(outDoc.OtherInfo.Priority)
                .WithSphereOfPromulgation(outDoc.OtherInfo.SphereOfPromulgation)
                .WithTyperNotation(outDoc.OtherInfo.TyperNotation)
                .WithPromulgationAmount(outDoc.OtherInfo.PromulgationAmount)
                .WithPageAmount(outDoc.OtherInfo.PageAmount)
                .WithAppendixes(outDoc.OtherInfo.Appendixes);

            //Thong tin luu vet giua ca he thong, co quan 
            List<TraceHeader> traceHeaders = new List<TraceHeader>();
            traceHeaders = outDoc.TraceHeaderList.TraceHeader;
            edXML1.WithTraceHeaderList(new TraceHeaderList().WithTraceHeader(traceHeaders));

            //Thong tin xu ly nghiep vu van ban
            Bussiness bussiness = new Bussiness()
                .WithBussinessDocType(outDoc.Bussiness.BussinessDocType)
                .WithBussinessDocReason(outDoc.Bussiness.BussinessDocReason)
                .WithPaper(outDoc.Bussiness.Paper)
                .WithStaffInfo(new EdXML102.Header.StaffInfo()
                {
                    Department = outDoc.Bussiness.StaffInfo.Department,
                    Staff = outDoc.Bussiness.StaffInfo.Staff,
                    Email = outDoc.Bussiness.StaffInfo.Email,
                    Mobile = outDoc.Bussiness.StaffInfo.Mobile
                }); ;
            edXML1.WithBussiness(bussiness);

            EdXMLInfo edXmlInfo;

            edXmlInfo = edXML1.ToFile(pathFolder + @"\" + edXmlName);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");
            Console.WriteLine("== FilePath: " + edXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + edXmlInfo.HashSha256);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");

            edXML1.Dispose();
            return pathFolder + @"\" + edXmlName;
        }

        public static string edoc_revocation(string edXmlName, bool readAttachment, string[] listFileAttachmentName, bool zipFile, string pathFolder, string guid, EdXml102 outDoc)
        {
            Random rd = new Random();
            EdXml102 edXML1 = new EdXml102();
            edXML1.Protocol = EdXML102.Util.EdXMLEnum.Protocol.HTTP;
            //Thong tin tap tin dinh kem
            if (readAttachment)
            {
                int numFileAttachment = listFileAttachmentName.Length;

                FileAttach[] fas = new FileAttach[numFileAttachment];
                for (int i = 0; i <= numFileAttachment - 1; i++)
                {
                    if (System.IO.File.Exists(listFileAttachmentName[i]))
                    {
                        FileAttach fa = new FileAttach();
                        int Option = 2;


                        String fiPath = listFileAttachmentName[i];
                        String fiDescription = string.Format("File dinh kem {0}", (i + 1).ToString());
                        String fiContentID = Guid.NewGuid().ToString();
                        String folderPath = Path.GetTempPath();

                        if (Option == 1)
                        {
                            //OPTION 1:
                            try
                            {
                                //fa.FileStream = File.
                                //fa.ReadFile(fiPath, fiDescription, fiContentID, folderPath);
                                fas[i] = fa;
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            //OPTION 2:
                            using (FileStream fs = new FileStream(fiPath, FileMode.Open))
                            {
                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                                fa.FileName = fs.Name;
                                fa.FileSize = fs.Length;
                                fa.FileStream = new System.IO.MemoryStream(buffer);
                                fa.OriginalName = fiDescription;
                                fa.ContentId = fiContentID;
                                fs.Close();
                            }
                            FileInfo fi = new FileInfo(fa.FileName);
                            fa.ContentType = EdXMLUtil.MimeTypeFromExtension(fi.Extension);
                            if (zipFile)
                            {
                                try
                                {
                                    fa.FileStream = EdXMLUtil.Compressed(fa, folderPath);
                                }
                                catch
                                {

                                }

                                fa.FileSize = fa.FileStream.Length;
                            }

                            fas[i] = fa;
                        }
                    }
                }
                edXML1.FileAttachList = fas;
            }

            //Thong tin don vi gui
            EdXML102.Header.From fr = new EdXML102.Header.From()
                .WithOrganId(outDoc.From.OrganId)
                .WithOrganName(outDoc.From.OrganName)
                .WithOrganizationInCharge(outDoc.From.OrganizationInCharge)
                .WithOrganAdd(outDoc.From.OrganAdd)
                .WithEmail(outDoc.From.Email)
                .WithTelephone(outDoc.From.Telephone)
                .WithFax(outDoc.From.Fax)
                .WithWebsite(outDoc.From.Website);
            edXML1.WithFrom(fr);

            //Thong tin danh sach don vi nhan
            List<To> listTo = new List<To>();
            foreach (var toItem in outDoc.ToList)
            {
                listTo.Add(
               new To()
               .WithOrganId(toItem.OrganId)
               .WithOrganName(toItem.OrganName)
               .WithOrganizationInCharge(toItem.OrganizationInCharge)
               .WithOrganAdd(toItem.OrganAdd)
               .WithEmail(toItem.Email)
               .WithTelephone(toItem.Telephone)
               .WithFax(toItem.Fax)
               .WithWebsite(toItem.Website));
            }

            edXML1.WithToList(listTo.ToArray());

            //Thong tin so hieu van ban
            Code code = new Code()
            {
                Number = outDoc.Code.Number,
                Notation = outDoc.Code.Notation
            };
            edXML1.WithCode(code);
            //Thong tin id van ban
            edXML1.WithDocumentId(new DocumentId()
            {
                Value = outDoc.DocumentId.Value
            });

            //Thong tin ban hanh van ban
            edXML1.Promulgation = new Promulgation()
                .WithPlace(outDoc.Promulgation.Place)
                .WithDate(outDoc.Promulgation.Date);

            //Thong tin loai van ban
            edXML1.Document = new Document()
                .WithType(outDoc.Document.Type)
                .WithName(outDoc.Document.Name);

            //Thong tin trich yeu noi dung van ban
            edXML1.Subject.Value = outDoc.Subject.Value;

            //Thong tin loai chi dao
            edXML1.SteeringType = outDoc.SteeringType;

            //Thong tin noi dung van ban can ban hanh
            edXML1.Content.Value = outDoc.Content.Value;

            //Thong tin nguoi ki
            edXML1.SignerInfo = new SignerInfo()
                .WithCompetence(outDoc.SignerInfo.Competence)
                .WithPosition(outDoc.SignerInfo.Position)
                .WithFullName(outDoc.SignerInfo.FullName);

            //Thong tin han tra loi van ban
            edXML1.DueDate.Value = outDoc.DueDate.Value;

            //Thong tin danh sach noi nhan va luu van ban
            edXML1.ToPlaces = new ToPlaces()
                .WithPlace(outDoc.ToPlaces.Places);

            //Thong tin them
            edXML1.OtherInfo = new OtherInfo()
                .WithPriority(outDoc.OtherInfo.Priority)
                .WithSphereOfPromulgation(outDoc.OtherInfo.SphereOfPromulgation)
                .WithTyperNotation(outDoc.OtherInfo.TyperNotation)
                .WithPromulgationAmount(outDoc.OtherInfo.PromulgationAmount)
                .WithPageAmount(outDoc.OtherInfo.PageAmount)
                .WithAppendixes(outDoc.OtherInfo.Appendixes);

             

            //Thong tin luu vet giua ca he thong, co quan
            List<TraceHeader> traceHeaders = new List<TraceHeader>();
            traceHeaders = outDoc.TraceHeaderList.TraceHeader;
            edXML1.WithTraceHeaderList(new TraceHeaderList().WithTraceHeader(traceHeaders));

            //Thong tin xu ly nghiep vu van ban
            Bussiness bussiness = new Bussiness()
                .WithBussinessDocType(1)
                .WithBussinessDocReason("Thu hồi văn bản điện tử")
                .WithPaper(0)
                .WithStaffInfo(new EdXML102.Header.StaffInfo()
                {
                    Department = outDoc.Bussiness.StaffInfo.Department,
                    Staff = outDoc.Bussiness.StaffInfo.Staff,
                    Email = outDoc.Bussiness.StaffInfo.Email,
                    Mobile = outDoc.Bussiness.StaffInfo.Mobile
                });
            edXML1.WithBussiness(bussiness);

            EdXMLInfo edXmlInfo;

            edXmlInfo = edXML1.ToFile(pathFolder + @"\" + edXmlName);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");
            Console.WriteLine("== FilePath: " + edXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + edXmlInfo.HashSha256);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");

            edXML1.Dispose();
            return pathFolder + @"\" + edXmlName;
        }

        public static void edoc_update(string edXmlName, bool readAttachment, string[] listFileAttachmentName, bool zipFile)
        {
            Random rd = new Random();
            EdXml102 edXML1 = new EdXml102();
            edXML1.Protocol = EdXML102.Util.EdXMLEnum.Protocol.HTTP;

            //Thong tin tap tin dinh kem
            if (readAttachment)
            {
                int numFileAttachment = listFileAttachmentName.Length;

                FileAttach[] fas = new FileAttach[numFileAttachment];
                for (int i = 0; i <= numFileAttachment - 1; i++)
                {
                    if (System.IO.File.Exists(@".\" + listFileAttachmentName[i]))
                    {
                        FileAttach fa = new FileAttach();
                        int Option = 2;


                        String fiPath = @".\" + listFileAttachmentName[i];
                        String fiDescription = string.Format("File dinh kem {0}", (i + 1).ToString());
                        String fiContentID = Guid.NewGuid().ToString();
                        String folderPath = Path.GetTempPath();

                        if (Option == 1)
                        {
                            //OPTION 1:
                            try
                            {
                                fa.ReadFile(fiPath, fiDescription, fiContentID, folderPath);
                                fas[i] = fa;
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            //OPTION 2:
                            using (FileStream fs = new FileStream(fiPath, FileMode.Open))
                            {
                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                                fa.FileName = fs.Name;
                                fa.FileSize = fs.Length;
                                fa.FileStream = new System.IO.MemoryStream(buffer);
                                fa.OriginalName = fiDescription;
                                fa.ContentId = fiContentID;
                                fs.Close();
                            }
                            FileInfo fi = new FileInfo(fa.FileName);
                            fa.ContentType = EdXMLUtil.MimeTypeFromExtension(fi.Extension);
                            if (zipFile)
                            {
                                try
                                {
                                    fa.FileStream = EdXMLUtil.Compressed(fa, folderPath);
                                }
                                catch
                                {

                                }

                                fa.FileSize = fa.FileStream.Length;
                            }

                            fas[i] = fa;
                        }
                    }
                }
                edXML1.FileAttachList = fas;
            }

            //Thong tin don vi gui
            EdXML102.Header.From fr = new EdXML102.Header.From()
                .WithOrganId("000.00.04.G14")
                .WithOrganName("Trung tâm công nghệ thông tin - Tập đoàn Dầu khí Việt Nam")
                .WithOrganizationInCharge("Tập đoàn Công nghiệp Dầu khí Việt Nam - ERP")
                .WithOrganAdd("Tầng 2, tòa nhà DETECH, số 8 Tôn Thất Thuyết")
                .WithEmail("vanthucucud@erp.gov.vn")
                .WithTelephone("(84-4)37821766")
                .WithFax("(84-4)35378208")
                .WithWebsite("http://www.aita.gov.vn");
            edXML1.WithFrom(fr);

            //Thong tin danh sach don vi nhan
            List<To> listTo = new List<To>();
            listTo.Add(
                new To()
                .WithOrganId("000.00.00.H14")
                .WithOrganName("UBND Tỉnh Cao Bằng")
                .WithOrganizationInCharge("UBND Tỉnh Cao Bằng")
                .WithOrganAdd("Số 011 - Đường Hoàng Đình Giong - Thành Phố Cao Bằng, Tỉnh Cao Bằng")
                .WithEmail("caobang@gov.vn")
                .WithTelephone("02063852139")
                .WithFax("02063852139")
                .WithWebsite("www.caobang.gov.vn"));

            listTo.Add(
                new To()
                .WithOrganId("000.00.00.G17")
                .WithOrganName("Bộ Xây dựng")
                .WithOrganAdd("37 Lê Đại Hành - Hà Nội")
                .WithEmail("bxd@moc.gov.vn")
                .WithTelephone("(84-4) 3821 5137")
                .WithFax("(84-4) 3821 5137")
                .WithWebsite("www.xaydung.gov.vn"));
            edXML1.WithToList(listTo.ToArray());

            //Thong tin so hieu van ban
            Code code = new Code()
            {
                Number = "7816",
                Notation = "VPCP-TTĐT"
            };
            edXML1.WithCode(code);

            //Thong tin id van ban
            edXML1.WithDocumentId(new DocumentId()
            {
                Value = "000.00.00.G22," + DateTime.Now.ToString("yyyy/MM/dd") + ",7816/VPCP-TTĐT"
            });

            //Thong tin ban hanh van ban
            edXML1.Promulgation = new Promulgation()
                .WithPlace("Hà Nội")
                .WithDate(DateTime.Now);

            //Thong tin loai van ban
            edXML1.Document = new Document()
                .WithType("18")
                .WithName("Công văn");

            //Thong tin trich yeu noi dung van ban
            edXML1.Subject.Value = "V/v kết nối, liên thông phần mềm quản lý văn bản";

            //Thong tin loai chi dao
            edXML1.SteeringType = 0;

            //Thong tin noi dung van ban can ban hanh
            edXML1.Content.Value = "V/v kết nối, liên thông phần mềm quản lý văn bản";

            //Thong tin nguoi ki
            edXML1.SignerInfo = new SignerInfo()
                .WithCompetence("KT. BỘ TRƯỞNG, CHỦ NHIỆM")
                .WithPosition("PHÓ CHỦ NHIỆM")
                .WithFullName("Lê Mạnh Hà");

            //Thong tin han tra loi van ban
            edXML1.DueDate.Value = DateTime.Now.AddDays(2);

            //Thong tin danh sach noi nhan va luu van ban
            edXML1.ToPlaces = new ToPlaces()
                .WithPlace(new String[] {
                    "Cao Bằng",
                    "Bộ Xây dựng"
                });

            //Thong tin them
            edXML1.OtherInfo = new OtherInfo()
                .WithPriority(0)
                .WithSphereOfPromulgation("Liên thông văn bản quốc gia")
                .WithTyperNotation("TVC")
                .WithPromulgationAmount(10)
                .WithPageAmount(2)
                .WithAppendixes(new String[] {
                    "Công văn về việc kết nối, liên thông phần mềm quản lý văn bản"
                });

            //Thong tin luu vet giua ca he thong, co quan
            List<TraceHeader> traceHeaders = new List<TraceHeader>();
            traceHeaders.Add(new TraceHeader()
            {
                Timestampvalue = DateTime.Now,
                OrganId = "000.00.00.G22"
            });
            edXML1.WithTraceHeaderList(new TraceHeaderList().WithTraceHeader(traceHeaders));

            //Thong tin xu ly nghiep vu van ban
            Bussiness bussiness = new Bussiness()
                .WithBussinessDocType(2)
                .WithBussinessDocReason("Cập nhật văn bản điện tử")
                .WithDocumentId("000.00.00.G22," + DateTime.Now.ToString("yyyy/MM/dd") + ",7816/VPCP-TTĐT")
                .WithPaper(0)
                .WithStaffInfo(new EdXML102.Header.StaffInfo()
                {
                    Department = "Phong hanh chinh",
                    Staff = "Nguyen Thi Ngoc Tram",
                    Email = "vanthucucud@mic.gov.vn",
                    Mobile = "84912000001"
                });
            edXML1.WithBussiness(bussiness);

            //Thong tin van ban duoc cap nhat
            List<UpdateReceiver> updateReceivers = new List<UpdateReceiver>();
            updateReceivers.Add(new UpdateReceiver()
            {
                ReceiverType = 0,
                OrganId = "000.00.00.H41"
            });

            updateReceivers.Add(new UpdateReceiver()
            {
                ReceiverType = 1,
                OrganId = "000.00.00.H14"
            });

            BussinessDocumentInfo updateList = new BussinessDocumentInfo()
               .WithUpdateDocumentInfo(1)
               .WithUpdateDocumentReceiver(1)
               .WithUpdateListReceiver(new ReceiverList().WithUpdateReceiver(updateReceivers));
            edXML1.WithUpdateList(updateList);

            EdXMLInfo edXmlInfo;

            edXmlInfo = edXML1.ToFile(@".\" + edXmlName);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");
            Console.WriteLine("== FilePath: " + edXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + edXmlInfo.HashSha256);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");

            edXML1.Dispose();
        }

        public static void edoc_replace(string edXmlName, bool readAttachment, string[] listFileAttachmentName, bool zipFile)
        {
            Random rd = new Random();
            EdXml102 edXML1 = new EdXml102();
            edXML1.Protocol = EdXML102.Util.EdXMLEnum.Protocol.HTTP;

            //Thong tin tap tin dinh kem
            if (readAttachment)
            {
                int numFileAttachment = listFileAttachmentName.Length;

                FileAttach[] fas = new FileAttach[numFileAttachment];
                for (int i = 0; i <= numFileAttachment - 1; i++)
                {
                    if (System.IO.File.Exists(@".\" + listFileAttachmentName[i]))
                    {
                        FileAttach fa = new FileAttach();
                        int Option = 2;


                        String fiPath = @".\" + listFileAttachmentName[i];
                        String fiDescription = string.Format("File dinh kem {0}", (i + 1).ToString());
                        String fiContentID = Guid.NewGuid().ToString();
                        String folderPath = Path.GetTempPath();

                        if (Option == 1)
                        {
                            //OPTION 1:
                            try
                            {
                                fa.ReadFile(fiPath, fiDescription, fiContentID, folderPath);
                                fas[i] = fa;
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            //OPTION 2:
                            using (FileStream fs = new FileStream(fiPath, FileMode.Open))
                            {
                                byte[] buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, System.Convert.ToInt32(fs.Length));
                                fa.FileName = fs.Name;
                                fa.FileSize = fs.Length;
                                fa.FileStream = new System.IO.MemoryStream(buffer);
                                fa.OriginalName = fiDescription;
                                fa.ContentId = fiContentID;
                                fs.Close();
                            }
                            FileInfo fi = new FileInfo(fa.FileName);
                            fa.ContentType = EdXMLUtil.MimeTypeFromExtension(fi.Extension);
                            if (zipFile)
                            {
                                try
                                {
                                    fa.FileStream = EdXMLUtil.Compressed(fa, folderPath);
                                }
                                catch
                                {

                                }

                                fa.FileSize = fa.FileStream.Length;
                            }

                            fas[i] = fa;
                        }
                    }
                }
                edXML1.FileAttachList = fas;
            }

            //Thong tin don vi gui
            EdXML102.Header.From fr = new EdXML102.Header.From()
                .WithOrganId("000.00.00.G22")
                .WithOrganName("Văn phòng Chính phủ")
                .WithOrganizationInCharge("Văn phòng Chính phủ")
                .WithOrganAdd("Số 1 Hoàng Hoa Thám, Quận Ba Đình, Hà Nội")
                .WithEmail("vpcp@gov.vn")
                .WithTelephone("8043100")
                .WithFax("04")
                .WithWebsite("vpcp.vn");
            edXML1.WithFrom(fr);

            //Thong tin danh sach don vi nhan
            List<To> listTo = new List<To>();
            listTo.Add(
                new To()
                .WithOrganId("000.00.00.H41")
                .WithOrganName("UBND Tỉnh Nghệ An")
                .WithOrganizationInCharge("UBND Tỉnh Nghệ An")
                .WithOrganAdd("Số 03, đường Trường Thi, Thành phố Vinh, Tỉnh Nghệ An, Việt Nam")
                .WithEmail("nghean@gov.vn")
                .WithTelephone("0383 840418")
                .WithFax("0383 843049")
                .WithWebsite("www.nghean.vn"));

            listTo.Add(
                new To()
                .WithOrganId("000.00.00.H14")
                .WithOrganName("UBND Tỉnh Cao Bằng")
                .WithOrganizationInCharge("UBND Tỉnh Cao Bằng")
                .WithOrganAdd("Số 011 - Đường Hoàng Đình Giong - Thành Phố Cao Bằng, Tỉnh Cao Bằng")
                .WithEmail("caobang@gov.vn")
                .WithTelephone("02063852139")
                .WithFax("02063852139")
                .WithWebsite("www.caobang.gov.vn"));

            edXML1.WithToList(listTo.ToArray());

            //Thong tin so hieu van ban
            Code code = new Code()
            {
                Number = "7816",
                Notation = "VPCP-TTĐT"
            };
            edXML1.WithCode(code);

            //Thong tin id van ban
            edXML1.WithDocumentId(new DocumentId()
            {
                Value = "000.00.00.G22," + DateTime.Now.ToString("yyyy/MM/dd") + ",7816/VPCP-TTĐT"
            });

            //Thong tin ban hanh van ban
            edXML1.Promulgation = new Promulgation()
                .WithPlace("Hà Nội")
                .WithDate(DateTime.Now);

            //Thong tin loai van ban
            edXML1.Document = new Document()
                .WithType("18")
                .WithName("Công văn");

            //Thong tin trich yeu noi dung van ban
            edXML1.Subject.Value = "V/v kết nối, liên thông phần mềm quản lý văn bản";

            //Thong tin loai chi dao
            edXML1.SteeringType = 0;

            //Thong tin noi dung van ban can ban hanh
            edXML1.Content.Value = "V/v kết nối, liên thông phần mềm quản lý văn bản";

            //Thong tin nguoi ki
            edXML1.SignerInfo = new SignerInfo()
                .WithCompetence("KT. BỘ TRƯỞNG, CHỦ NHIỆM")
                .WithPosition("PHÓ CHỦ NHIỆM")
                .WithFullName("Lê Mạnh Hà");

            //Thong tin han tra loi van ban
            edXML1.DueDate.Value = DateTime.Now.AddDays(2);

            //Thong tin danh sach noi nhan va luu van ban
            edXML1.ToPlaces = new ToPlaces()
                .WithPlace(new String[] {
                    "Nghệ An",
                    "Cao Bằng"
                });

            //Thong tin them
            //Thong tin them
            edXML1.OtherInfo = new OtherInfo()
                .WithPriority(0)
                .WithSphereOfPromulgation("Liên thông văn bản quốc gia")
                .WithTyperNotation("TVC")
                .WithPromulgationAmount(10)
                .WithPageAmount(2)
                .WithAppendixes(new String[] {
                    "Công văn về việc kết nối, liên thông phần mềm quản lý văn bản"
                });

            //Thong tin luu vet giua ca he thong, co quan
            List<TraceHeader> traceHeaders = new List<TraceHeader>();
            traceHeaders.Add(new TraceHeader()
            {
                Timestampvalue = DateTime.Now,
                OrganId = "000.00.00.G22"
            });
            edXML1.WithTraceHeaderList(new TraceHeaderList().WithTraceHeader(traceHeaders));

            //Thong tin xu ly nghiep vu van ban
            Bussiness bussiness = new Bussiness()
                .WithBussinessDocType(3)
                .WithBussinessDocReason("Văn bản thay thế")
                .WithPaper(0)
                .WithStaffInfo(new EdXML102.Header.StaffInfo()
                {
                    Department = "Văn thư văn phòng",
                    Staff = "Nguyen Thi Ngoc Tram",
                    Email = "vanthuvanphong@gov.vn",
                    Mobile = "84912000001"
                });
            edXML1.WithBussiness(bussiness);

            //Thong tin van ban bi thay the
            List<ReplacementInfo> replacementInfos = new List<ReplacementInfo>();
            replacementInfos.Add(new ReplacementInfo()
               .WithDocumentId("000.00.00.G22,2018/09/20,7806/VPCP-TTĐT")
               .WithOrganId(new string[] { "000.00.00.H14", "000.00.00.H41" }));

            replacementInfos.Add(new ReplacementInfo()
               .WithDocumentId("000.00.00.G22,2018/08/20,7006/VPCP-TTĐT")
               .WithOrganId(new string[] { "000.00.00.H14", "000.00.00.H41" }));

            edXML1.WithReplacementInfoList(new ReplacementInfoList().WithReplacementInfo(replacementInfos));

            EdXMLInfo edXmlInfo;

            edXmlInfo = edXML1.ToFile(@".\" + edXmlName);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");
            Console.WriteLine("== FilePath: " + edXmlInfo.FilePath);
            Console.WriteLine("== HashSha256: " + edXmlInfo.HashSha256);
            Console.WriteLine("== Content QCVN 102 ===========================================\n");

            edXML1.Dispose();
        }


    }
}
