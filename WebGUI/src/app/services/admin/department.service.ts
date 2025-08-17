import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DepartmentModel } from 'src/app/models/department.model';
import { GetAllInfoUserModel, UserLogInModel } from 'src/app/models/user.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private baseUrl = `${environment.apiUrl}/Department`;
  constructor(private http: HttpClient) { }

  getAll(): Observable<BaseResponseModel<DepartmentModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllDepartments`;
    return this.http.get<BaseResponseModel<DepartmentModel[]>>(apiUrl);
  }

  getUserOfDepartment(departmentId: number): Observable<BaseResponseModel<GetAllInfoUserModel[]>> {
    const apiUrl = `${this.baseUrl}/${departmentId}`;
    return this.http.get<BaseResponseModel<GetAllInfoUserModel[]>>(apiUrl);
  }
  getById(id: number): Observable<BaseResponseModel<DepartmentModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<DepartmentModel>>(apiUrl);
  }

  create(payload: DepartmentModel): Observable<BaseResponseModel<DepartmentModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<DepartmentModel>>(apiUrl, payload);
  }

  update(payload: DepartmentModel): Observable<BaseResponseModel<DepartmentModel>> {
    const apiUrl = `${this.baseUrl}/Update`;
    return this.http.put<BaseResponseModel<DepartmentModel>>(apiUrl, payload);
  }

  delete(id: number): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }
}
