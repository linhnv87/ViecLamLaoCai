import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { GetAllInfoUserModel } from 'src/app/models/user.model';
import { ReviewerWorkflowModel } from 'src/app/models/work-flow.model';
import { GetCurrentUserId } from 'src/app/utils/commonFunctions';
import { DOCUMENT_STATUS } from 'src/app/utils/constants';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root',
})
export class WorkFlowService {
  private baseUrl = `${environment.apiUrl}/GetReviewerWorkflow`;

  constructor(private http: HttpClient) { }

  getWorkFlowByStatus(
    status: string,
    state: string,
  ): Observable<BaseResponseModel<ReviewerWorkflowModel>> {
    const userId = GetCurrentUserId();
    let statusId = 0;
    switch (status) {
      case DOCUMENT_STATUS.DU_THAO:
        statusId = 1;
        break;
      case DOCUMENT_STATUS.XIN_Y_KIEN:
        statusId = 2;
        break;
      case DOCUMENT_STATUS.PHE_DUYET:
        statusId = 3;
        break;
      case DOCUMENT_STATUS.KY_SO:
        statusId = 4;
        break;
      case DOCUMENT_STATUS.CHO_BAN_HANH:
        statusId = 5;
        break;
      case DOCUMENT_STATUS.BAN_HANH:
        statusId = 6;
        break;
      case DOCUMENT_STATUS.KHONG_BAN_HANH:
        statusId = 7;
        break;
      case DOCUMENT_STATUS.TRA_LAI:
        statusId = 8;
        break;
      case DOCUMENT_STATUS.XIN_Y_KIEN_LAI:
        statusId = 9;
        break;
    }
    const apiUrl = `${this.baseUrl}/search`;
    return this.http.post<BaseResponseModel<ReviewerWorkflowModel>>(apiUrl, {
      statusId,
      userId,
      state,
    });
  }
  getAll(): Observable<BaseResponseModel<ReviewerWorkflowModel[]>> {
    const apiUrl = `${this.baseUrl}/getAll`;
    return this.http.get<BaseResponseModel<ReviewerWorkflowModel[]>>(apiUrl);
  }

  getGetUsersByWorkflowId(groupId: number): Observable<BaseResponseModel<GetAllInfoUserModel[]>> {
    const apiUrl = `${this.baseUrl}/GetGetUsersByWorkflowId/${groupId}`;
    return this.http.get<BaseResponseModel<GetAllInfoUserModel[]>>(apiUrl);
  }

  create(input: ReviewerWorkflowModel): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<number>>(apiUrl, { ...input });
  }

  update(id: number, input: ReviewerWorkflowModel): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/${id}`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, { ...input });
  }

  delete(id: number): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }
}
