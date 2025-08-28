import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { BaseResponseModel } from '../../models/base-response.model';
import { environment } from 'src/environments/environments';

export interface BusinessRegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  representativeName: string;
  phone: string;
  companyName: string;
  address: string;
  industry?: string;
  companySize?: string;
  website?: string;
  description?: string;
}

export interface CandidateRegisterRequest {
  fullName: string;
  phone: string;
  email: string;
  password: string;
  confirmPassword: string;
  dateOfBirth?: Date;
  gender?: string;
  address?: string;
  districtId?: number;
  communeId?: number;
  educationLevelId?: number;
  careerId?: number;
}

export interface RegistrationResponse {
  userId: string;
  message: string;
  requiresEmailVerification: boolean;
  requiresApproval: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.apiUrl}/AppUser`;

  constructor(private http: HttpClient) {}

  registerBusiness(request: BusinessRegisterRequest): Observable<BaseResponseModel<RegistrationResponse>> {
    return this.http.post<BaseResponseModel<RegistrationResponse>>(`${this.baseUrl}/RegisterBusiness`, request);
  }

  registerCandidate(request: CandidateRegisterRequest): Observable<BaseResponseModel<RegistrationResponse>> {
    return this.http.post<BaseResponseModel<RegistrationResponse>>(`${this.baseUrl}/RegisterCandidate`, request);
  }
}
