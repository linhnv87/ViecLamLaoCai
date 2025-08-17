import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { UnitModel } from 'src/app/models/unit.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})

export class UnitService {
  private baseUrl = `${environment.apiUrl}/Unit`;
  constructor(private http: HttpClient) { }

  getAll(): Observable<BaseResponseModel<UnitModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<UnitModel[]>>(apiUrl);
  }
  getById(id: number): Observable<BaseResponseModel<UnitModel>> {
    const apiUrl = `${this.baseUrl}/GetById/${id}`;
    return this.http.get<BaseResponseModel<UnitModel>>(apiUrl);
  }

  create(payload: UnitModel): Observable<BaseResponseModel<UnitModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<UnitModel>>(apiUrl, payload);
  }

  update(payload: UnitModel): Observable<BaseResponseModel<UnitModel>> {
    const apiUrl = `${this.baseUrl}/Update`;
    return this.http.put<BaseResponseModel<UnitModel>>(apiUrl, payload);
  }

  delete(id: number): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }
}
