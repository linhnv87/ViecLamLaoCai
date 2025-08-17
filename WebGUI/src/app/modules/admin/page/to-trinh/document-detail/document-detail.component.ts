import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CommentModel } from 'src/app/models/comment.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { CommentService } from 'src/app/services/admin/comment.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import * as common from 'src/app/utils/commonFunctions';
import { NgxExtendedPdfViewerService } from 'ngx-extended-pdf-viewer';
import { environment } from 'src/environments/environments';
import { DOCUMENT_STATUS } from 'src/app/utils/constants';
import { DocumentReviewService } from 'src/app/services/admin/document-review.service';

@Component({
  selector: 'app-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.scss'],
})
export class DocumentDetailComponent implements OnInit {
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private documentService: DocumentService,
    public sanitizer: DomSanitizer,
    private commentService: CommentService,
    private toastr: ToastrService,
    private pdfService: NgxExtendedPdfViewerService,
    private documentReviewService: DocumentReviewService,
  ) {}
  userRoles = common.GetRoleInfo();
  currentUserId = common.GetCurrentUserId();
  currentUserName = common.GetCurrentUserInfo().userName;
  docId: number = 0;
  newComment = '';
  comments: CommentModel[] = [];
  documentApprovals: DocumentApprovalModel[] = [];
  selectedDocument: DocumentFileModel | undefined;
  urlToView: SafeResourceUrl | undefined;
  urlToViewV2: string = '';
  document: DocumentModel | undefined;
  myApprovalStatus = '';
  myApprovalStatusId = 0;
  myApprovalId = 0;

  groupedApprovals: any[] = [];
  groupedFiles: any[] = [];

  showChangeAction = false;
  documentStatus = DOCUMENT_STATUS;
  // NEW DATA
  badgeKetQuaYKien = '';
  public selectedDocumentId?: number;
  isFinal = false;
  listAttachment: DocumentFileModel[] = [];
  listAttachmentFilter: DocumentFileModel[] = [];
  assigneeId = '';
  biThuKySoWithStatusXinYKien = false;

  ngOnInit(): void {
    this.assigneeId = this.route.snapshot.queryParams?.['assigneeId'];
    this.loadData();
  }

  loadData() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (!!id) {
        this.docId = id;
        this.readDocument();
        this.getListAttachment();
        this.documentService.getDocumentsById(id).subscribe(res => {
          if (res.isSuccess) {
            this.document = res.result as DocumentModel;
          }
        });
        this.loadComments(id);
      }
    });
  }
  readDocument() {
    this.documentReviewService.viewDocument(this.docId).subscribe();
  }

  getListAttachment() {
    this.documentService.GetDocumentAttachmentsById(this.docId).subscribe(res => {
      if (res.isSuccess) {
        this.listAttachment = res.result;
        if (this.listAttachment?.length) {
          this.listAttachment.forEach(x => {
            x.filePath = common.GetFullFilePath(x.filePath);
            x.filePathToView = common.GetFullFilePath(x.filePathToView!);
          });
          this.listAttachmentFilter = this.listAttachment.filter(x => x.fileType !== null);
          this.selectedDocument = this.listAttachmentFilter[0];
          this.selectedDocumentId = this.selectedDocument.id;
          this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(
            this.selectedDocument?.filePathToView!,
          );
          this.getFileToShow(this.selectedDocument?.filePathToView!);
        }
      }
    });
  }

  getFileToShow(path: string) {
    var serverPath = path.replace(environment.hostUrl, 'wwwroot');
    this.documentService.getFile(serverPath).subscribe(res => {
      let blob: Blob = res as Blob;
      let url = window.URL.createObjectURL(blob);
      this.urlToViewV2 = url;
    });
  }

  loadComments(docId: number) {
    this.commentService.getDocumentCommnets(docId).subscribe(res => {
      if (res.isSuccess) {
        this.comments = res.result as CommentModel[];
        this.comments = this.comments.filter(
          x =>
            !(
              x.comment?.includes('[Duyệt kèm ý kiến bổ sung]') ||
              x.comment?.includes('[Từ chối duyệt]')
            ),
        );
      }
    });
  }

  viewFile_V2(file: DocumentFileModel) {
    this.selectedDocument = file;
    if (this.selectedDocumentId !== file.id) {
      this.selectedDocumentId = file.id;
    }
    this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(
      this.selectedDocument?.filePathToView!,
    );
    this.getFileToShow(this.selectedDocument?.filePathToView!);
  }

  saveDocument() {
    const that = this;
    this.pdfService.getCurrentDocumentAsBlob().then(data => {
      const blob = new Blob([data], { type: 'application/pdf' });
      that.documentService
        .uploadDocument(blob, that.currentUserName, that.currentUserId, that.docId)
        .subscribe(res => {
          if (res.success) {
            console.log('save document success');
          }
        });
    });
  }

  SignFileCallBack(rv: any) {
    var received_msg = JSON.parse(rv);
    console.log(received_msg);

    if (received_msg.Status == 0) {
      const url = received_msg.FileServer;
      const filenameMatch = url.match(/\/Upload\/(.*)/);
      const filename = filenameMatch[1];
      var payload: DocumentFileModel = {
        id: 0,
        fileName: filename,
        filePath: '\\Files\\Document_Attachments\\' + filename,
        docId: this.docId,
        version: 1,
        userId: this.currentUserId,
        modified: new Date(),
        deleted: false,
        modifiedBy: this.currentUserId,
        createdBy: this.currentUserId,
        created: new Date(),
        filePathToView: '\\Files\\Document_Attachments\\' + filename,
        isFinal: true,
        fileType: 3,
      };

      const ob$ = this.biThuKySoWithStatusXinYKien
        ? this.documentService.GDSignedForceFile(payload)
        : this.documentService.GDSignedFile(payload);
      ob$.subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công');
          this.navigateToComponentBWithParam();
        } else {
          this.toastr.error(res.message);
        }
      });
    } else {
      this.toastr.error(received_msg.Message, 'Thông báo từ phần mềm ký số');
    }
  }

  navigateToComponentBWithParam() {
    this.route.paramMap.subscribe(param => {
      const paramValue = param.get('statusCode');
      this.router.navigate(['/admin/to-trinh', paramValue]);
    });
  }

  // new code
  changeDocument() {
    const currentDocument = this.getCurrentDocumentById();
    if (currentDocument) {
      this.viewFile_V2(currentDocument);
    }
  }

  dowloadFile() {
    const currentDocument = this.getCurrentDocumentById();
    if (currentDocument) {
      window.open(currentDocument.filePath, '_blank');
    }
  }

  getCurrentDocumentById() {
    return this.listAttachment?.find(f => f.id === Number(this.selectedDocumentId));
  }

  onKySoGD() {
    this.biThuKySoWithStatusXinYKien =
      this.userRoles.biThu &&
      (this.document?.statusCode === DOCUMENT_STATUS.XIN_Y_KIEN ||
        this.document?.statusCode === DOCUMENT_STATUS.XIN_Y_KIEN_LAI);
    this.isFinal = true;
    const that = this;
    const currentDocument = this.getCurrentDocumentById();
    const fullPath = currentDocument?.filePathToView;
    const webSocket = new WebSocket('wss://127.0.0.1:8987/Config');
    webSocket.onopen = function (e) {
      const prms = {
        FileName: fullPath?.replace('//Files', '/Files'),
        FileUploadHandler: common.GetFullFilePathSign('FileUpload.aspx'),
      };
      const json_prms = JSON.stringify(prms);
      vgca_sign_approved(json_prms, that.SignFileCallBack.bind(that));
    };
  }

  shouldShowTab(): boolean {
    const restrictedRoles = ['banChapHanh', 'banThuongVu'];
    if (this.document?.statusCode === DOCUMENT_STATUS.XIN_Y_KIEN) {
      return true;
    }
    const userRoles = Object.keys(this.userRoles).filter(
      role => (this.userRoles as Record<string, boolean>)[role]
    );
  
    const hasNonRestrictedRole = userRoles.some(role => !restrictedRoles.includes(role));
    return hasNonRestrictedRole;
  }
  
}
