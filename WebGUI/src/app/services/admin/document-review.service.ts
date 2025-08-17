import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { environment } from 'src/environments/environments';
import { DocumentReviewModel } from 'src/app/models/documentReview.model';
import { GetCurrentUserId } from 'src/app/utils/commonFunctions';

@Injectable({
  providedIn: 'root',
})
export class DocumentReviewService {
  private baseUrl = `${environment.apiUrl}/DocumentReview`;

  constructor(private http: HttpClient) {}

  updateDocumentReview(
    documentReview: DocumentReviewModel,
  ): Observable<BaseResponseModel<DocumentReviewModel>> {
    const apiUrl = `${this.baseUrl}/UpdateDocumentReview`;
    const formData = new FormData();
    formData.append('docId', documentReview.docId! + '');
    formData.append('userId', documentReview.userId!);
    formData.append('comment', documentReview.comment!);
    formData.append('reviewResult', documentReview.reviewResult?.toString() || '');
    documentReview?.files?.forEach(f => {
      formData.append('attachment', f);
    });
    formData.append('submitCount', documentReview.submitCount! + '');
    return this.http.put<BaseResponseModel<DocumentReviewModel>>(apiUrl, formData);
  }

  GetFinalPdf(docId: number): Observable<BaseResponseModel<DocumentFileModel>> {
    const apiUrl = `${this.baseUrl}/GetFinalPdf/${docId}`;
    return this.http.get<BaseResponseModel<DocumentFileModel>>(apiUrl);
  }

  viewDocument(documentId: number): Observable<BaseResponseModel<DocumentReviewModel>> {
    return this.http.post<BaseResponseModel<DocumentReviewModel>>(this.baseUrl, {
      docId: documentId,
      userId: GetCurrentUserId(),
    });
  }
}
