import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { environment } from '../../../environments/environments';

export interface BusinessDashboardModel {
  totalJobs: number;
  activeJobs: number;
  totalApplications: number;
  totalViews: number;
  todayApplications: number;
  todayViews: number;
  recentJobs: RecentJobModel[];
  recentCandidates: RecentCandidateModel[];
}

export interface CandidateDashboardModel {
  totalApplications: number;
  pendingApplications: number;
  approvedApplications: number;
  rejectedApplications: number;
  profileViews: number;
  savedJobs: SavedJobModel[];
  recentApplications: RecentApplicationModel[];
}

export interface AdminDashboardModel {
  totalBusinesses: number;
  pendingApprovals: number;
  totalJobs: number;
  totalCandidates: number;
  todayRegistrations: number;
  systemHealth: SystemHealthModel;
}

export interface RecentJobModel {
  id: number;
  title: string;
  company: string;
  applications: number;
  views: number;
  postedDate: string;
  status: string;
  urgent: boolean;
}

export interface RecentCandidateModel {
  id: number;
  name: string;
  position: string;
  experience: string;
  education: string;
  appliedDate: string;
  status: string;
  avatar: string;
}

export interface SavedJobModel {
  id: number;
  jobTitle: string;
  company: string;
  logo: string;
  salary: string;
  savedDate: string;
  location: string;
  urgent: boolean;
}

export interface RecentApplicationModel {
  id: number;
  jobTitle: string;
  company: string;
  logo: string;
  salary: string;
  status: string;
  appliedDate: string;
  location: string;
}

export interface ActivityModel {
  id: number;
  type: 'application' | 'job_view' | 'profile_update' | 'message';
  title: string;
  description: string;
  timestamp: string;
  userId: string;
}

export interface SystemHealthModel {
  status: 'healthy' | 'warning' | 'error';
  uptime: string;
  responseTime: number;
  activeUsers: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = `${environment.apiUrl}/Dashboard`;

  constructor(private http: HttpClient) {}

  // GET /api/Dashboard/GetBusinessDashboard/{businessId}
  getBusinessDashboard(businessId: number): Observable<BaseResponseModel<BusinessDashboardModel>> {
    const apiUrl = `${this.baseUrl}/GetBusinessDashboard/${businessId}`;
    return this.http.get<BaseResponseModel<BusinessDashboardModel>>(apiUrl);
  }

  // GET /api/Dashboard/GetCandidateDashboard/{candidateId}
  getCandidateDashboard(candidateId: number): Observable<BaseResponseModel<CandidateDashboardModel>> {
    const apiUrl = `${this.baseUrl}/GetCandidateDashboard/${candidateId}`;
    return this.http.get<BaseResponseModel<CandidateDashboardModel>>(apiUrl);
  }

  // GET /api/Dashboard/GetAdminDashboard
  getAdminDashboard(): Observable<BaseResponseModel<AdminDashboardModel>> {
    const apiUrl = `${this.baseUrl}/GetAdminDashboard`;
    return this.http.get<BaseResponseModel<AdminDashboardModel>>(apiUrl);
  }

  // GET /api/Dashboard/GetRecentActivities/{userId}
  getRecentActivities(userId: string): Observable<BaseResponseModel<ActivityModel[]>> {
    const apiUrl = `${this.baseUrl}/GetRecentActivities/${userId}`;
    return this.http.get<BaseResponseModel<ActivityModel[]>>(apiUrl);
  }

  // ===== DUMMY DATA METHODS (for development) =====

  getBusinessDashboardDummy(): Observable<BaseResponseModel<BusinessDashboardModel>> {
    const dummyData: BusinessDashboardModel = {
      totalJobs: 1247,
      activeJobs: 156,
      totalApplications: 89,
      totalViews: 12450,
      todayApplications: 23,
      todayViews: 567,
      recentJobs: [
        {
          id: 1,
          title: 'Frontend Developer',
          company: 'Công ty ABC Tech',
          applications: 45,
          views: 234,
          postedDate: '2024-01-15',
          status: 'active',
          urgent: true
        }
      ],
      recentCandidates: [
        {
          id: 1,
          name: 'Nguyễn Văn A',
          position: 'Frontend Developer',
          experience: '3 năm',
          education: 'Đại học Bách Khoa',
          appliedDate: '2024-01-15',
          status: 'new',
          avatar: 'assets/vieclamlaocai/img/image 16.png'
        }
      ]
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getCandidateDashboardDummy(): Observable<BaseResponseModel<CandidateDashboardModel>> {
    const dummyData: CandidateDashboardModel = {
      totalApplications: 15,
      pendingApplications: 8,
      approvedApplications: 5,
      rejectedApplications: 2,
      profileViews: 234,
      savedJobs: [
        {
          id: 1,
          jobTitle: 'Full Stack Developer',
          company: 'Công ty Cổ phần Phần mềm GHI',
          logo: 'assets/vieclamlaocai/img/image 23.png',
          salary: '20 - 35 triệu',
          savedDate: '2024-01-12',
          location: 'Lào Cai',
          urgent: true
        }
      ],
      recentApplications: [
        {
          id: 1,
          jobTitle: 'Frontend Developer - React/Angular',
          company: 'Công ty TNHH Công nghệ ABC',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '15 - 25 triệu',
          status: 'pending',
          appliedDate: '2024-01-15',
          location: 'Lào Cai'
        }
      ]
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }
}
