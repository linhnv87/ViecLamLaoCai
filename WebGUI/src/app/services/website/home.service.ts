import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { FeaturedJob, SuggestedJob, JobCategory, HomePageStats, LatestJob, FeaturedCompany, HomePageData, PaginatedResponse } from '../../models/home.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  private baseUrl = `${environment.apiUrl}/Home`;

  constructor(private http: HttpClient) {}

  // GET /api/Home/GetHomePageData
  getHomePageData(): Observable<BaseResponseModel<HomePageData>> {
    const apiUrl = `${this.baseUrl}/GetHomePageData`;
    return this.http.get<BaseResponseModel<HomePageData>>(apiUrl);
  }

  // GET /api/Home/GetFeaturedJobs/{count?}
  getFeaturedJobs(count?: number): Observable<BaseResponseModel<FeaturedJob[]>> {
    const apiUrl = `${this.baseUrl}/GetFeaturedJobs${count ? `/${count}` : ''}`;
    return this.http.get<BaseResponseModel<FeaturedJob[]>>(apiUrl);
  }

  // GET /api/Home/GetSuggestedJobs?count={count}&userId={userId}
  getSuggestedJobs(userId?: string, count?: number): Observable<BaseResponseModel<SuggestedJob[]>> {
    let apiUrl = `${this.baseUrl}/GetSuggestedJobs`;
    const params: string[] = [];
    if (count) params.push(`count=${count}`);
    if (userId) params.push(`userId=${userId}`);
    if (params.length > 0) apiUrl += `?${params.join('&')}`;
    
    return this.http.get<BaseResponseModel<SuggestedJob[]>>(apiUrl);
  }

  // GET /api/Home/GetJobCategories
  getJobCategories(): Observable<BaseResponseModel<JobCategory[]>> {
    const apiUrl = `${this.baseUrl}/GetJobCategories`;
    return this.http.get<BaseResponseModel<JobCategory[]>>(apiUrl);
  }

  // GET /api/Home/GetHomeStats
  getHomeStats(): Observable<BaseResponseModel<HomePageStats>> {
    const apiUrl = `${this.baseUrl}/GetHomeStats`;
    return this.http.get<BaseResponseModel<HomePageStats>>(apiUrl);
  }

  // POST /api/Home/SearchJobs - WITH PAGINATION
  searchJobs(searchQuery: {
    keyword?: string;
    location?: string;
    category?: string;
    salaryRange?: string;
    experience?: string;
    jobType?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortOrder?: string;
  }): Observable<BaseResponseModel<PaginatedResponse<FeaturedJob>>> {
    const apiUrl = `${this.baseUrl}/SearchJobs`;
    
    // Set default pagination if not provided
    const request = {
      ...searchQuery,
      page: searchQuery.page || 1,
      pageSize: searchQuery.pageSize || 10,
      sortBy: searchQuery.sortBy || 'relevance',
      sortOrder: searchQuery.sortOrder || 'desc'
    };
    
    console.log('🔍 Search API call with pagination:', request);
    return this.http.post<BaseResponseModel<PaginatedResponse<FeaturedJob>>>(apiUrl, request);
  }

  // GET /api/Home/GetPopularSearches/{count?}?period={period}
  getPopularSearches(count?: number, period?: 'day' | 'week' | 'month'): Observable<BaseResponseModel<string[]>> {
    let apiUrl = `${this.baseUrl}/GetPopularSearches${count ? `/${count}` : ''}`;
    if (period) apiUrl += `?period=${period}`;
    return this.http.get<BaseResponseModel<string[]>>(apiUrl);
  }

  // GET /api/Home/GetLatestJobs/{count?}
  getLatestJobs(count?: number): Observable<BaseResponseModel<LatestJob[]>> {
    const apiUrl = `${this.baseUrl}/GetLatestJobs${count ? `/${count}` : ''}`;
    return this.http.get<BaseResponseModel<LatestJob[]>>(apiUrl);
  }

  // GET /api/Home/GetFeaturedCompanies/{count?}
  getFeaturedCompanies(count?: number): Observable<BaseResponseModel<FeaturedCompany[]>> {
    const apiUrl = `${this.baseUrl}/GetFeaturedCompanies${count ? `/${count}` : ''}`;
    return this.http.get<BaseResponseModel<FeaturedCompany[]>>(apiUrl);
  }

  // ===== FALLBACK DUMMY DATA METHODS (for development/testing) =====
  
  getHomePageDataDummy(): Observable<BaseResponseModel<HomePageData>> {
    const dummyData: HomePageData = {
      stats: {
        totalJobs: 3456,
        totalCompanies: 1247,
        totalCandidates: 5678,
        newJobsThisWeek: 89,
        featuredJobsCount: 12,
        urgentJobsCount: 5
      },
      featuredJobs: [
        {
          id: 1,
          title: 'Frontend Developer - React/Vue.js',
          company: 'Công ty TNHH Công nghệ ABC',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '15 - 25 triệu',
          location: 'Lào Cai',
          urgent: true,
          daysLeft: 5,
          featured: true,
          jobType: 'Full-time',
          experience: '2-3 năm',
          postedDate: '2024-01-15',
          description: 'Tham gia phát triển các ứng dụng web hiện đại với React/Vue.js',
          requirements: ['React/Vue.js', 'JavaScript/TypeScript', 'HTML/CSS', 'Git'],
          benefits: ['Lương cạnh tranh', 'Bảo hiểm đầy đủ', 'Môi trường trẻ trung']
        },
        {
          id: 2,
          title: 'Kế toán tổng hợp',
          company: 'Công ty Cổ phần Đầu tư XYZ',
          logo: 'assets/vieclamlaocai/img/image 23.png',
          salary: '12 - 18 triệu',
          location: 'Lào Cai',
          urgent: false,
          daysLeft: 12,
          featured: true,
          jobType: 'Full-time',
          experience: '1-2 năm',
          postedDate: '2024-01-12',
          description: 'Thực hiện công việc kế toán tổng hợp cho công ty',
          requirements: ['Tốt nghiệp kế toán', 'Sử dụng Excel thành thạo', 'Có chứng chỉ'],
          benefits: ['Lương theo năng lực', 'Thưởng hiệu quả', 'Đào tạo chuyên môn']
        },
        {
          id: 3,
          title: 'Digital Marketing Specialist',
          company: 'Công ty TNHH Marketing DEF',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '10 - 20 triệu',
          location: 'Lào Cai',
          urgent: true,
          daysLeft: 3,
          featured: true,
          jobType: 'Full-time',
          experience: '1-3 năm',
          postedDate: '2024-01-10',
          description: 'Phụ trách các chiến dịch marketing online và offline',
          requirements: ['Facebook Ads', 'Google Ads', 'SEO/SEM', 'Analytics'],
          benefits: ['Hoa hồng cao', 'Thưởng dự án', 'Cơ hội thăng tiến']
        }
      ],
      suggestedJobs: [
        {
          id: 4,
          title: 'Backend Developer - Node.js',
          company: 'Công ty Cổ phần Phần mềm GHI',
          logo: 'assets/vieclamlaocai/img/image 23.png',
          salary: '18 - 30 triệu',
          experience: '2-3 năm kinh nghiệm',
          location: 'Lào Cai',
          jobType: 'Full-time',
          postedDate: '2024-01-14',
          description: 'Phát triển API và hệ thống backend với Node.js',
          requirements: ['Node.js', 'MongoDB/PostgreSQL', 'REST API', 'Docker']
        },
        {
          id: 5,
          title: 'UI/UX Designer',
          company: 'Công ty TNHH Thiết kế JKL',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '12 - 22 triệu',
          experience: '1-2 năm kinh nghiệm',
          location: 'Lào Cai',
          jobType: 'Full-time',
          postedDate: '2024-01-13',
          description: 'Thiết kế giao diện người dùng cho các ứng dụng web/mobile',
          requirements: ['Figma', 'Adobe Creative Suite', 'Wireframing', 'Prototyping']
        }
      ],
      jobCategories: [
        { icon: 'icon-bags.svg', title: 'Kinh doanh - Bán hàng', count: 5365, growthRate: 12.5, averageSalary: '12-25 triệu' },
        { icon: 'icon-color-calculator.svg', title: 'Tài chính - Kế toán', count: 1345, growthRate: 8.2, averageSalary: '10-20 triệu' },
        { icon: 'icon-color-town.svg', title: 'Bất động sản', count: 5365, growthRate: 15.8, averageSalary: '15-35 triệu' },
        { icon: 'icon-color-ai.svg', title: 'Công nghệ thông tin', count: 6345, growthRate: 25.3, averageSalary: '18-40 triệu' },
        { icon: 'icon-color-folder.svg', title: 'Hành chính - Thư ký', count: 165, growthRate: 5.1, averageSalary: '8-15 triệu' },
        { icon: 'icon-color-hat.svg', title: 'Xây dựng', count: 585, growthRate: 18.7, averageSalary: '12-28 triệu' }
      ],
      latestJobs: [
        {
          id: 201,
          title: 'Kế toán nội bộ - không yêu cầu kinh nghiệm',
          company: 'Công ty Cổ phần dịch vụ Công nghệ TSC Việt Nam',
          logo: 'assets/vieclamlaocai/img/image 16.png',
          salary: '15 - 50 triệu',
          location: 'Lào Cai',
          urgent: true,
          daysLeft: 5,
          postedDate: '2024-01-16',
          jobType: 'Full-time',
          experience: '0-1 năm'
        },
        {
          id: 202,
          title: 'Nhân viên Kinh doanh - Bán hàng',
          company: 'Công ty TNHH Thương mại ABC',
          logo: 'assets/vieclamlaocai/img/image 23.png',
          salary: '12 - 25 triệu',
          location: 'Lào Cai',
          urgent: false,
          daysLeft: 8,
          postedDate: '2024-01-16',
          jobType: 'Full-time',
          experience: '1-2 năm'
        }
      ],
      featuredCompanies: [
        {
          id: 101,
          name: 'Công ty TNHH Công nghệ ABC',
          logo: 'assets/vieclamlaocai/img/image 22.png',
          jobCount: 15,
          industry: 'Công nghệ thông tin',
          size: '50-200 nhân viên',
          location: 'Lào Cai',
          verified: true,
          description: 'Chuyên phát triển phần mềm và ứng dụng web',
          website: 'https://abctech.vn'
        },
        {
          id: 102,
          name: 'Công ty CP Đầu tư XYZ',
          logo: 'assets/vieclamlaocai/img/image 19.png',
          jobCount: 12,
          industry: 'Tài chính - Đầu tư',
          size: '100-500 nhân viên',
          location: 'Lào Cai',
          verified: true,
          description: 'Đầu tư và phát triển bất động sản',
          website: 'https://xyzinvest.vn'
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

  getFeaturedJobsDummy(count?: number): Observable<BaseResponseModel<FeaturedJob[]>> {
    const featuredJobs: FeaturedJob[] = [
      {
        id: 1,
        title: 'Frontend Developer - React/Vue.js',
        company: 'Công ty TNHH Công nghệ ABC',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '15 - 25 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 5,
        featured: true,
        jobType: 'Full-time',
        experience: '2-3 năm',
        postedDate: '2024-01-15'
      },
      {
        id: 2,
        title: 'Kế toán tổng hợp',
        company: 'Công ty Cổ phần Đầu tư XYZ',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '12 - 18 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 12,
        featured: true,
        jobType: 'Full-time',
        experience: '1-2 năm',
        postedDate: '2024-01-12'
      },
      {
        id: 3,
        title: 'Digital Marketing Specialist',
        company: 'Công ty TNHH Marketing DEF',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '10 - 20 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 3,
        featured: true,
        jobType: 'Full-time',
        experience: '1-3 năm',
        postedDate: '2024-01-10'
      }
    ];

    const result = count ? featuredJobs.slice(0, count) : featuredJobs;

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: result
    });
  }

  getSuggestedJobsDummy(userId?: string, count?: number): Observable<BaseResponseModel<SuggestedJob[]>> {
    const suggestedJobs: SuggestedJob[] = [
      {
        id: 4,
        title: 'Nhân Viên Tư Vấn Bán Hàng Ô Tô GAC',
        company: 'Công ty TNHH Đầu tư Tripple Bulls',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '15 - 50 triệu',
        experience: '3 - 4 năm kinh nghiệm',
        location: 'Lào Cai',
        jobType: 'Full-time',
        postedDate: '2024-01-14'
      },
      {
        id: 5,
        title: 'Chuyên viên Marketing Online',
        company: 'Công ty TNHH Thiết kế JKL',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '12 - 22 triệu',
        experience: '1-2 năm kinh nghiệm',
        location: 'Lào Cai',
        jobType: 'Full-time',
        postedDate: '2024-01-13'
      }
    ];

    const result = count ? suggestedJobs.slice(0, count) : suggestedJobs;

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: result
    });
  }

  getJobCategoriesDummy(): Observable<BaseResponseModel<JobCategory[]>> {
    const categories: JobCategory[] = [
      { icon: 'icon-bags.svg', title: 'Kinh doanh - Bán hàng', count: 5365, growthRate: 12.5, averageSalary: '12-25 triệu' },
      { icon: 'icon-color-calculator.svg', title: 'Tài chính - Kế toán', count: 1345, growthRate: 8.2, averageSalary: '10-20 triệu' },
      { icon: 'icon-color-town.svg', title: 'Bất động sản', count: 5365, growthRate: 15.8, averageSalary: '15-35 triệu' },
      { icon: 'icon-color-ai.svg', title: 'Công nghệ thông tin', count: 6345, growthRate: 25.3, averageSalary: '18-40 triệu' },
      { icon: 'icon-color-folder.svg', title: 'Hành chính - Thư ký', count: 165, growthRate: 5.1, averageSalary: '8-15 triệu' },
      { icon: 'icon-color-hat.svg', title: 'Xây dựng', count: 585, growthRate: 18.7, averageSalary: '12-28 triệu' }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: categories
    });
  }

  getHomeStatsDummy(): Observable<BaseResponseModel<HomePageStats>> {
    const stats: HomePageStats = {
      totalJobs: 3456,
      totalCompanies: 1247,
      totalCandidates: 5678,
      newJobsThisWeek: 89,
      featuredJobsCount: 12,
      urgentJobsCount: 5
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: stats
    });
  }

  getLatestJobsDummy(count?: number): Observable<BaseResponseModel<LatestJob[]>> {
    const latestJobs: LatestJob[] = [
      {
        id: 201,
        title: 'Kế toán nội bộ - không yêu cầu kinh nghiệm',
        company: 'Công ty Cổ phần dịch vụ Công nghệ TSC Việt Nam',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '15 - 50 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 5,
        postedDate: '2024-01-16',
        jobType: 'Full-time',
        experience: '0-1 năm'
      },
      {
        id: 202,
        title: 'Nhân viên Kinh doanh - Bán hàng',
        company: 'Công ty TNHH Thương mại ABC',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '12 - 25 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 8,
        postedDate: '2024-01-16',
        jobType: 'Full-time',
        experience: '1-2 năm'
      }
    ];

    const result = count ? latestJobs.slice(0, count) : latestJobs;

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: result
    });
  }

  getFeaturedCompaniesDummy(count?: number): Observable<BaseResponseModel<FeaturedCompany[]>> {
    const featuredCompanies: FeaturedCompany[] = [
      {
        id: 101,
        name: 'Công ty TNHH Công nghệ ABC',
        logo: 'assets/vieclamlaocai/img/image 22.png',
        jobCount: 15,
        industry: 'Công nghệ thông tin',
        size: '50-200 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Chuyên phát triển phần mềm và ứng dụng web',
        website: 'https://abctech.vn'
      },
      {
        id: 102,
        name: 'Công ty CP Đầu tư XYZ',
        logo: 'assets/vieclamlaocai/img/image 19.png',
        jobCount: 12,
        industry: 'Tài chính - Đầu tư',
        size: '100-500 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Đầu tư và phát triển bất động sản',
        website: 'https://xyzinvest.vn'
      }
    ];

    const result = count ? featuredCompanies.slice(0, count) : featuredCompanies;

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: result
    });
  }
}





