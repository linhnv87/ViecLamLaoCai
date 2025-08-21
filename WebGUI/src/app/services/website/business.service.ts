import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { environment } from '../../../environments/environments';

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

@Injectable({
  providedIn: 'root'
})
export class BusinessService {
  private baseUrl = `${environment.apiUrl}/Business`;

  constructor(private http: HttpClient) {}

  // GET /api/Business/GetAll
  getAll(): Observable<BaseResponseModel<BusinessApprovalData[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<BusinessApprovalData[]>>(apiUrl);
  }

  // GET /api/Business/GetPendingApprovals
  getPendingApprovals(): Observable<BaseResponseModel<BusinessApprovalData[]>> {
    const apiUrl = `${this.baseUrl}/GetPendingApprovals`;
    return this.http.get<BaseResponseModel<BusinessApprovalData[]>>(apiUrl);
  }

  // GET /api/Business/GetById/{id}
  getById(id: number): Observable<BaseResponseModel<BusinessApprovalData>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<BusinessApprovalData>>(apiUrl);
  }

  // POST /api/Business/Create
  create(business: BusinessRegistrationModel): Observable<BaseResponseModel<BusinessApprovalData>> {
    const formData = new FormData();
    formData.append('businessName', business.businessName);
    formData.append('taxCode', business.taxCode);
    formData.append('contactPerson', business.contactPerson);
    formData.append('email', business.email);
    formData.append('phone', business.phone);
    formData.append('address', business.address);
    formData.append('businessType', business.businessType);
    
    business.documents.forEach((file, index) => {
      formData.append('documents', file);
    });

    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<BusinessApprovalData>>(apiUrl, formData);
  }

  // PUT /api/Business/Update/{id}
  update(id: number, business: BusinessApprovalData): Observable<BaseResponseModel<BusinessApprovalData>> {
    const apiUrl = `${this.baseUrl}/Update/${id}`;
    return this.http.put<BaseResponseModel<BusinessApprovalData>>(apiUrl, business);
  }

  // PUT /api/Business/Approve/{id}
  approve(id: number, approvedBy: string, notes?: string): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/Approve/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { approvedBy, notes });
  }

  // PUT /api/Business/Reject/{id}
  reject(id: number, rejectedBy: string, reason: string): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/Reject/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { rejectedBy, reason });
  }

  // PUT /api/Business/SetReviewing/{id}
  setReviewing(id: number, reviewedBy: string): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/SetReviewing/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { reviewedBy });
  }

  // GET /api/Business/GetStatistics
  getStatistics(): Observable<BaseResponseModel<BusinessApprovalStats>> {
    const apiUrl = `${this.baseUrl}/GetStatistics`;
    return this.http.get<BaseResponseModel<BusinessApprovalStats>>(apiUrl);
  }

  // GET /api/Business/GetDocuments/{businessId}
  getDocuments(businessId: number): Observable<BaseResponseModel<DocumentInfo[]>> {
    const apiUrl = `${this.baseUrl}/GetDocuments/${businessId}`;
    return this.http.get<BaseResponseModel<DocumentInfo[]>>(apiUrl);
  }

  // POST /api/Business/UploadDocument/{businessId}
  uploadDocument(businessId: number, file: File, documentType: string): Observable<BaseResponseModel<DocumentInfo>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('documentType', documentType);
    
    const apiUrl = `${this.baseUrl}/UploadDocument/${businessId}`;
    return this.http.post<BaseResponseModel<DocumentInfo>>(apiUrl, formData);
  }

  // GET /api/Business/DownloadDocument/{documentId}
  downloadDocument(documentId: number): Observable<Blob> {
    const apiUrl = `${this.baseUrl}/DownloadDocument/${documentId}`;
    return this.http.get(apiUrl, { responseType: 'blob' });
  }

  verifyDocument(documentId: number): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/VerifyDocument/${documentId}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, {});
  }

  // ===== DUMMY DATA METHODS (for development) =====
  
  getAllDummy(): Observable<BaseResponseModel<BusinessApprovalData[]>> {
    const dummyData: BusinessApprovalData[] = [
      {
        id: 1,
        businessName: 'Công ty TNHH Công nghệ ABC',
        taxCode: '0123456789',
        contactPerson: 'Nguyễn Văn A',
        email: 'contact@abc-tech.com',
        phone: '0987654321',
        address: '123 Đường Lào Cai, Phường 1, TP. Lào Cai',
        businessType: 'Công nghệ thông tin',
        registrationDate: '2024-01-15',
        status: 'pending',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        documents: [
          { id: 1, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-15', verified: true },
          { id: 2, name: 'Giấy chứng nhận đăng ký thuế', type: 'PDF', url: '#', uploadDate: '2024-01-15', verified: false }
        ]
      },
      {
        id: 2,
        businessName: 'Công ty Cổ phần Thương mại XYZ',
        taxCode: '0987654321',
        contactPerson: 'Trần Thị B',
        email: 'info@xyz-trading.com',
        phone: '0123456789',
        address: '456 Đường Hoàng Liên, Phường 2, TP. Lào Cai',
        businessType: 'Thương mại',
        registrationDate: '2024-01-14',
        status: 'reviewing',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        documents: [
          { id: 3, name: 'Giấy phép kinh doanh', type: 'PDF', url: '#', uploadDate: '2024-01-14', verified: true },
          { id: 4, name: 'Báo cáo tài chính', type: 'Excel', url: '#', uploadDate: '2024-01-14', verified: true }
        ]
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getStatisticsDummy(): Observable<BaseResponseModel<BusinessApprovalStats>> {
    const dummyStats: BusinessApprovalStats = {
      pending: 156,
      approved: 1089,
      rejected: 23,
      reviewing: 45
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyStats
    });
  }
}
