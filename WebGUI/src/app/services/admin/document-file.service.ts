import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { environment } from 'src/environments/environments';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class DocumentFileService {

  private baseUrl = `${environment.apiUrl}/DocumentFile`;

  constructor(private http: HttpClient) { }

  delete(id: number) : Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${id}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }

  create(payload: DocumentFileModel): Observable<BaseResponseModel<DocumentFileModel>> {
    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<DocumentFileModel>>(apiUrl,payload);
  }
  addRelatedDocument(DocId:number,modifiedBy:string,sideFiles: File[]): Observable<BaseResponseModel<DocumentFileModel[]>> {
    const apiUrl = `${this.baseUrl}/add-related-document-file`;
    const formData: FormData = new FormData();
    formData.append('DocId', DocId.toString());
    sideFiles?.forEach(f => {
      formData.append('sideFiles', f);
    });
    formData.append('modifiedBy', modifiedBy + '');
    return this.http.post<BaseResponseModel<DocumentFileModel[]>>(apiUrl, formData);
  }
}
