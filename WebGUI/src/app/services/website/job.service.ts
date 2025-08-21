import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { environment } from '../../../environments/environments';

export interface JobModel {
  id: number;
  title: string;
  company: string;
  logo: string;
  salary: string;
  location: string;
  description: string;
  requirements: string[];
  benefits: string[];
  jobType: 'full-time' | 'part-time' | 'contract' | 'internship';
  experience: string;
  industry: string;
  postedDate: string;
  expiryDate: string;
  status: 'active' | 'paused' | 'closed';
  urgent: boolean;
  applications: number;
  views: number;
  businessId: number;
}

export interface JobPostingModel {
  id?: number;
  title: string;
  description: string;
  requirements: string;
  benefits: string;
  salary: string;
  location: string;
  jobType: string;
  experience: string;
  industry: string;
  expiryDate: string;
  businessId: number;
}

export interface JobSearchModel {
  keyword?: string;
  location?: string;
  industry?: string;
  salary?: string;
  jobType?: string;
  experience?: string;
  page: number;
  limit: number;
}

export interface JobPostingStats {
  totalJobs: number;
  activeJobs: number;
  pausedJobs: number;
  closedJobs: number;
  totalApplications: number;
  totalViews: number;
}

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private baseUrl = `${environment.apiUrl}/Job`;

  constructor(private http: HttpClient) {}

  // GET /api/Job/GetAll
  getAll(status?: string, businessId?: number, page: number = 1, limit: number = 10): Observable<BaseResponseModel<JobModel[]>> {
    let apiUrl = `${this.baseUrl}/GetAll?page=${page}&limit=${limit}`;
    if (status) apiUrl += `&status=${status}`;
    if (businessId) apiUrl += `&businessId=${businessId}`;
    
    return this.http.get<BaseResponseModel<JobModel[]>>(apiUrl);
  }

  // GET /api/Job/GetById/{id}
  getById(id: number): Observable<BaseResponseModel<JobModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<JobModel>>(apiUrl);
  }

  // POST /api/Job/Create
  create(job: JobPostingModel): Observable<BaseResponseModel<JobModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<JobModel>>(apiUrl, job);
  }

  // PUT /api/Job/Update/{id}
  update(id: number, job: JobPostingModel): Observable<BaseResponseModel<JobModel>> {
    const apiUrl = `${this.baseUrl}/Update/${id}`;
    return this.http.put<BaseResponseModel<JobModel>>(apiUrl, job);
  }

  // PUT /api/Job/ChangeStatus/{id}
  changeStatus(id: number, status: 'active' | 'paused' | 'closed'): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/ChangeStatus/${id}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, { status });
  }

  // DELETE /api/Job/Delete/{id}
  delete(id: number): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<boolean>>(apiUrl);
  }

  // POST /api/Job/Search
  search(searchModel: JobSearchModel): Observable<BaseResponseModel<JobModel[]>> {
    const apiUrl = `${this.baseUrl}/Search`;
    return this.http.post<BaseResponseModel<JobModel[]>>(apiUrl, searchModel);
  }

  // GET /api/Job/GetStatistics/{businessId}
  getStatistics(businessId: number): Observable<BaseResponseModel<JobPostingStats>> {
    const apiUrl = `${this.baseUrl}/GetStatistics/${businessId}`;
    return this.http.get<BaseResponseModel<JobPostingStats>>(apiUrl);
  }

  // ===== DUMMY DATA METHODS (for development) =====

  getAllDummy(): Observable<BaseResponseModel<JobModel[]>> {
    const dummyData: JobModel[] = [
      {
        id: 1,
        title: 'Frontend Developer - React/Angular',
        company: 'Công ty TNHH Công nghệ ABC',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '15 - 25 triệu',
        location: 'Lào Cai',
        description: 'Phát triển giao diện người dùng cho các ứng dụng web',
        requirements: ['React/Angular', 'TypeScript', '2+ năm kinh nghiệm'],
        benefits: ['Bảo hiểm y tế', 'Thưởng theo dự án', 'Đào tạo'],
        jobType: 'full-time',
        experience: '2-3 năm',
        industry: 'Công nghệ thông tin',
        postedDate: '2024-01-15',
        expiryDate: '2024-02-15',
        status: 'active',
        urgent: true,
        applications: 45,
        views: 234,
        businessId: 1
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }
}
