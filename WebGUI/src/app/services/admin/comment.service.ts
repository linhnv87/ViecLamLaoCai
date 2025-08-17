import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { CommentModel } from 'src/app/models/comment.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  private baseUrl = `${environment.apiUrl}/Comment`;

  constructor(private http: HttpClient) { }

  createComment(comment: CommentModel): Observable<BaseResponseModel<CommentModel>> {
    var apiUrl = `${this.baseUrl}/CreateComment`;
    return this.http.post<BaseResponseModel<CommentModel>>(apiUrl, comment);
  }
  
  getDocumentCommnets(docId: number): Observable<BaseResponseModel<CommentModel[]>>{
    var apiUrl = `${this.baseUrl}/GetDocumentComments/${docId}`;
    return this.http.get<BaseResponseModel<CommentModel[]>>(apiUrl);
  }
  
}
