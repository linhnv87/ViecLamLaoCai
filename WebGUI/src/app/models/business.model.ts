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
  status: 'pending' | 'approved' | 'rejected' | 'reviewing';
  logo: string;
  documents: DocumentInfo[];
  notes?: string;
}

export interface DocumentInfo {
  id: number;
  name: string;
  type: string;
  url: string;
  uploadDate: string;
  verified: boolean;
}

export interface BusinessApprovalStats {
  pending: number;
  approved: number;
  rejected: number;
  reviewing: number;
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
