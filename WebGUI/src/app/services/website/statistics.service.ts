import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from '../../models/baseResponse.model';
import { environment } from '../../../environments/environments';

export interface StatisticsOverview {
  totalBusinesses: number;
  totalJobPostings: number;
  totalApplications: number;
  totalCandidates: number;
  pendingApprovals: number;
  activeJobs: number;
  monthlyGrowth: number;
  yearlyGrowth: number;
}

export interface TrendData {
  month: string;
  value: number;
  change: number;
}

export interface SearchTrendData {
  keyword: string;
  count: number;
  trend: 'up' | 'down' | 'stable';
  percentage: number;
}

export interface IndustryStatistic {
  name: string;
  businessCount: number;
  jobCount: number;
  percentage: number;
  color: string;
}

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private baseUrl = `${environment.apiUrl}/Statistics`;

  constructor(private http: HttpClient) {}

  // GET /api/Statistics/GetOverview
  getOverview(): Observable<BaseResponseModel<StatisticsOverview>> {
    const apiUrl = `${this.baseUrl}/GetOverview`;
    return this.http.get<BaseResponseModel<StatisticsOverview>>(apiUrl);
  }

  // GET /api/Statistics/GetBusinessTrends
  getBusinessTrends(timeRange: string = 'last12months', industry: string = 'all'): Observable<BaseResponseModel<TrendData[]>> {
    const apiUrl = `${this.baseUrl}/GetBusinessTrends?timeRange=${timeRange}&industry=${industry}`;
    return this.http.get<BaseResponseModel<TrendData[]>>(apiUrl);
  }

  // GET /api/Statistics/GetApplicationTrends
  getApplicationTrends(timeRange: string = 'last12months'): Observable<BaseResponseModel<TrendData[]>> {
    const apiUrl = `${this.baseUrl}/GetApplicationTrends?timeRange=${timeRange}`;
    return this.http.get<BaseResponseModel<TrendData[]>>(apiUrl);
  }

  // GET /api/Statistics/GetSearchKeywords
  getSearchKeywords(limit: number = 20, timeRange: string = 'monthly'): Observable<BaseResponseModel<SearchTrendData[]>> {
    const apiUrl = `${this.baseUrl}/GetSearchKeywords?limit=${limit}&timeRange=${timeRange}`;
    return this.http.get<BaseResponseModel<SearchTrendData[]>>(apiUrl);
  }

  // GET /api/Statistics/GetIndustryStatistics
  getIndustryStatistics(): Observable<BaseResponseModel<IndustryStatistic[]>> {
    const apiUrl = `${this.baseUrl}/GetIndustryStatistics`;
    return this.http.get<BaseResponseModel<IndustryStatistic[]>>(apiUrl);
  }

  // GET /api/Statistics/ExportReport
  exportReport(format: string = 'pdf', timeRange: string = 'monthly'): Observable<Blob> {
    const apiUrl = `${this.baseUrl}/ExportReport?format=${format}&timeRange=${timeRange}`;
    return this.http.get(apiUrl, { responseType: 'blob' });
  }

  // POST /api/Statistics/RefreshData
  refreshData(): Observable<BaseResponseModel<boolean>> {
    const apiUrl = `${this.baseUrl}/RefreshData`;
    return this.http.post<BaseResponseModel<boolean>>(apiUrl, {});
  }

  // ===== DUMMY DATA METHODS (for development) =====

  getOverviewDummy(): Observable<BaseResponseModel<StatisticsOverview>> {
    const dummyData: StatisticsOverview = {
      totalBusinesses: 1247,
      totalJobPostings: 3456,
      totalApplications: 8923,
      totalCandidates: 5678,
      pendingApprovals: 23,
      activeJobs: 456,
      monthlyGrowth: 12.5,
      yearlyGrowth: 45.2
    };

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getBusinessTrendsDummy(): Observable<BaseResponseModel<TrendData[]>> {
    const dummyData: TrendData[] = [
      { month: 'T1/2024', value: 89, change: 12.5 },
      { month: 'T2/2024', value: 95, change: 6.7 },
      { month: 'T3/2024', value: 112, change: 17.9 },
      { month: 'T4/2024', value: 108, change: -3.6 },
      { month: 'T5/2024', value: 125, change: 15.7 },
      { month: 'T6/2024', value: 134, change: 7.2 },
      { month: 'T7/2024', value: 142, change: 6.0 },
      { month: 'T8/2024', value: 156, change: 9.9 },
      { month: 'T9/2024', value: 163, change: 4.5 },
      { month: 'T10/2024', value: 178, change: 9.2 },
      { month: 'T11/2024', value: 189, change: 6.2 },
      { month: 'T12/2024', value: 198, change: 4.8 }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }

  getSearchKeywordsDummy(): Observable<BaseResponseModel<SearchTrendData[]>> {
    const dummyData: SearchTrendData[] = [
      { keyword: 'Frontend Developer', count: 1245, trend: 'up', percentage: 15.2 },
      { keyword: 'Backend Developer', count: 1123, trend: 'up', percentage: 12.8 },
      { keyword: 'Full Stack', count: 987, trend: 'up', percentage: 8.5 },
      { keyword: 'UI/UX Designer', count: 856, trend: 'stable', percentage: 5.2 },
      { keyword: 'Data Analyst', count: 743, trend: 'up', percentage: 18.3 },
      { keyword: 'Marketing', count: 689, trend: 'down', percentage: -3.2 },
      { keyword: 'Sales', count: 567, trend: 'stable', percentage: 2.1 },
      { keyword: 'Kế toán', count: 456, trend: 'up', percentage: 7.8 }
    ];

    return of({
      statusCode: 200,
      isSuccess: true,
      message: 'Success',
      result: dummyData
    });
  }
}
