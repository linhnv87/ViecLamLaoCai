import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentModel } from 'src/app/models/document.model';
import { environment } from 'src/environments/environments';
import { ToTrinhPayloadModel } from 'src/app/models/to-trinh.model';

@Injectable({
  providedIn: 'root',
})
export class ToTrinhService {
  private baseUrl = `${environment.apiUrl}/ToTrinh`;

  constructor(private http: HttpClient) { }

  update(payload: ToTrinhPayloadModel, files?: File[]): Observable<BaseResponseModel<DocumentModel>> {
    const formData = this.prepareFormData(payload, files);
    return this.http.put<BaseResponseModel<DocumentModel>>(`${this.baseUrl}/Update`, formData);
  }

  reUpdate(payload: ToTrinhPayloadModel, files?: File[]): Observable<BaseResponseModel<DocumentModel>> {
    const formData = this.prepareFormData(payload, files);
    return this.http.put<BaseResponseModel<DocumentModel>>(`${this.baseUrl}/ReUpdate`, formData);
  }

  prepareFormData(payload: ToTrinhPayloadModel, files?: File[]): FormData {
    const formData = new FormData();
    formData.append('documentId', payload.documentId?.toString() || '');
    formData.append('userId', payload.userId?.toString() || '');
    formData.append('fromStatusCode', payload.fromStatusCode?.toString() || '');
    formData.append('toStatusCode', payload.toStatusCode?.toString() || '');
    formData.append('comment', payload.comment?.toString() || '');
    payload.users?.forEach(x => {
      formData.append('users', x || '');
    });
    files?.forEach(f => {
      formData.append('attachmentFiles', f);
    });

    return formData;
  }
}
