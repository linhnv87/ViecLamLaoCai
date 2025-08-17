import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { DocumentApprovalModel, DocumentApprovalQueryModel, DocumentApprovalSummaryModel, DocumentApprovalSummaryModel_V2 } from 'src/app/models/documentApproval.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DocumentApprovalService {

  private baseUrl = `${environment.apiUrl}/DocumentApproval`;

  constructor(private http: HttpClient) { }

  GetSingleByUserIdAndDocId(userId: string, docId: number) : Observable<BaseResponseModel<DocumentApprovalModel>> {
    const apiUrl = `${this.baseUrl}/GetSingleByUserIdAndDocId/${userId}/${docId}`;
    return this.http.get<BaseResponseModel<DocumentApprovalModel>>(apiUrl);
  }
  
  CreateDocumentApproval(documentApproval : DocumentApprovalModel) : Observable<BaseResponseModel<DocumentApprovalModel>>{        
    const apiUrl = `${this.baseUrl}/CreateDocumentApproval`;
    const formData = new FormData();    
    formData.append('id', documentApproval.id! + '')
    formData.append('title', documentApproval.title!)
    formData.append('docId', documentApproval.docId! + '')
    formData.append('statusCode', documentApproval.statusCode!.toString())
    formData.append('userId', documentApproval.userId!)
    formData.append('userName', documentApproval.userName!)
    formData.append('modified', moment(documentApproval.modified!).format('YYYY-MM-DD hh:mm:ss'))
    formData.append('deleted', documentApproval.deleted! + '')
    formData.append('createdBy', documentApproval.createdBy!)
    formData.append('created', moment(documentApproval.created!).format('YYYY-MM-DD hh:mm:ss'))
    formData.append('comment', documentApproval.comment!)
    formData.append('filePath', documentApproval.filePath!)
    formData.append('attachment', documentApproval.attachment!)
    formData.append('submitCount', documentApproval.submitCount!+'')
    return this.http.post<BaseResponseModel<DocumentApprovalModel>>(apiUrl, formData);
  }

  UpdateDocumentApproval(documentApproval : DocumentApprovalModel) : Observable<BaseResponseModel<DocumentApprovalModel>>{
    const apiUrl = `${this.baseUrl}/UpdateDocumentApproval`;
    return this.http.put<BaseResponseModel<DocumentApprovalModel>>(apiUrl, documentApproval);
  }

  GetFinalPdf(docId: number) : Observable<BaseResponseModel<DocumentFileModel>>{
    const apiUrl = `${this.baseUrl}/GetFinalPdf/${docId}`;
    return this.http.get<BaseResponseModel<DocumentFileModel>>(apiUrl);
  }
  
  GetApprovalSummary(docId: number): Observable<BaseResponseModel<DocumentApprovalSummaryModel[]>>{
    const apiUrl = `${this.baseUrl}/GetApprovalSummary/${docId}`;
    return this.http.get<BaseResponseModel<DocumentApprovalSummaryModel[]>>(apiUrl);
  }

  GetApprovalSummary_V2(queries: DocumentApprovalQueryModel): Observable<BaseResponseModel<DocumentApprovalSummaryModel_V2[]>>{
    const apiUrl = `${this.baseUrl}/GetApprovalSummary_V2`;
    return this.http.put<BaseResponseModel<DocumentApprovalSummaryModel_V2[]>>(apiUrl, queries);
  }

  GetIndividualApprovalList(userId: string): Observable<BaseResponseModel<DocumentApprovalModel[]>>{
    const apiUrl = `${this.baseUrl}/GetIndividualDocumentApprovals/${userId}`;
    return this.http.get<BaseResponseModel<DocumentApprovalModel[]>>(apiUrl);
  }
}
