import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentHistoryModel } from 'src/app/models/documentHistory.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DocumentHistoryService {

  private baseUrl = `${environment.apiUrl}/DocumentHistory`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<BaseResponseModel<DocumentHistoryModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<DocumentHistoryModel[]>>(apiUrl);
  }
  getByDocumentId(docId: number): Observable<BaseResponseModel<DocumentHistoryModel[]>> {
    const apiUrl = `${this.baseUrl}/GetDocumentHistory/${docId}`;
    return this.http.get<BaseResponseModel<DocumentHistoryModel[]>>(apiUrl);
  }
  getByUserId(userId: string): Observable<BaseResponseModel<DocumentHistoryModel[]>> {
    const apiUrl = `${this.baseUrl}/GetDocumentHistoryByUser/${userId}`;
    return this.http.get<BaseResponseModel<DocumentHistoryModel[]>>(apiUrl);
  }
  getById(id: number): Observable<BaseResponseModel<DocumentHistoryModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<DocumentHistoryModel>>(apiUrl);
  }

  create(payload: DocumentHistoryModel): Observable<BaseResponseModel<DocumentHistoryModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<DocumentHistoryModel>>(apiUrl, payload);
  }

  // update(payload: DocumentHistoryModel): Observable<BaseResponseModel<DocumentHistoryModel>> {
  //   const apiUrl = `${this.baseUrl}/Update`;
  //   return this.http.put<BaseResponseModel<DocumentHistoryModel>>(apiUrl, payload);
  // }

  delete(id: number): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }
}
