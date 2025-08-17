using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class AspnetUser
    {
        public AspnetUser()
        {
            AspnetPersonalizationPerUsers = new HashSet<AspnetPersonalizationPerUser>();
            TblComments = new HashSet<TblComment>();
            TblDocumentApprovalCreatedByNavigations = new HashSet<TblDocumentApproval>();
            TblDocumentApprovalModifiedByNavigations = new HashSet<TblDocumentApproval>();
            TblDocumentApprovalUsers = new HashSet<TblDocumentApproval>();
            TblDocumentFileCreatedByNavigations = new HashSet<TblDocumentFile>();
            TblDocumentFileModifiedByNavigations = new HashSet<TblDocumentFile>();
            TblStatusCreatedByNavigations = new HashSet<TblStatus>();
            TblStatusModifiedByNavigations = new HashSet<TblStatus>();
            Roles = new HashSet<AspnetRole>();
        }

        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string LoweredUserName { get; set; } = null!;
        public string? MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }

        public virtual AspnetApplication Application { get; set; } = null!;
        public virtual AspnetMembership? AspnetMembership { get; set; }
        public virtual AspnetProfile? AspnetProfile { get; set; }
        public virtual ICollection<AspnetPersonalizationPerUser> AspnetPersonalizationPerUsers { get; set; }
        public virtual ICollection<TblComment> TblComments { get; set; }
        public virtual ICollection<TblDocumentApproval> TblDocumentApprovalCreatedByNavigations { get; set; }
        public virtual ICollection<TblDocumentApproval> TblDocumentApprovalModifiedByNavigations { get; set; }
        public virtual ICollection<TblDocumentApproval> TblDocumentApprovalUsers { get; set; }
        public virtual ICollection<TblDocumentFile> TblDocumentFileCreatedByNavigations { get; set; }
        public virtual ICollection<TblDocumentFile> TblDocumentFileModifiedByNavigations { get; set; }
        public virtual ICollection<TblStatus> TblStatusCreatedByNavigations { get; set; }
        public virtual ICollection<TblStatus> TblStatusModifiedByNavigations { get; set; }

        public virtual ICollection<AspnetRole> Roles { get; set; }
    }
}
