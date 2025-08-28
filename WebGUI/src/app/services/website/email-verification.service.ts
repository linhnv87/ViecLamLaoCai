import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseResponseModel } from '../../models/base-response.model';
import { environment } from 'src/environments/environments';

export interface BusinessVerificationRequest {
  companyName: string;
  taxNumber: string;
  position: string;
  phone: string;
  email: string;
  address: string;
  website?: string;
  companySize?: string;
  description?: string;
  documents: {
    businessLicense?: File;
    representativeId?: File;
    companyStamp?: File;
  };
}

export interface BusinessVerificationResponse {
  id: number;
  companyId: number;
  status: 'pending' | 'approved' | 'rejected';
  submittedDate: string;
  reviewedDate?: string;
  reviewerNotes?: string;
  verificationCode: string;
}

export interface CompanyInfo {
  companyId: number;
  companyName: string;
  email: string;
  phoneNumber: string;
  address: string;
  representativeName: string;
  taxNumber: string;
  position: string;
  website: string;
  companySize: string;
  industry: string;
  description: string;
  isVerified: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class EmailVerificationService {
  private baseUrl = `${environment.apiUrl}/BusinessVerification`;

  constructor(private http: HttpClient) {}

  // Submit business verification
  submitVerification(request: BusinessVerificationRequest): Observable<BaseResponseModel<BusinessVerificationResponse>> {
    const formData = new FormData();
    
    console.log('📤 Submitting verification request:', request);
    
    formData.append('companyName', request.companyName);
    formData.append('taxNumber', request.taxNumber);
    formData.append('position', request.position);
    formData.append('phone', request.phone);
    formData.append('email', request.email);
    formData.append('address', request.address);
    
    if (request.website) {
      formData.append('website', request.website);
    }
    if (request.companySize) {
      formData.append('companySize', request.companySize);
    }
    if (request.description) {
      formData.append('description', request.description);
    }

    if (request.documents.businessLicense) {
      console.log('📎 Adding file 1:', request.documents.businessLicense.name, request.documents.businessLicense.size);
      formData.append('documents', request.documents.businessLicense);
    } else {
      console.warn('⚠️ No businessLicense file provided');
    }
    
    if (request.documents.representativeId) {
      console.log('📎 Adding file 2:', request.documents.representativeId.name, request.documents.representativeId.size);
      formData.append('documents', request.documents.representativeId);
    } else {
      console.warn('⚠️ No representativeId file provided');
    }
    
    if (request.documents.companyStamp) {
      console.log('📎 Adding file 3 (optional):', request.documents.companyStamp.name, request.documents.companyStamp.size);
      formData.append('documents', request.documents.companyStamp);
    }

    console.log('🌐 Sending to:', `${this.baseUrl}/SubmitVerification`);
    return this.http.post<BaseResponseModel<BusinessVerificationResponse>>(`${this.baseUrl}/SubmitVerification`, formData);
  }

  // Get verification status
  getVerificationStatus(companyId: number): Observable<BaseResponseModel<BusinessVerificationResponse>> {
    return this.http.get<BaseResponseModel<BusinessVerificationResponse>>(`${this.baseUrl}/GetStatus/${companyId}`);
  }

  // Get verification history
  getVerificationHistory(companyId: number): Observable<BaseResponseModel<BusinessVerificationResponse[]>> {
    return this.http.get<BaseResponseModel<BusinessVerificationResponse[]>>(`${this.baseUrl}/GetHistory/${companyId}`);
  }

  // Check if company is verified by company ID
  isCompanyVerified(companyId: number): Observable<BaseResponseModel<boolean>> {
    return this.http.get<BaseResponseModel<boolean>>(`${this.baseUrl}/IsVerified/${companyId}`);
  }

  // Check if company is verified by user ID
  isCompanyVerifiedByUserId(userId: string): Observable<BaseResponseModel<boolean>> {
    return this.http.get<BaseResponseModel<boolean>>(`${this.baseUrl}/IsVerifiedByUserId/${userId}`);
  }

  // Resend verification email
  resendVerificationEmail(verificationId: number): Observable<BaseResponseModel<string>> {
    return this.http.post<BaseResponseModel<string>>(`${this.baseUrl}/ResendEmail/${verificationId}`, {});
  }

  // Cancel pending verification
  cancelVerification(verificationId: number): Observable<BaseResponseModel<string>> {
    return this.http.delete<BaseResponseModel<string>>(`${this.baseUrl}/CancelVerification/${verificationId}`);
  }

  // Get company information by user ID
  getCompanyInfo(userId: string): Observable<BaseResponseModel<CompanyInfo>> {
    return this.http.get<BaseResponseModel<CompanyInfo>>(`${this.baseUrl}/GetCompanyInfo/${userId}`);
  }
}


