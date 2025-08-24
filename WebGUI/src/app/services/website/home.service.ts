import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { FeaturedJob, SuggestedJob, JobCategory, HomePageStats, LatestJob, FeaturedCompany, HomePageData } from '../../models/home.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  private baseUrl = `${environment.apiUrl}/Home`;

  constructor(private http: HttpClient) {}
  // GET /api/Home/GetJobs?type=latest|featured|suggested&count=6&userId=123
  getJobs(type: 'latest' | 'featured' | 'suggested', options?: { count?: number; userId?: string }): Observable<BaseResponseModel<FeaturedJob[] | LatestJob[] | SuggestedJob[]>> {
    let apiUrl = `${this.baseUrl}/GetJobs`;
    const params: string[] = [];
    params.push(`type=${encodeURIComponent(type)}`);
    if (options?.count !== undefined) params.push(`count=${options.count}`);
    if (type === 'suggested' && options?.userId) params.push(`userId=${encodeURIComponent(options.userId)}`);
    if (params.length > 0) apiUrl += `?${params.join('&')}`;
    return this.http.get<BaseResponseModel<any>>(apiUrl);
  }


  // GET /api/Home/GetHomePageData
  getHomePageData(): Observable<BaseResponseModel<HomePageData>> {
    const apiUrl = `${this.baseUrl}/GetHomePageData`;
    return this.http.get<BaseResponseModel<HomePageData>>(apiUrl);
  }

  // GET /api/Home/GetFeaturedJobs
  getFeaturedJobs(count?: number): Observable<BaseResponseModel<FeaturedJob[]>> {
    const apiUrl = `${this.baseUrl}/GetTopFeaturedJobs${typeof count === 'number' ? `/${count}` : ''}`;
    return this.http.get<BaseResponseModel<FeaturedJob[]>>(apiUrl);
  }

  // GET /api/Home/GetSuggestedJobs
  getSuggestedJobs(userId?: string, count?: number): Observable<BaseResponseModel<SuggestedJob[]>> {
    let apiUrl = `${this.baseUrl}/GetTopSuggestedJobs`;
    const params: string[] = [];
    if (typeof count === 'number') params.push(`count=${count}`);
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

  // POST /api/Home/SearchJobs
  searchJobs(searchQuery: {
    keyword?: string;
    location?: string;
    category?: string;
    salaryRange?: string;
    experience?: string;
  }): Observable<BaseResponseModel<any[]>> {
    const apiUrl = `${this.baseUrl}/SearchJobs`;
    return this.http.post<BaseResponseModel<any[]>>(apiUrl, searchQuery);
  }

  // GET /api/Home/GetPopularSearches
  getPopularSearches(count?: number, period?: 'day' | 'week' | 'month'): Observable<BaseResponseModel<string[]>> {
    let apiUrl = `${this.baseUrl}/GetPopularSearches${typeof count === 'number' ? `/${count}` : ''}`;
    const params: string[] = [];
    if (period) params.push(`period=${encodeURIComponent(period)}`);
    if (params.length > 0) apiUrl += `?${params.join('&')}`;
    return this.http.get<BaseResponseModel<string[]>>(apiUrl);
  }

  // GET /api/Home/GetLatestJobs
  getLatestJobs(count?: number): Observable<BaseResponseModel<LatestJob[]>> {
    const apiUrl = `${this.baseUrl}/GetLatestJobs${typeof count === 'number' ? `/${count}` : ''}`;
    return this.http.get<BaseResponseModel<LatestJob[]>>(apiUrl);
  }

  // GET /api/Home/GetFeaturedCompanies
  getFeaturedCompanies(count?: number): Observable<BaseResponseModel<FeaturedCompany[]>> {
    const apiUrl = `${this.baseUrl}/GetFeaturedCompanies${typeof count === 'number' ? `/${count}` : ''}`;
    return this.http.get<BaseResponseModel<FeaturedCompany[]>>(apiUrl);
  }

  // ===== DUMMY DATA METHODS (for development) =====
  
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
      recentActivities: []
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
      },
      {
        id: 4,
        title: 'Backend Developer - Node.js',
        company: 'Công ty Cổ phần Phần mềm GHI',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '18 - 30 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 8,
        featured: true,
        jobType: 'Full-time',
        experience: '3-5 năm',
        postedDate: '2024-01-14'
      },
      {
        id: 5,
        title: 'UI/UX Designer',
        company: 'Công ty TNHH Thiết kế JKL',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '12 - 22 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 2,
        featured: true,
        jobType: 'Full-time',
        experience: '2-4 năm',
        postedDate: '2024-01-13'
      },
      {
        id: 6,
        title: 'Sales Manager',
        company: 'Công ty TNHH Thương mại MNO',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '20 - 35 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 15,
        featured: true,
        jobType: 'Full-time',
        experience: '3-5 năm',
        postedDate: '2024-01-11'
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
      },
      {
        id: 6,
        title: 'Kỹ sư Phần mềm Java',
        company: 'Công ty Cổ phần Công nghệ PQR',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '20 - 35 triệu',
        experience: '2-4 năm kinh nghiệm',
        location: 'Lào Cai',
        jobType: 'Full-time',
        postedDate: '2024-01-12'
      },
      {
        id: 7,
        title: 'Nhân viên Kinh doanh B2B',
        company: 'Công ty TNHH Thương mại STU',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '10 - 25 triệu',
        experience: '1-3 năm kinh nghiệm',
        location: 'Lào Cai',
        jobType: 'Full-time',
        postedDate: '2024-01-11'
      },
      {
        id: 8,
        title: 'Graphic Designer',
        company: 'Công ty TNHH Sáng tạo VWX',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '8 - 15 triệu',
        experience: '1-2 năm kinh nghiệm',
        location: 'Lào Cai',
        jobType: 'Full-time',
        postedDate: '2024-01-10'
      },
      {
        id: 9,
        title: 'Trưởng phòng Nhân sự',
        company: 'Công ty Cổ phần Đầu tư YZ',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '25 - 40 triệu',
        experience: '5+ năm kinh nghiệm',
        location: 'Lào Cai',
        jobType: 'Full-time',
        postedDate: '2024-01-09'
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

  searchJobsDummy(searchQuery: any): Observable<BaseResponseModel<FeaturedJob[]>> {
    // Get comprehensive search results including featured jobs
    const featuredJobs = [
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
        description: 'Phát triển giao diện người dùng cho các ứng dụng web sử dụng React hoặc Vue.js...',
        requirements: ['React/Vue.js', 'JavaScript', 'HTML/CSS', 'Git']
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
        description: 'Thực hiện các công việc kế toán tổng hợp, lập báo cáo tài chính định kỳ...',
        requirements: ['Bằng cử nhân Kế toán', 'Excel thành thạo', 'Kinh nghiệm 1-2 năm']
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
        description: 'Lập kế hoạch và thực hiện các chiến dịch marketing online...',
        requirements: ['Facebook Ads', 'Google Ads', 'Content Marketing', 'Analytics']
      }
    ];

    // Additional search results
    const additionalJobs = [
      {
        id: 101,
        title: 'Nhân viên Kế toán Tổng hợp',
        company: 'Công ty TNHH Kế toán ABC',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '12 - 18 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 10,
        featured: false,
        jobType: 'Full-time',
        experience: '2-3 năm',
        postedDate: '2024-01-15',
        description: 'Thực hiện công tác kế toán tổng hợp, lập báo cáo tài chính, theo dõi công nợ...',
        requirements: ['Tốt nghiệp Kế toán', 'Sử dụng thành thạo Excel', 'Kinh nghiệm 2 năm']
      },
      {
        id: 102,
        title: 'Chuyên viên Tuyển dụng',
        company: 'Công ty CP Nhân lực DEF',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '15 - 25 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 7,
        featured: false,
        jobType: 'Full-time',
        experience: '1-3 năm',
        postedDate: '2024-01-14',
        description: 'Tìm kiếm và tuyển dụng nhân sự cho các vị trí khác nhau trong công ty...',
        requirements: ['Tốt nghiệp Đại học', 'Kỹ năng giao tiếp tốt', 'Am hiểu về HR']
      },
      {
        id: 103,
        title: 'Lập trình viên Python',
        company: 'Công ty TNHH Công nghệ GHI',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '20 - 35 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 15,
        featured: false,
        jobType: 'Full-time',
        experience: '2-4 năm',
        postedDate: '2024-01-13',
        description: 'Phát triển ứng dụng web bằng Python/Django, xây dựng API...',
        requirements: ['Python/Django', 'Database SQL', 'Git', 'RESTful API']
      },
      {
        id: 104,
        title: 'Nhân viên Bán hàng Online',
        company: 'Công ty TNHH Thương mại JKL',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '8 - 15 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 5,
        featured: false,
        jobType: 'Full-time',
        experience: '0-2 năm',
        postedDate: '2024-01-12',
        description: 'Tư vấn khách hàng qua các kênh online, chăm sóc khách hàng...',
        requirements: ['Kỹ năng bán hàng', 'Sử dụng Facebook', 'Giao tiếp tốt']
      }
    ];

    const allJobs = [...featuredJobs, ...additionalJobs];

    // Apply search filters
    let filteredResults = allJobs;

    if (searchQuery.keyword || searchQuery.q) {
      const keyword = (searchQuery.keyword || searchQuery.q).toLowerCase();
      filteredResults = filteredResults.filter(job => 
        job.title.toLowerCase().includes(keyword) ||
        job.company.toLowerCase().includes(keyword) ||
        (job.description && job.description.toLowerCase().includes(keyword)) ||
        (job.requirements && job.requirements.some(req => req.toLowerCase().includes(keyword)))
      );
    }

    if (searchQuery.location) {
      filteredResults = filteredResults.filter(job => 
        job.location.includes(searchQuery.location)
      );
    }

    if (searchQuery.salaryRange || searchQuery.salary) {
      const salaryRange = searchQuery.salaryRange || searchQuery.salary;
      filteredResults = filteredResults.filter(job => {
        if (salaryRange === '5-10') {
          return job.salary.includes('5') || job.salary.includes('8') || job.salary.includes('10');
        } else if (salaryRange === '10-15') {
          return job.salary.includes('10') || job.salary.includes('12') || job.salary.includes('15');
        } else if (salaryRange === '15-25') {
          return job.salary.includes('15') || job.salary.includes('18') || job.salary.includes('20') || job.salary.includes('22') || job.salary.includes('25');
        } else if (salaryRange === '25-35') {
          return job.salary.includes('25') || job.salary.includes('28') || job.salary.includes('30') || job.salary.includes('35');
        } else if (salaryRange === '35+') {
          return job.salary.includes('35') || job.salary.includes('40') || job.salary.includes('50');
        }
        return true;
      });
    }

    if (searchQuery.experience) {
      filteredResults = filteredResults.filter(job => {
        if (searchQuery.experience === '0-1') {
          return job.experience?.includes('0') || job.experience?.includes('1');
        } else if (searchQuery.experience === '1-2') {
          return job.experience?.includes('1') || job.experience?.includes('2');
        } else if (searchQuery.experience === '2-5') {
          return job.experience?.includes('2') || job.experience?.includes('3') || job.experience?.includes('4') || job.experience?.includes('5');
        } else if (searchQuery.experience === '5+') {
          return job.experience?.includes('5+') || job.experience?.includes('5') || job.experience?.includes('trên');
        }
        return true;
      });
    }

    if (searchQuery.jobType) {
      filteredResults = filteredResults.filter(job => 
        job.jobType === searchQuery.jobType
      );
    }

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Search completed successfully',
      result: filteredResults
    });
  }

  getPopularSearchesDummy(): Observable<BaseResponseModel<string[]>> {
    const popularSearches = [
      'Frontend Developer',
      'Backend Developer',
      'Full Stack Developer',
      'UI/UX Designer',
      'Digital Marketing',
      'Kế toán',
      'Nhân viên bán hàng',
      'Project Manager'
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: popularSearches
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
      },
      {
        id: 203,
        title: 'Chuyên viên IT Support',
        company: 'Công ty CP Công nghệ DEF',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '10 - 18 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 3,
        postedDate: '2024-01-15',
        jobType: 'Full-time',
        experience: '1-3 năm'
      },
      {
        id: 204,
        title: 'Nhân viên Hành chính - Nhân sự',
        company: 'Công ty TNHH Dịch vụ GHI',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '8 - 15 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 10,
        postedDate: '2024-01-15',
        jobType: 'Full-time',
        experience: '0-2 năm'
      },
      {
        id: 205,
        title: 'Graphic Designer',
        company: 'Công ty TNHH Sáng tạo JKL',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '9 - 16 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 7,
        postedDate: '2024-01-14',
        jobType: 'Full-time',
        experience: '1-2 năm'
      },
      {
        id: 206,
        title: 'Nhân viên Telesales',
        company: 'Công ty CP Viễn thông MNO',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '7 - 12 triệu',
        location: 'Lào Cai',
        urgent: true,
        daysLeft: 6,
        postedDate: '2024-01-14',
        jobType: 'Full-time',
        experience: '0-1 năm'
      },
      {
        id: 207,
        title: 'Lập trình viên PHP',
        company: 'Công ty TNHH Phần mềm PQR',
        logo: 'assets/vieclamlaocai/img/image 16.png',
        salary: '15 - 28 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 12,
        postedDate: '2024-01-13',
        jobType: 'Full-time',
        experience: '2-4 năm'
      },
      {
        id: 208,
        title: 'Trưởng phòng Kinh doanh',
        company: 'Công ty CP Đầu tư STU',
        logo: 'assets/vieclamlaocai/img/image 23.png',
        salary: '25 - 45 triệu',
        location: 'Lào Cai',
        urgent: false,
        daysLeft: 15,
        postedDate: '2024-01-12',
        jobType: 'Full-time',
        experience: '5+ năm'
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
      },
      {
        id: 103,
        name: 'Công ty TNHH Marketing DEF',
        logo: 'assets/vieclamlaocai/img/image 21.png',
        jobCount: 8,
        industry: 'Marketing - Quảng cáo',
        size: '20-50 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Dịch vụ marketing và quảng cáo trực tuyến',
        website: 'https://defmarketing.vn'
      },
      {
        id: 104,
        name: 'Công ty CP Sản xuất GHI',
        logo: 'assets/vieclamlaocai/img/image 18.png',
        jobCount: 10,
        industry: 'Sản xuất - Chế biến',
        size: '200-500 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Sản xuất và chế biến nông sản',
        website: 'https://ghimanufacturing.vn'
      },
      {
        id: 105,
        name: 'Công ty TNHH Xây dựng JKL',
        logo: 'assets/vieclamlaocai/img/image 22.png',
        jobCount: 14,
        industry: 'Xây dựng - Kiến trúc',
        size: '100-200 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Thi công xây dựng dân dụng và công nghiệp',
        website: 'https://jklconstruction.vn'
      },
      {
        id: 106,
        name: 'Công ty CP Du lịch MNO',
        logo: 'assets/vieclamlaocai/img/image 19.png',
        jobCount: 7,
        industry: 'Du lịch - Khách sạn',
        size: '50-100 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Dịch vụ du lịch và khách sạn cao cấp',
        website: 'https://mnotourism.vn'
      },
      {
        id: 107,
        name: 'Công ty TNHH Logistics PQR',
        logo: 'assets/vieclamlaocai/img/image 18.png',
        jobCount: 9,
        industry: 'Vận tải - Logistics',
        size: '50-100 nhân viên',
        location: 'Lào Cai',
        verified: true,
        description: 'Dịch vụ vận chuyển và logistics chuyên nghiệp',
        website: 'https://pqrlogistics.vn'
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





