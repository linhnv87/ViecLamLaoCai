import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { CommentComponent } from 'src/app/components/comment/comment.component';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { HandlerSelectComponent } from 'src/app/components/handler-select/handler-select.component';
import { CommentModel } from 'src/app/models/comment.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { DocumentRetrievalModel } from 'src/app/models/documentHistory.model';
import { CommentService } from 'src/app/services/admin/comment.service';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import { DocumentFileService } from 'src/app/services/admin/document-file.service';
import { DialogService } from 'src/app/services/dialog.service';
import { ExcelExportService } from 'src/app/services/excel-export.service';
import { NotificationService } from 'src/app/services/nofitication.service';
import * as common from 'src/app/utils/commonFunctions';
import { NgxExtendedPdfViewerService } from 'ngx-extended-pdf-viewer';
import { saveAs } from 'file-saver';


import { environment } from 'src/environments/environments';
import { DocumentUploadModel } from 'src/app/models/uploadDocument.model';

@Component({
  selector: 'app-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.css']
})

export class DocumentDetailComponent implements OnInit {

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private documentFileService: DocumentFileService,
    private documentApprovalService: DocumentApprovalService,
    public sanitizer: DomSanitizer,
    private commentService: CommentService,
    private toastr: ToastrService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private pdfService: NgxExtendedPdfViewerService,
    private excelExportService: ExcelExportService,
    private dialogService: DialogService) {

  }
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

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (!!id) {
        this.docId = id;
        this.documentService.getDocumentsById(id).subscribe(res => {

          if (res.isSuccess) {
            this.document = res.result as DocumentModel;
            this.getMyApprovalData();
            if (this.document.approvals) {
              this.groupedApprovals = Object.entries(this.groupBy(this.document.approvals!, 'submitCount')).map(([submitCount, data]) => ({
                submitCount,
                data
              }));
              this.groupedApprovals = this.groupedApprovals.map((e: any) => {
                return { ...e, show: false };
              })
              try {
                this.groupedApprovals = common.fillMissingAttempts(this.groupedApprovals)
              }
              catch {

              }
              this.groupedApprovals[0].show = true;
              // this.exportApprovalExcel();        
            }

            if (this.document.documentFiles) {
              this.document.documentFiles.forEach(x => {
                x.filePath = common.GetFullFilePath(x.filePath);
                x.filePathToView = common.GetFullFilePath(x.filePathToView!);
              });
              if (this.document.documentFiles.length > 0) {
                this.selectedDocument = this.document.documentFiles[0];
                this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(this.selectedDocument?.filePathToView!);
                this.getFileToShow(this.selectedDocument?.filePathToView!);

                //Grouping
                this.groupedFiles = Object.entries(this.groupBy(this.document.documentFiles!, 'version')).map(([version, data]) => ({
                  version,
                  data
                }));
                this.groupedFiles = this.groupedFiles.sort(x => x.version).reverse();
                this.groupedFiles = this.groupedFiles.map((e: any) => {
                  return { ...e, show: false };
                })
                this.groupedFiles.forEach(e => {
                  e.data = [...e.data].sort(this.sortFile);
                })
                try {
                  this.groupedFiles = common.fillMissingVersions(this.groupedFiles)
                }
                catch {

                }

                //this.groupedFiles = this.groupedFiles.filter(x => x.version <= this.document?.submitCount!)
                this.groupedFiles[0].show = true;
                //console.log(this.groupedFiles);

              }
            }
          }
        });
        this.loadComments(id);
      }
    });
  }

  getFileToShow(path: string) {
    var serverPath = path.replace(environment.hostUrl, 'wwwroot');
    this.documentService.getFile(serverPath).subscribe(res => {
      let blob: Blob = res as Blob;
      let url = window.URL.createObjectURL(blob);
      this.urlToViewV2 = url;
    })
  }

  exportApprovalExcel() {
    console.log(this.document?.approvals);
    const transformedData: any[] = []
    this.document?.approvals?.forEach((e, i) => {
      var customObj: any = {};
      customObj['STT'] = i + 1
      customObj['Lần trình'] = e.submitCount
      customObj['Thành viên ban cán sự'] = e.userName
      customObj['Kết quả đánh giá'] = e.title
      customObj['Ý kiến bổ sung'] = e.comment && e.comment != 'undefined' ? e.comment : ''
      customObj['Đánh giá lúc'] = e.created
      transformedData.push(customObj)
    })
    console.log(transformedData);

    this.excelExportService.exportToExcel(transformedData, 'Biểu tổng hợp đánh giá', 'Tổng hợp đánh giá')
  }

  sortFile(a: any, b: any) {
    var res = 0
    if (a.fileType > b.fileType) res = -1;
    else {
      if (a.id < b.id) res = -1;
    }
    // console.log([a.fileType, b.fileType, res]);
    return res;
  }

  toggleApprovals(index: number) {
    this.groupedApprovals[index].show = !this.groupedApprovals[index].show;
  }

  toggleFiles(index: number) {
    this.groupedFiles[index].show = !this.groupedFiles[index].show;
  }

  getMyApprovalData() {
    if (this.document?.statusCode == 2) this.myApprovalStatus = 'Tờ trình đang ở trạng thái tạm lưu, vui lòng không đánh giá'
    else if (this.document?.statusCode == 4) this.myApprovalStatus = 'Tờ trình này đã được duyệt'
    else if (this.document?.statusCode == 5) this.myApprovalStatus = 'Tờ trình này đã được trả về cho chuyên viên kèm ý kiến chỉnh sửa'
    else if (this.document?.statusCode == 6) this.myApprovalStatus = 'Tờ trình này không được duyệt'
    else if (this.document?.statusCode == 8) this.myApprovalStatus = 'Tờ trình này đang chờ ra nghị quyết'
    else if (this.document?.statusCode == 9) this.myApprovalStatus = 'Tờ trình này đã ra nghị quyết'
    else if (this.document?.statusCode == 10) this.myApprovalStatus = 'Tờ trình này đã được trả lại cho chuyên viên'
    else if (this.document?.statusCode == 11) this.myApprovalStatus = 'Tờ trình này đang chờ ký số'
    else if (this.document?.statusCode == 12) this.myApprovalStatus = 'Tờ trình này đang chờ phát hành'
    else if (this.document?.statusCode == 3 || this.document?.statusCode == 7) {
      this.documentApprovalService.GetSingleByUserIdAndDocId(this.currentUserId, this.docId!).subscribe(res => {
        if (res.isSuccess && res.result != null) {
          const t = res.result as DocumentApprovalModel;
          this.myApprovalStatusId = t.statusCode!;
          this.myApprovalId = t.id!;
          this.myApprovalStatus = t.statusCode == 4 ? 'Bạn đã duyệt tờ trình này' : t.statusCode == 5 ? 'Bạn đã bổ sung ý kiến chỉnh sửa tờ trình này' : t.statusCode == 6 ? 'Bạn không duyệt tờ trình này' : '';
        }
      })
    }
  }

  loadComments(docId: number) {
    this.commentService.getDocumentCommnets(docId).subscribe(res => {
      if (res.isSuccess) {
        this.comments = res.result as CommentModel[];
        this.comments = this.comments.filter(x => !(x.comment?.includes('[Duyệt kèm ý kiến bổ sung]') || x.comment?.includes('[Từ chối duyệt]')))
      }
    })
  }

  viewFile(event: any) {
    const selectedFileId = event.target.value;
    this.selectedDocument = this.document!.documentFiles!.find(x => x.id == selectedFileId);
    this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(this.selectedDocument?.filePathToView!);
    this.getFileToShow(this.selectedDocument?.filePathToView!);
  }

  viewFile_V2(file: DocumentFileModel) {
    this.selectedDocument = file;
    this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(this.selectedDocument?.filePathToView!);
    this.getFileToShow(this.selectedDocument?.filePathToView!);
  }

  addComment() {
    if (this.newComment.length > 0) {
      const obj = {
        id: 0,
        docId: this.docId,
        comment: this.newComment,
        userId: this.currentUserId,
        userName: '',
        deleted: false,
        modified: new Date(),
        modifiedBy: this.currentUserId,
        created: new Date(),
        createdBy: this.currentUserId
      } as CommentModel;
      this.commentService.createComment(obj).subscribe(res => {
        if (res.isSuccess) {
          this.newComment = '';
          this.toastr.success('Gửi ý kiến thành công')
          this.loadComments(this.docId)
        }
        console.log(res);

      })
    }
  }
  toggleChangeAction() {
    this.showChangeAction = !this.showChangeAction;
  }

  isEditApproval = false;
  changeAction(status: number) {
    this.isEditApproval = true;
    var message = '';
    var response = '';
    if (status == 4) {
      message = `Xác nhận duyệt tờ trình ${this.document?.title}`;
      this.submitApproval(status, response, message)
    } else if (status == 5) {
      message = `Xác nhận bổ sung ý kiến chỉnh sửa cho tờ trình ${this.document?.title}, phản hồi của bạn sẽ được gửi đi`
      const modalRef = this.modalService.open(CommentComponent, { size: 'lg' });
      modalRef.componentInstance.title = 'Ý kiến bổ sung';
      modalRef.result.then(result => {
        response = result
        this.submitApproval(status, response, message)
      })
    } else if (status == 6) {
      message = `Xác nhận không duyệt tờ trình ${this.document?.title}, phản hồi của bạn sẽ được đính kèm vào phần ý kiến trao đổi`
      const modalRef = this.modalService.open(CommentComponent, { size: 'lg' });
      modalRef.componentInstance.title = 'Lý do từ chối duyệt';
      modalRef.result.then(result => {
        response = result;
        this.submitApproval(status, response, message)
      })
    }
  }

  handleAction(status: number) {
    var message = '';
    var response = '';
    if (status == 4) {
      message = `Xác nhận duyệt tờ trình ${this.document?.title}`;
      this.submitApproval(status, response, message)
    } else if (status == 5) {
      message = `Xác nhận bổ sung ý kiến chỉnh sửa cho tờ trình ${this.document?.title}, phản hồi của bạn sẽ được gửi đi`
      const modalRef = this.modalService.open(CommentComponent, { size: 'lg' });
      modalRef.componentInstance.title = 'Ý kiến bổ sung';
      modalRef.result.then(result => {
        response = result
        this.submitApproval(status, response, message)
      })
    } else if (status == 6) {
      message = `Xác nhận không duyệt tờ trình ${this.document?.title}, phản hồi của bạn sẽ được đính kèm vào phần ý kiến trao đổi`
      const modalRef = this.modalService.open(CommentComponent, { size: 'lg' });
      modalRef.componentInstance.title = 'Lý do từ chối duyệt';
      modalRef.result.then(result => {
        response = result;
        this.submitApproval(status, response, message)
      })
    }
  }

  returnDoc() {
    const docId = this.docId;
    const modalRef = this.modalService.open(CommentComponent);
    modalRef.componentInstance.title = 'Lý do trả lại';
    modalRef.componentInstance.showUpload = false;
    modalRef.result.then(result => {
      if (result) {
        console.log(result);
        var request: DocumentRetrievalModel = {
          documentId: docId,
          note: result.comment
        }
        const updateStatusSub = this.documentService.returnDoc(request).subscribe(res => {
          if (res.isSuccess) {
            this.notificationService.SendSMS(this.document?.id!, 9).subscribe(res => {
              if (res.isSuccess) {
                this.toastr.success("Tin nhắn thông báo sẽ được gửi tới cho chuyên viên");
              }
            })
            this.toastr.success('Trả lại tờ trình thảnh công');
            this.navigateToComponentBWithParam();
          } else {
            this.toastr.error(res.message);
          }
        })
      }
    })
  }

  handleAction_General(status: number) {
    var message = '';
    var that = this;
    var handler = 0;
    if (status == 10) {
      message = `Xác nhận trả lại tờ trình ${this.document?.title}? Tờ trình này sẽ được trả lại cho chuyên viên.`;
      const modalRef = this.modalService.open(CommentComponent, { size: 'lg' });
      modalRef.componentInstance.title = 'Lý do trả lại';
      modalRef.result.then(result => {
        var response = result;
        //this.submitApproval(status, response, message)
        const modalRef2 = this.modalService.open(ConfirmationComponent);
        modalRef2.componentInstance.message = message;
        modalRef2.result.then(result => {
          if (result) {
            this.documentService.updateStatus(this.docId, status, handler).subscribe(res => {
              if (res.isSuccess) {
                this.toastr.success('Thay đổi trạng thái tờ trình thành công')
                this.notificationService.SendSMS(this.document?.id!, 8).subscribe(res => {
                  if (res.isSuccess) {
                    this.toastr.success("Tin nhắn thông báo sẽ được gửi tới cho chuyên viên");
                  }
                })
                this.navigateToComponentBWithParam();
              }
            })
          }
        })
      })
    }
    else if (status == 6) {
      message = `Xác nhận không thông qua tờ trình ${this.document?.title}? Tờ trình này sẽ được đưa vào mục không duyệt.`;
      const modalRef = this.modalService.open(CommentComponent, { size: 'lg' });
      modalRef.componentInstance.title = 'Lý do không thông qua';
      modalRef.result.then(result => {
        var response = result;
        //this.submitApproval(status, response, message)
        const modalRef2 = this.modalService.open(ConfirmationComponent);
        modalRef2.componentInstance.message = message;
        modalRef2.result.then(result => {
          if (result) {
            this.documentService.updateStatus(this.docId, status, handler).subscribe(res => {
              if (res.isSuccess) {
                this.toastr.success('Thay đổi trạng thái tờ trình thành công')
                this.navigateToComponentBWithParam();
              }
            })
          }
        })
      })
    }
    else if (status == 12) {
      this.documentApprovalService.GetFinalPdf(this.docId).subscribe(res => {
        if (res.isSuccess) {
          var fullPath = common.GetFullFilePath(res.result.filePathToView as string);
          var webSocket = new WebSocket('wss://127.0.0.1:8987/Config');
          webSocket.onopen = function(e){
            var prms = {
              FileName: fullPath,
              FileUploadHandler: common.GetFullFilePathSign("FileUpload.aspx")
            };
            var json_prms = JSON.stringify(prms);
            vgca_sign_approved(json_prms,that.SignFileCallBack.bind(that));
          }
        }
      })
    }
    else if (status == 13) {
      var that = this;
      this.pdfService.getCurrentDocumentAsBlob().then(data =>{
        const blob = new Blob([data], { type: 'application/pdf' });
        that.documentService.uploadDocument(blob,that.currentUserName, that.currentUserId, that.docId).subscribe(res => {
          if(res.success){
            console.log("asdwq");
          }
        })
      });
    }
    else if (status == 9) {
      this.documentService.updateStatus(this.docId, status, handler).subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công')
          this.navigateToComponentBWithParam();
        }
      })
    }
    else if (status == 8) {
      message = `Xác nhận thông qua tờ trình ${this.document?.title}? Hành động này sẽ cập nhật trạng thái tờ trình thành "Chờ ra nghị quyết". Tờ trình sẽ được gửi về cho chuyên viên xử lý`;
      const modalRefE = this.modalService.open(HandlerSelectComponent);
      modalRefE.result.then(result => {
        handler = result;
        const modalRef = this.modalService.open(ConfirmationComponent);
        modalRef.componentInstance.message = message;
        modalRef.result.then(result => {
          if (result) {
            this.documentService.updateStatus(this.docId, status, handler).subscribe(res => {
              if (res.isSuccess) {
                this.toastr.success('Thay đổi trạng thái tờ trình thành công')
                if (status == 8) {
                  this.notificationService.SendSMS(this.document?.id!, 6).subscribe(res => {
                    if (res.isSuccess) {
                      this.toastr.success("Tin nhắn thông báo sẽ được gửi tới cho chuyên viên");
                    }
                  })
                }
                this.navigateToComponentBWithParam();
              }
            })
          }
        })
      })
    }
  }


  SignFileCallBack(rv: any) {
    var received_msg = JSON.parse(rv);
    console.log(received_msg);

    if (received_msg.Status == 0) {
      const url = received_msg.FileServer;
      const filenameMatch = url.match(/\/Upload\/(.*)/);
      const filename = filenameMatch[1];
      var payload : DocumentFileModel = {
        id : 0,
        fileName: filename,
        filePath: "\\Files\\Document_Attachments\\" + filename,
        docId: this.docId,
        version: 1,
        userId: this.currentUserId,
        modified: new Date(),
        deleted: false,
        modifiedBy: this.currentUserId,
        createdBy: this.currentUserId,
        created: new Date(),
        filePathToView: "\\Files\\Document_Attachments\\" + filename,
        isFinal: true,
        fileType: 1
      } 
      this.documentService.GDSignedFile(payload).subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công')
          this.navigateToComponentBWithParam();
        }
      })
    }
    else {
      this.toastr.error(received_msg.Message, "Thông báo từ phần mềm ký số");
    }
  }

  submitApproval(status: number, response: any, message: string) {
    const obj = {
      id: this.myApprovalId,
      title: status == 4 ? 'Duyệt' : status == 5 ? 'Ý kiến bổ sung' : 'Không duyệt',
      docId: this.docId,
      statusCode: status,
      userId: this.currentUserId,
      userName: '',
      modified: new Date(),
      deleted: false,
      modifiedBy: this.currentUserId,
      createdBy: this.currentUserId,
      created: new Date(),
      comment: response.comment,
      filePath: '',
      attachment: response.file,
      submitCount: this.document?.submitCount
    } as DocumentApprovalModel;

    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = message;
    modalRef.result.then(result => {
      if (result) {
        this.documentApprovalService.CreateDocumentApproval(obj).subscribe(res => {
          if (res.isSuccess) {
            // this.newComment = response;
            // this.addComment();
            this.toastr.success("Xử lý tờ trình thành công")
            this.navigateToComponentBWithParam()
          }
        })
      }
    })
  }

  showAction = false;
  showGeneralAction = false;
  toggleAction(type: number) {
    if (type == 1) {
      this.showAction = !this.showAction;
    }
    else {
      this.showGeneralAction = !this.showGeneralAction;
    }
  }

  showCommentDialog(approval?: DocumentApprovalModel) {
    const modalRef = this.modalService.open(ConfirmationComponent, { size: 'lg', backdrop: 'static' });
    modalRef.componentInstance.message = approval?.comment;
    if (approval?.filePath && approval?.filePath != '') {
      modalRef.componentInstance.attachmentFilePath = common.GetFullFilePath(approval?.filePath);
    }
  }

  navigateToComponentBWithParam() {
    this.route.paramMap.subscribe(param => {
      const paramValue = param.get('statusCode');
      this.router.navigate(['/admin/to-trinh', paramValue]);
    });
  }

  groupBy(array: any[], property: string) {
    return array.reduce((result, obj) => {
      const key = obj[property];

      if (!result[key]) {
        result[key] = [];
      }
      result[key].push(obj);

      return result;
    }, {});
  }


}
