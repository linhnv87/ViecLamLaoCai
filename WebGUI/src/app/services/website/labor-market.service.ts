import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { MarketOverview, JobTrend, SkillDemand, SalaryRange, MarketInsight } from '../../models/labor-market.model';
import { environment } from '../../../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class LaborMarketService {
  private baseUrl = `${environment.apiUrl}/LaborMarket`;

  constructor(private http: HttpClient) {}

  // GET /api/LaborMarket/GetMarketOverview
  getMarketOverview(): Observable<BaseResponseModel<MarketOverview>> {
    const apiUrl = `${this.baseUrl}/GetMarketOverview`;
    return this.http.get<BaseResponseModel<MarketOverview>>(apiUrl);
  }

  // GET /api/LaborMarket/GetJobTrends
  getJobTrends(region: string = 'laocai', timeRange: string = 'quarterly'): Observable<BaseResponseModel<JobTrend[]>> {
    const apiUrl = `${this.baseUrl}/GetJobTrends?region=${region}&timeRange=${timeRange}`;
    return this.http.get<BaseResponseModel<JobTrend[]>>(apiUrl);
  }

  // GET /api/LaborMarket/GetSkillDemands
  getSkillDemands(category: string = 'all', level: string = 'all'): Observable<BaseResponseModel<SkillDemand[]>> {
    const apiUrl = `${this.baseUrl}/GetSkillDemands?category=${category}&level=${level}`;
    return this.http.get<BaseResponseModel<SkillDemand[]>>(apiUrl);
  }

  // GET /api/LaborMarket/GetSalaryRanges
  getSalaryRanges(industry: string = 'all', experience: string = 'all'): Observable<BaseResponseModel<SalaryRange[]>> {
    const apiUrl = `${this.baseUrl}/GetSalaryRanges?industry=${industry}&experience=${experience}`;
    return this.http.get<BaseResponseModel<SalaryRange[]>>(apiUrl);
  }

  // GET /api/LaborMarket/GetMarketInsights
  getMarketInsights(category: string = 'all', impact: string = 'all'): Observable<BaseResponseModel<MarketInsight[]>> {
    const apiUrl = `${this.baseUrl}/GetMarketInsights?category=${category}&impact=${impact}`;
    return this.http.get<BaseResponseModel<MarketInsight[]>>(apiUrl);
  }

  // GET /api/LaborMarket/ExportReport
  exportReport(format: string = 'pdf', timeRange: string = 'quarterly'): Observable<Blob> {
    const apiUrl = `${this.baseUrl}/ExportReport?format=${format}&timeRange=${timeRange}`;
    return this.http.get(apiUrl, { responseType: 'blob' });
  }

  // POST /api/LaborMarket/RefreshData
  refreshData(): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/RefreshData`;
    return this.http.post<BaseResponseModel<boolean>>(apiUrl, {});
  }

  // ===== DUMMY DATA METHODS (for development) =====

  getMarketOverviewDummy(): Observable<BaseResponseModel<MarketOverview>> {
    const dummyData: MarketOverview = {
      totalActiveJobs: 3456,
      totalCompaniesHiring: 1247,
      avgTimeToHire: 15,
      jobGrowthRate: 18.5,
      candidateCompetition: 3.2,
      salaryGrowth: 12.3
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getJobTrendsDummy(): Observable<BaseResponseModel<JobTrend[]>> {
    const dummyData: JobTrend[] = [
      {
        industry: 'Công nghệ thông tin',
        demandLevel: 'high',
        growth: 25.8,
        avgSalary: '15-35 triệu',
        topPositions: ['Frontend Developer', 'Backend Developer', 'Full Stack Developer', 'DevOps Engineer'],
        color: '#007aff'
      },
      {
        industry: 'Thương mại điện tử',
        demandLevel: 'high',
        growth: 22.3,
        avgSalary: '12-28 triệu',
        topPositions: ['Digital Marketing', 'E-commerce Manager', 'Content Creator', 'Data Analyst'],
        color: '#28a745'
      },
      {
        industry: 'Tài chính ngân hàng',
        demandLevel: 'medium',
        growth: 15.7,
        avgSalary: '18-45 triệu',
        topPositions: ['Financial Analyst', 'Risk Manager', 'Investment Advisor', 'Credit Officer'],
        color: '#ffc107'
      }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getSkillDemandsDummy(): Observable<BaseResponseModel<SkillDemand[]>> {
    const dummyData: SkillDemand[] = [
      { skill: 'JavaScript/TypeScript', demand: 1456, growth: 28.5, category: 'Programming', level: 'hot' },
      { skill: 'React/Vue.js', demand: 1234, growth: 25.2, category: 'Frontend', level: 'hot' },
      { skill: 'Node.js/Python', demand: 1123, growth: 22.8, category: 'Backend', level: 'hot' },
      { skill: 'Digital Marketing', demand: 987, growth: 20.3, category: 'Marketing', level: 'hot' },
      { skill: 'Data Analysis', demand: 856, growth: 18.7, category: 'Analytics', level: 'hot' }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }
}






