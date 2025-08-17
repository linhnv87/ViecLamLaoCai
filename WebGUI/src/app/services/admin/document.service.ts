import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { Observable, of } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentRetrievalModel } from 'src/app/models/documentHistory.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { environment } from 'src/environments/environments';
import { Guid } from 'guid-typescript';
import { GetCurrentUserId, GetCurrentUserRoles } from 'src/app/utils/commonFunctions';
import { ApprovalItemModel } from 'src/app/models/documentApproval.model';
import { DATE_FORMAT, ROLES } from 'src/app/utils/constants';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
  private baseUrl = `${environment.apiUrl}/Document`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<BaseResponseModel<DocumentModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<DocumentModel[]>>(apiUrl);
  }

  getAllDocumentsByStatusCode(
    statusCode: string | null,
  ): Observable<BaseResponseModel<DocumentModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllDocument/${statusCode}`;
    return this.http.get<BaseResponseModel<DocumentModel[]>>(apiUrl);
  }

  uploadDocument(
    model: Blob,
    userName: string,
    currentUserId: Guid,
    docId: number,
  ): Observable<any> {
    const formData = new FormData();
    formData.append('document', new File([model], `${userName}.pdf`, { type: model.type }));
    formData.append('title', `${userName}.pdf`);
    formData.append('id', docId.toString());
    formData.append('modifiedBy', currentUserId.toString());
    formData.append('createdBy', currentUserId.toString());
    return this.http.post(`${this.baseUrl}/UploadDocument`, formData);
  }

  getDocumentsById(id: number): Observable<BaseResponseModel<DocumentModel>> {
    const currentUserId = GetCurrentUserId();
    const apiUrl = `${this.baseUrl}/GetDocumentById/${id}/?assigneeId=${currentUserId}`;
    return this.http.get<BaseResponseModel<DocumentModel>>(apiUrl);
  }
  getDocumentsByIdItem(id: number): Observable<BaseResponseModel<DocumentModel>> {
    const currentUserId = GetCurrentUserId();
    const apiUrl = `${this.baseUrl}/GetDocumentById/${id}`;
    return this.http.get<BaseResponseModel<DocumentModel>>(apiUrl);
  }
  create(
    payload: DocumentModel,
    files: File[],
    sideFiles: File[],
  ): Observable<BaseResponseModel<DocumentModel>> {
    const formData = new FormData();
    payload.dateEndApproval = payload.dateEndApproval;
    payload.created = payload.created;

    for (let i = 0; i < files.length; i++) {
      // formData.append('files', files[i]);
      formData.append('mainFiles', files[i]);
    }
    for (let i = 0; i < sideFiles.length; i++) {
      // formData.append('files', files[i]);
      formData.append('sideFiles', sideFiles[i]);
    }
    formData.append('title', payload.title!);
    formData.append('note', payload.note!);
    formData.append('fieldId', payload.fieldId?.toString() || '');
    formData.append('typeId', payload.typeId?.toString() || '');
    payload.assignedRoles?.forEach(x => {
      formData.append('assignedRoles', x || '');
    });

    formData.append(
      'dateEndApproval',
      moment(payload.dateEndApproval!).format(DATE_FORMAT.FULL_DATE_TO_BE),
    );

    formData.append(
      'remindDatetime',
      moment(payload.remindDatetime!).format(DATE_FORMAT.FULL_DATE_TO_BE),
    );
    formData.append('statusCode', payload.statusCode + '');
    formData.append('modified', moment(payload.modified!).format(DATE_FORMAT.FULL_DATE_TO_BE));
    formData.append('modifiedBy', payload.modifiedBy + '');
    formData.append('created', moment(payload.created!).format(DATE_FORMAT.FULL_DATE_TO_BE));
    formData.append('createdBy', payload.createdBy + '');
    formData.append('deleted', payload.deleted + '');

    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<DocumentModel>>(apiUrl, formData);
  }

  GDSignedFile(payload: DocumentFileModel): Observable<BaseResponseModel<DocumentFileModel>> {
    const apiUrl = `${this.baseUrl}/GDSignedFile`;
    return this.http.post<BaseResponseModel<DocumentFileModel>>(apiUrl, payload);
  }
  GDSignedForceFile(payload: DocumentFileModel): Observable<BaseResponseModel<DocumentFileModel>> {
    const apiUrl = `${this.baseUrl}/GDSignedFileForce`;
    return this.http.post<BaseResponseModel<DocumentFileModel>>(apiUrl, payload);
  }

  signedFile(docId: number, userId: string): Observable<BaseResponseModel<any>> {
    const apiUrl = `${this.baseUrl}/SignedFile?docId=${docId}&userId=${userId}`;
    return this.http.post<BaseResponseModel<any>>(apiUrl, '');
  }

  updatePriorityDocument(
    docId: number,
    priorityNumber: number,
  ): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/UpdatePriorityDocument?docId=${docId}&priorityNumber=${priorityNumber}`;
    return this.http.post<BaseResponseModel<number>>(apiUrl, '');
  }

  update(
    payload: DocumentModel,
    files?: File[],
    sideFiles?: File[],
  ): Observable<BaseResponseModel<DocumentModel>> {
    const formData = this.prepareFormData(payload, files, sideFiles);
    const apiUrl = `${this.baseUrl}/Update`;

    return this.http.put<BaseResponseModel<DocumentModel>>(apiUrl, formData);
  }

  publish(docId: number, files: File[]): Observable<BaseResponseModel<number>> {
    const formData = new FormData();

    for (let i = 0; i < files.length; i++) {
      formData.append('files', files[i]);
    }
    const apiUrl = `${this.baseUrl}/Publish/${docId}`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, formData);
  }

  retrieve(payload: DocumentRetrievalModel): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/RetrieveDocument`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, payload);
  }

  returnDoc(payload: DocumentRetrievalModel): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/ReturnDocument`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, payload);
  }

  updateStatus(
    docId: number,
    status: number,
    handler: number,
  ): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/UpdateStatus/${docId}/${status}/${handler}`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, null);
  }

  delete(docId: number): Observable<BaseResponseModel<number>> {
    const apiUrl = `${this.baseUrl}/Delete/${docId}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }

  getPieChartData(): Observable<BaseResponseModel<any>> {
    const apiUrl = `${this.baseUrl}/GetDocumentsPie`;
    return this.http.get<BaseResponseModel<any>>(apiUrl);
  }

  getMonthChart(): Observable<BaseResponseModel<any>> {
    const apiUrl = `${this.baseUrl}/GetDocumentsChartMonth`;
    return this.http.get<BaseResponseModel<any>>(apiUrl);
  }

  getFile(filePath: string): Observable<Blob> {
    const apiUrl = `${this.baseUrl}/GetFile`;
    return this.http.post(apiUrl, { filePath: filePath }, { responseType: 'blob' });
  }

  // NEW API
  getList(
    status?: string,
    isCounting: boolean = false,
  ): Observable<BaseResponseModel<DocumentModel[]>> {
    const currentUserId = GetCurrentUserId();
    const params = new URLSearchParams({
      CurrentUserId: currentUserId,
      ...(status && { Status: status }),
      ...(isCounting && { IsCounting: 'true' }),
    }).toString();
    const apiUrl = `${this.baseUrl}/GetDocumentList?${params}`;
    return this.http.get<BaseResponseModel<DocumentModel[]>>(apiUrl);
  }

  getDocumentApprovalsById(docId: number): Observable<BaseResponseModel<ApprovalItemModel[]>> {
    const apiUrl = `${this.baseUrl}/GetDocumentApprovals/${docId}`;
    return this.http.get<BaseResponseModel<ApprovalItemModel[]>>(apiUrl);
  }

  GetDocumentAttachmentsById(docId: number): Observable<BaseResponseModel<DocumentFileModel[]>> {
    const apiUrl = `${this.baseUrl}/GetDocumentAttachments/${docId}`;
    return this.http.get<BaseResponseModel<DocumentFileModel[]>>(apiUrl);
  }

  createAndSend(
    payload: DocumentModel,
    files: File[],
    sideFiles: File[],
  ): Observable<BaseResponseModel<DocumentModel>> {
    const formData = this.prepareFormData(payload, files, sideFiles);
    const apiUrl = `${this.baseUrl}/CreateAndSend`;
    return this.http.post<BaseResponseModel<DocumentModel>>(apiUrl, formData);
  }

  createDraft(
    payload: DocumentModel,
    files: File[],
    sideFiles: File[],
  ): Observable<BaseResponseModel<DocumentModel>> {
    payload.assigneeId = GetCurrentUserId();
    const formData = this.prepareFormData(payload, files, sideFiles);
    const apiUrl = `${this.baseUrl}/CreateDraft`;
    return this.http.post<BaseResponseModel<DocumentModel>>(apiUrl, formData);
  }

  prepareFormData(payload: DocumentModel, files?: File[], sideFiles?: File[]): FormData {
    const formData = new FormData();
    payload.dateEndApproval = payload.dateEndApproval;
    payload.created = payload.created;
    files?.forEach(f => {
      formData.append('mainFiles', f);
    });
    sideFiles?.forEach(f => {
      formData.append('sideFiles', f);
    });

    if (payload.statusCode) {
      formData.append('statusCode', payload.statusCode as string);
    }
    formData.append('id', payload.id?.toString() || '');
    formData.append('title', payload.title!);
    formData.append('note', payload.note!);
    formData.append('fieldId', payload.fieldId?.toString() || '');
    formData.append('typeId', payload.typeId?.toString() || '');
    payload.users?.forEach(x => {
      formData.append('users', x || '');
    });
    payload.usersSMS?.forEach(x => {
      formData.append('usersSMS', x || '');
    });
    if (payload.assigneeId) {
      formData.append('assigneeId', payload.assigneeId);
    }

    formData.append(
      'dateEndApproval',
      moment(payload.dateEndApproval!).format(DATE_FORMAT.FULL_DATE_TO_BE),
    );

    formData.append(
      'remindDatetime',
      payload.remindDatetime
        ? moment(payload.remindDatetime).format(DATE_FORMAT.FULL_DATE_TO_BE)
        : ''
    );  
    formData.append('modified', moment(payload.modified!).format(DATE_FORMAT.FULL_DATE_TO_BE));
    formData.append('modifiedBy', payload.modifiedBy + '');
    formData.append('created', moment(payload.created!).format(DATE_FORMAT.FULL_DATE_TO_BE));
    formData.append('createdBy', payload.createdBy + '');
    formData.append('deleted', payload.deleted + '');
    return formData;
  }

  resultSignedFiles(docId: number, submitCount: number): Observable<BaseResponseModel<any>> {
    const userId = GetCurrentUserId();
    const apiUrl = `${this.baseUrl}/ResultSignedFiles?docId=${docId}&userId=${userId}&submitCount=${submitCount}`;
    return this.http.post<BaseResponseModel<any>>(apiUrl, {});
  }
  printResult(docId: number, comment: string): Observable<Blob> {
    const userId = GetCurrentUserId();
    const apiUrl = `${this.baseUrl}/PrintResult?docId=${docId}&userId=${userId}&comment=${comment}`;
    return this.http.post(apiUrl, {}, { responseType: 'blob' });
  }
  getListAdmin(status?: string): Observable<BaseResponseModel<DocumentModel[]>> {
    let apiUrl = `${this.baseUrl}/GetDocumentList`;
    if (status) {
      apiUrl += `?Status=${status}`;
    }
    return this.http.get<BaseResponseModel<DocumentModel[]>>(apiUrl);
  }
  processApproving(): Observable<BaseResponseModel<boolean>> {
    let apiUrl = `${this.baseUrl}/ProcessApproving`;
    return this.http.get<BaseResponseModel<boolean>>(apiUrl);
  }
}
