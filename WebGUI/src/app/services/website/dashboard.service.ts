import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { BusinessDashboardModel, CandidateDashboardModel, AdminDashboardModel, RecentJobModel, RecentCandidateModel, SavedJobModel, RecentApplicationModel, ActivityModel, SystemHealthModel } from '../../models/dashboard.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private baseUrl = `${environment.apiUrl}/Dashboard`;

  constructor(private http: HttpClient) {}

  // GET /api/Dashboard/GetBusinessDashboard/{userId}
  getBusinessDashboard(userId: string): Observable<BaseResponseModel<BusinessDashboardModel>> {
    const apiUrl = `${this.baseUrl}/GetBusinessDashboard/${userId}`;
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

  // ===== DETAILED BUSINESS DASHBOARD APIs =====

  // GET /api/Dashboard/GetRecentJobs/{businessId}?companyName={companyName}
  getRecentJobs(businessId: number, companyName: string): Observable<BaseResponseModel<RecentJobModel[]>> {
    const apiUrl = `${this.baseUrl}/GetRecentJobs/${businessId}?companyName=${encodeURIComponent(companyName)}`;
    return this.http.get<BaseResponseModel<RecentJobModel[]>>(apiUrl);
  }

  // GET /api/Dashboard/GetRecentCandidates/{businessId}
  getRecentCandidates(businessId: number): Observable<BaseResponseModel<RecentCandidateModel[]>> {
    const apiUrl = `${this.baseUrl}/GetRecentCandidates/${businessId}`;
    return this.http.get<BaseResponseModel<RecentCandidateModel[]>>(apiUrl);
  }

  // ===== DETAILED CANDIDATE DASHBOARD APIs =====

  // GET /api/Dashboard/GetSavedJobs/{candidateId}
  getSavedJobs(candidateId: number): Observable<BaseResponseModel<SavedJobModel[]>> {
    const apiUrl = `${this.baseUrl}/GetSavedJobs/${candidateId}`;
    return this.http.get<BaseResponseModel<SavedJobModel[]>>(apiUrl);
  }

  // GET /api/Dashboard/GetRecentApplications/{candidateId}
  getRecentApplications(candidateId: number): Observable<BaseResponseModel<RecentApplicationModel[]>> {
    const apiUrl = `${this.baseUrl}/GetRecentApplications/${candidateId}`;
    return this.http.get<BaseResponseModel<RecentApplicationModel[]>>(apiUrl);
  }

  // GET /api/Dashboard/GetAppliedJobs/{candidateId}
  getAppliedJobs(candidateId: number): Observable<BaseResponseModel<RecentApplicationModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAppliedJobs/${candidateId}`;
    return this.http.get<BaseResponseModel<RecentApplicationModel[]>>(apiUrl);
  }

  // ===== ADMIN DASHBOARD DETAIL APIs =====

  // GET /api/Dashboard/GetTotalBusinesses
  getTotalBusinesses(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetTotalBusinesses`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetPendingApprovals
  getPendingApprovals(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetPendingApprovals`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetApprovedBusinesses
  getApprovedBusinesses(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetApprovedBusinesses`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetRejectedBusinesses
  getRejectedBusinesses(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetRejectedBusinesses`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetTotalJobs
  getTotalJobs(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetTotalJobs`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetTotalCandidates
  getTotalCandidates(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetTotalCandidates`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetTodayRegistrations
  getTodayRegistrations(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetTodayRegistrations`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  // GET /api/Dashboard/GetActiveUsersCount
  getActiveUsersCount(): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/GetActiveUsersCount`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
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
          experience: '3 năm kinh nghiệm',
          education: 'Đại học Bách Khoa Hà Nội',
          appliedDate: '2024-01-15',
          status: 'new',
          avatar: 'assets/vieclamlaocai/img/image 16.png',
          age: 28,
          location: 'Lào Cai',
          level: 'Nhân viên',
          industry: 'Công nghệ thông tin',
          previousCompany: 'Công ty TNHH ABC Tech',
          salaryExpectation: '15 - 25 triệu'
        },
        {
          id: 2,
          name: 'Trần Thị B',
          position: 'Kế toán',
          experience: '5 năm kinh nghiệm',
          education: 'Đại học Kinh tế Quốc dân',
          appliedDate: '2024-01-14',
          status: 'reviewed',
          avatar: 'assets/vieclamlaocai/img/Ellipse 6.png',
          age: 32,
          location: 'Lào Cai',
          level: 'Trưởng nhóm',
          industry: 'Kế toán - Kiểm toán',
          previousCompany: 'Công ty CP Đầu tư XYZ',
          salaryExpectation: '12 - 18 triệu'
        },
        {
          id: 3,
          name: 'Lê Văn C',
          position: 'Marketing Executive',
          experience: '2 năm kinh nghiệm',
          education: 'Đại học Ngoại thương',
          appliedDate: '2024-01-13',
          status: 'contacted',
          avatar: 'assets/vieclamlaocai/img/image 23.png',
          age: 26,
          location: 'Lào Cai',
          level: 'Nhân viên',
          industry: 'Marketing - PR',
          previousCompany: 'Công ty TNHH Marketing DEF',
          salaryExpectation: '10 - 15 triệu'
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
      profileViews: 0,
      suitableJobs: 74,
      employerEmails: 0,
      totalCVs: 0,
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
      ],
      appliedJobs: [
        {
          id: 1,
          jobTitle: 'Frontend Developer - React/Angular',
          company: 'Công ty TNHH Công nghệ ABC',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '15 - 25 triệu',
          status: 'pending',
          appliedDate: '2024-01-15',
          location: 'Lào Cai'
        },
        {
          id: 2,
          jobTitle: 'Backend Developer - Node.js',
          company: 'Công ty Cổ phần Phần mềm GHI',
          logo: 'assets/vieclamlaocai/img/image 23.png',
          salary: '25 - 40 triệu',
          status: 'reviewing',
          appliedDate: '2024-01-14',
          location: 'Hà Nội'
        },
        {
          id: 3,
          jobTitle: 'UI/UX Designer',
          company: 'Công ty TNHH Thiết kế XYZ',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '18 - 30 triệu',
          status: 'accepted',
          appliedDate: '2024-01-13',
          location: 'TP. Hồ Chí Minh'
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

  getAdminDashboardDummy(): Observable<BaseResponseModel<AdminDashboardModel>> {
    const dummyData: AdminDashboardModel = {
      totalBusinesses: 60,
      pendingApprovals: 12,
      approvedBusinesses: 45,
      rejectedBusinesses: 3,
      totalJobs: 1247,
      totalCandidates: 89,
      todayRegistrations: 5,
      systemHealth: {
        status: 'healthy',
        uptime: '99.9%',
        responseTime: 150,
        activeUsers: 234
      }
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }
}






