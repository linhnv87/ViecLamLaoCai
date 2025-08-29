using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Services.DTO
{
    public class BusinessVerificationRequestDTO
    {
        public Guid UserId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string TaxNumber { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string CompanySize { get; set; }
        public string Description { get; set; }
        public IFormFileCollection Documents { get; set; }
    }

    public class BusinessVerificationResponseDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string ReviewerNotes { get; set; }
        public string VerificationCode { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class CompanyInfoDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Address { get; set; } = "";
        public string RepresentativeName { get; set; } = "";
        public string TaxNumber { get; set; } = "";
        public string Website { get; set; } = "";
        public string CompanySize { get; set; } = "";
        public string Industry { get; set; } = "";
        public string Description { get; set; } = "";
        public string Position { get; set; } = "";
        public bool IsVerified { get; set; }
    }
}
