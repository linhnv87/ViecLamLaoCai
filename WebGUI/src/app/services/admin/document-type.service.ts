import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root',
})
export class DocumentTypeService {
  private baseUrl = `${environment.apiUrl}/DocumentTypes`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<BaseResponseModel<DocumentTypeModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<DocumentTypeModel[]>>(apiUrl);
  }
   getById(id: number): Observable<BaseResponseModel<DocumentTypeModel>> {
      const apiUrl = `${this.baseUrl}/GetById/${id}`;
      return this.http.get<BaseResponseModel<DocumentTypeModel>>(apiUrl);
    }
  
    create(payload: DocumentTypeModel): Observable<BaseResponseModel<DocumentTypeModel>> {
      const apiUrl = `${this.baseUrl}/Create`;
      return this.http.post<BaseResponseModel<DocumentTypeModel>>(apiUrl, payload);
    }
  
    update(payload: DocumentTypeModel): Observable<BaseResponseModel<DocumentTypeModel>> {
      const apiUrl = `${this.baseUrl}/Update`;
      return this.http.put<BaseResponseModel<DocumentTypeModel>>(apiUrl, payload);
    }
  
    delete(id: number): Observable<BaseResponseModel<number>> {
      const apiUrl = `${this.baseUrl}/Delete/${id}`;
      return this.http.delete<BaseResponseModel<number>>(apiUrl);
    }
    updateOrder(orderList: { id: number; order: number }[]): Observable<BaseResponseModel<boolean>> {
      const apiUrl = `${this.baseUrl}/update-order`;
      return this.http.post<BaseResponseModel<boolean>>(apiUrl, orderList);
    }
    
}
