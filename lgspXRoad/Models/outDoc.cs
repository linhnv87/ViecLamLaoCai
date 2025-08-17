using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class DTExportDV
    {
        public string TEN_DON_VI { get; set; }
        public string TEN_DV_CHA { get; set; }
        public string MA_DINH_DANH { get; set; }
        public string MA_DINH_DANH_CHA { get; set; }

        public string OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string CodeUnit { get; set; }
        public string CodeParentUnit { get; set; }
        public string LinkWeb { get; set; }
        public string Authority { get; set; }
        public string CASUrl { get; set; }
        public string Api { get; set; }
        public string Key { get; set; }
        public string AccessKey { get; set; }
        public string ESBApi { get; set; }
        public string DevonlineUrl { get; set; }
        public string KySoUrl { get; set; }
        public string EIMConnection { get; set; }
        public string DMSConnection { get; set; }
        public string ESBConnection { get; set; }


    }
    public class outDoc
    {
        public string DocumentIdValue { get; set; }
        public string Subject { get; set; }
        public int SteeringType { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public From From { get; set; }
        public DocTo DocTo { get; set; }
        public DocCode Code { get; set; }
        public Promulgation Promulgation { get; set; }
        public Document Document { get; set; }
        public SignerInfo SignerInfo { get; set; }
        public ToPlaces ToPlaces { get; set; }
        public OtherInfo OtherInfo { get; set; }
        public List<TraceHeader> TraceHeader { get; set; }
        public Bussiness Bussiness { get; set; }

    }

    public class From
    {
        public string OrganId { get; set; }
        public string OrganName { get; set; }
        public string OrganizationInCharge { get; set; }
        public string OrganAdd { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
    }
    public class DocTo
    {
        public string OrganId { get; set; }
        public string OrganName { get; set; }
        public string OrganizationInCharge { get; set; }
        public string OrganAdd { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
    }
    public class DocCode
    {
        public string Number { get; set; }
        public string Notation { get; set; }
    }
    public class Promulgation
    {
        public string Place { get; set; }
        public DateTime Date { get; set; }
    }

    //Thong tin loai van ban
    public class Document
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
    public class SignerInfo
    {
        public string Competence { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
    }
    public class ToPlaces
    {
        public string[] Place { get; set; }
    }
    public class OtherInfo
    {
        public int Priority { get; set; }
        public string SphereOfPromulgation { get; set; }
        public string TyperNotation { get; set; }
        public int PromulgationAmount { get; set; }
        public int PageAmount { get; set; }
        public string[] Appendixes { get; set; }
    }
    public class TraceHeader
    {
        public DateTime Timestampvalue { get; set; }
        public string OrganId { get; set; }
    }

    public class Bussiness
    {
        public int BussinessDocType { get; set; }
        public string BussinessDocReason { get; set; }
        public int Paper { get; set; }
        public StaffInfo StaffInfo { get; set; }
    }
    public class StaffInfo
    {
        public string Department { get; set; }
        public string Staff { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }
}