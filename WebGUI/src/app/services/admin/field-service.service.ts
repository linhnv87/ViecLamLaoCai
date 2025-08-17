import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';

import { FieldModel } from 'src/app/models/field.model';

import { environment } from 'src/environments/environments';
@Injectable({
  providedIn: 'root'
})
export class FieldServiceService {

  private baseUrl = `${environment.apiUrl}/Field`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<BaseResponseModel<FieldModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<FieldModel[]>>(apiUrl);
  }
  getById(id: number): Observable<BaseResponseModel<FieldModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<FieldModel>>(apiUrl);
  }

  create(payload: FieldModel): Observable<BaseResponseModel<FieldModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<FieldModel>>(apiUrl, payload);
  }

  update(payload: FieldModel): Observable<BaseResponseModel<FieldModel>> {
    const apiUrl = `${this.baseUrl}/Update`;
    return this.http.put<BaseResponseModel<FieldModel>>(apiUrl, payload);
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
