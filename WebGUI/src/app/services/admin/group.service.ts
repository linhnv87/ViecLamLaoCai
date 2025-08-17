import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { GroupDetailInputModel, GroupModel } from 'src/app/models/group.model';
import { GetAllInfoUserModel } from 'src/app/models/user.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  private baseUrl = `${environment.apiUrl}/Group`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<BaseResponseModel<GroupModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllGroups`;
    return this.http.get<BaseResponseModel<GroupModel[]>>(apiUrl);
  }

  getUserOfGroup(groupId: number): Observable<BaseResponseModel<GetAllInfoUserModel[]>> {
    const apiUrl = `${this.baseUrl}/${groupId}`;
    return this.http.get<BaseResponseModel<GetAllInfoUserModel[]>>(apiUrl);
  }

  create(input: GroupDetailInputModel): Observable<BaseResponseModel<number>> {
    return this.http.post<BaseResponseModel<number>>(this.baseUrl, { ...input });
  }

  update(id: number, input: GroupDetailInputModel): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/${id}`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, { ...input });
  }

  delete(id: number): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }
}
