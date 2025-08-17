import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environments';
import { BaseResponseModel } from '../models/baseResponse.model';
import { NotificationModel } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private baseUrl = `${environment.apiUrl}/Notification`
  
  constructor(private http: HttpClient) { }

  GetNotification(userId: string): Observable<BaseResponseModel<NotificationModel[]>>{
    const apiUrl = `${this.baseUrl}/GetNotificationsForUser/${userId}`;
    return this.http.get<BaseResponseModel<NotificationModel[]>>(apiUrl);
  }

  CreateNotification(type: number, docId: number, userId?: number): Observable<BaseResponseModel<number>>{
    const apiUrl = `${this.baseUrl}/CreateNotification/${type}/${docId}/${userId}`;
    return this.http.post<BaseResponseModel<number>>(apiUrl, null)
  }

  UpdateStatus(id: number): Observable<BaseResponseModel<number>>{
    const apiUrl = `${this.baseUrl}/UpdateNotificationStatus/${id}`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, null);
  }

  CheckNewAlert(userId: string): Observable<BaseResponseModel<number>>{
    const apiUrl = `${this.baseUrl}/CheckNewAlert/${userId}`;
    return this.http.get<BaseResponseModel<number>>(apiUrl);
  }

  SendSMS(docId: number, type: number): Observable<BaseResponseModel<boolean[]>>{
    const apiUrl = `${environment.apiUrl}/SMS/SendSMS/${docId}/${type}`;
    return this.http.get<BaseResponseModel<boolean[]>>(apiUrl)
  }
  SendSMSV2(docId: number, type: number): Observable<BaseResponseModel<boolean[]>>{
    const apiUrl = `${environment.apiUrl}/SMS/SendSMSV2/${docId}/${type}`;
    return this.http.get<BaseResponseModel<boolean[]>>(apiUrl)
  }
}
