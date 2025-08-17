import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { SMSLogGroupModel, SMSLogModel, SMSLogQueryModel } from 'src/app/models/sms-log.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})
export class SmsLogService {
 private baseUrl = `${environment.apiUrl}/SMS`
  constructor(private http: HttpClient) { }
  getAllLogsWithUserNames(queryModel: SMSLogQueryModel): Observable<BaseResponseModel<SMSLogGroupModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllLogsWithUserNames`;
    return this.http.post<BaseResponseModel<SMSLogGroupModel[]>>(apiUrl, queryModel);
  }
}
