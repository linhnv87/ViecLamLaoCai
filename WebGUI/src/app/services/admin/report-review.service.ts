import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import {
  DocumentReport,
  ReportReviewRequestModel,
  ReportReviewResponseModel,
} from 'src/app/models/report-review.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root',
})
export class ReportReviewService {
  private baseUrl = `${environment.apiUrl}/Report`;
  constructor(private http: HttpClient) {}

  getAllYKien(
    payload: ReportReviewRequestModel,
  ): Observable<BaseResponseModel<ReportReviewResponseModel>> {
    const url = this.getUrlParams(`${this.baseUrl}/ApprovalStatistic`, payload);

    return this.http.get<BaseResponseModel<ReportReviewResponseModel>>(url);
  }

  exportFileYKien(payload: ReportReviewRequestModel): Observable<Blob> {
    const url = this.getUrlParams(`${this.baseUrl}/ApprovalStatistic/Export`, payload);
    return this.http.get(url, { responseType: 'blob' });
  }

  getAllToTrinh(
    payload: ReportReviewRequestModel,
  ): Observable<BaseResponseModel<ReportReviewResponseModel>> {
    const url = this.getUrlParams(`${this.baseUrl}/DocumentApproval`, payload);

    return this.http.get<BaseResponseModel<ReportReviewResponseModel>>(url);
  }

  exportFileToTrinh(payload: ReportReviewRequestModel): Observable<Blob> {
    const url = this.getUrlParams(`${this.baseUrl}/DocumentApproval/Export`, payload);
    return this.http.get(url, { responseType: 'blob' });
  }

  exportFile(payload: ReportReviewRequestModel): Observable<Blob> {
    const url = this.getUrlParams(`${this.baseUrl}/review/ExportBlob`, payload);
    return this.http.get(url, { responseType: 'blob' });
  }

  getListDocumentById(userId: string, type: string, fromDate?: string | null, toDate?: string | null): Observable<BaseResponseModel<DocumentReport[]>> {
    let params: any = {};
  
    if (fromDate) params.fromDate = fromDate;
    if (toDate) params.toDate = toDate;
  
    return this.http.get<BaseResponseModel<DocumentReport[]>>(
      `${this.baseUrl}/${userId}/Documents/${type}`,
      { params }
    );
  }
  
  getUrlParams(url: string, payload: ReportReviewRequestModel): string {
    let apiUrl = `${url}/?PageNumber=${payload.pageNumber}&PageSize=${payload.pageSize}`;
    if (payload.keyword) {
      apiUrl += `&Keyword=${payload.keyword}`;
    }
    if (payload.fromDate) {
      apiUrl += `&FromDate=${payload.fromDate}`;
    }
    if (payload.toDate) {
      apiUrl += `&ToDate=${payload.toDate}`;
    }
    if (payload.fileName) {
      apiUrl += `&FileName=${payload.fileName}`;
    }
    if (payload.sheetName) {
      apiUrl += `&SheetName=${payload.sheetName}`;
    }
    if (payload.type) {
      apiUrl += `&Type=${payload.type}`;
    }
    return apiUrl;
  }
}
