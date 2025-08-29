export interface BusinessApprovalData {
  id: number;
  businessName: string;
  taxCode: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  businessType: string;
  registrationDate: string;
  status: string; // 'pending' | 'approved' | 'rejected' | 'reviewing'
  logo: string;
  documents: DocumentInfo[];
  notes?: string;
  approvalDate?: string;
  approvedBy?: string;
  createdDate?: string;
  modifiedDate?: string;
  website?: string;
  companySize?: string;
  industry?: string;
  description?: string;
  position?: string;
}

export interface DocumentInfo {
  id: number;
  name: string;
  type: string;
  url?: string;
  uploadDate: string;
  verified: boolean;
  fileSize?: number;
  filePath?: string;
}

export interface BusinessApprovalStats {
  pending: number;
  approved: number;
  rejected: number;
  reviewing: number;
  todaySubmissions?: number;
  totalSubmissions?: number;
}

export interface BusinessRegistrationModel {
  businessName: string;
  taxCode: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  businessType: string;
  documents: File[];
}

export interface BusinessApprovalFilter {
  status?: string; // all, pending, reviewing, approved, rejected
  businessType?: string; // all, specific business type
  searchKeyword?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string; // registrationDate, businessName, status
  sortOrder?: string; // asc, desc
}

export interface PaginatedBusinessApprovalResponse {
  data: BusinessApprovalData[];
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApproveBusinessRequest {
  businessId: number;
  approvedBy: string;
  notes?: string;
}

export interface RejectBusinessRequest {
  businessId: number;
  rejectedBy: string;
  reason: string;
}

export interface SetReviewingRequest {
  businessId: number;
  reviewedBy: string;
  notes?: string;
}

export interface BulkApproveRequest {
  businessIds: number[];
  approvedBy: string;
}

export interface BulkRejectRequest {
  businessIds: number[];
  rejectedBy: string;
  reason: string;
}

export interface ExportBusinessApprovalsRequest {
  filter: BusinessApprovalFilter;
  format: string; // excel, pdf
}
