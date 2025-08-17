import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environments';
import { BaseResponseModel } from '../models/baseResponse.model';

import { DocumentModel } from "../models/document.model";
import { GetAllRoleModel } from "../models/role.model";

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private baseUrl = `${environment.apiUrl}/AppRole`
  constructor(private http: HttpClient) { }


  GetAllRoleInfo(): Observable<BaseResponseModel<GetAllRoleModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllRole`;
    return this.http.get<BaseResponseModel<GetAllRoleModel[]>>(apiUrl);
  }
  getById(id: string): Observable<BaseResponseModel<GetAllRoleModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<GetAllRoleModel>>(apiUrl);
  }

  create(payload: GetAllRoleModel): Observable<BaseResponseModel<GetAllRoleModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<GetAllRoleModel>>(apiUrl, payload);
  }

  update(payload: GetAllRoleModel): Observable<BaseResponseModel<GetAllRoleModel>> {
    const apiUrl = `${this.baseUrl}/Update`;
    return this.http.put<BaseResponseModel<GetAllRoleModel>>(apiUrl, payload);
  }

  delete(id: string): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }



}
