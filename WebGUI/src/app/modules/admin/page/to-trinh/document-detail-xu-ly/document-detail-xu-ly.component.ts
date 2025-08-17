import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DocumentModel } from 'src/app/models/document.model';
import { GetRoleInfo } from 'src/app/utils/commonFunctions';
import { ACTION_STATUS, DOCUMENT_STATUS } from 'src/app/utils/constants';
import { ToastrService } from 'ngx-toastr';
import { DocumentService } from 'src/app/services/admin/document.service';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import * as common from 'src/app/utils/commonFunctions';
import { filter, switchMap } from 'rxjs';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { WorkFlowService } from 'src/app/services/admin/work-flow.service';
import { DocumentReviewService } from 'src/app/services/admin/document-review.service';
import { ReviewResult } from 'src/app/models/enums/review-result.enum';
import { ToTrinhService } from 'src/app/services/admin/to-trinh.service';
import { ToTrinhPayloadModel } from 'src/app/models/to-trinh.model';
import { CommunicationService } from 'src/app/services/communication.service';
import { SelectUserDialogComponent } from '../select-user-dialog/select-user-dialog.component';

@Component({
  selector: 'app-document-detail-xu-ly',
  templateUrl: './document-detail-xu-ly.component.html',
  styleUrls: ['./document-detail-xu-ly.component.scss'],
})
export class DocumentDetailXuLyComponent {
  @Input() document?: DocumentModel;
  @Input() assigneeId?: string;
  documentStatus = DOCUMENT_STATUS;
  userRoles = GetRoleInfo();
  ykienKhac = '';
  isModalOpen: boolean = false;
  opinionText: string = '';
  currentUserId = common.GetCurrentUserId();
  currentUserName = common.GetCurrentUserInfo().userName;
  isFinal = false;
  files: File[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private documentService: DocumentService,
    private modalService: NgbModal,
    private workFlowService: WorkFlowService,
    private documentReviewService: DocumentReviewService,
    private toTrinhService: ToTrinhService,
    private communicationService: CommunicationService,
  ) { }

  navigateToComponentBWithParam() {
    this.communicationService.triggerReloadFunction();
    this.route.paramMap.subscribe(param => {
      const paramValue = param.get('statusCode');
      this.router.navigate(['/admin/to-trinh', paramValue]);
    });
  }

  SignFileCallBack(rv: any) {
    const received_msg = JSON.parse(rv);
    console.log(received_msg);

    if (received_msg.Status == 0) {
      const url = received_msg.FileServer;
      const filenameMatch = url.match(/\/Upload\/(.*)/);
      const filename = filenameMatch[1];
      const payload: DocumentFileModel = {
        id: 0,
        fileName: filename,
        filePath: '\\Files\\Document_Attachments\\' + filename,
        docId: this.document?.id || 0,
        version: 1,
        userId: this.currentUserId,
        modified: new Date(),
        deleted: false,
        modifiedBy: this.currentUserId,
        createdBy: this.currentUserId,
        created: new Date(),
        filePathToView: '\\Files\\Document_Attachments\\' + filename,
        isFinal: false,
        fileType: 1,
      };
      this.documentService.GDSignedFile(payload).subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công');
          this.navigateToComponentBWithParam();
        }
      });
    } else {
      this.toastr.error(received_msg.Message, 'Thông báo từ phần mềm ký số');
    }
  }

  onKySo() {
    this.isFinal = false;
    const that = this;
    this.documentService.signedFile(this.document?.id || 0, this.currentUserId).subscribe(res => {
      if (res.isSuccess) {
        console.log(res);
        const fullPath = common.GetFullFilePath(res.result.linkPdfToView as string);
        const webSocket = new WebSocket('wss://127.0.0.1:8987/Config');
        webSocket.onopen = function (e) {
          const prms = {
            FileName: fullPath.replace('//Files', '/Files'),
            FileUploadHandler: common.GetFullFilePathSign('FileUpload.aspx'),
          };
          const json_prms = JSON.stringify(prms);
          vgca_sign_approved(json_prms, that.SignFileCallBack.bind(that));
        };
      }
    });
  }

  onYKienKhac() {
    if (!this.ykienKhac) {
      this.toastr.error('Vui lòng nhập ý kiến');
    }
    this.showConfirmXinYKien(this.ykienKhac, ReviewResult.YKienKhac);
  }
  onDongY() {
    const defaultMessage = `Tôi đồng ý`;
    this.showConfirmXinYKien(this.ykienKhac || defaultMessage, ReviewResult.DongY);
  }

  onKhongDongY() {
    const defaultMessage = `Tôi Không đồng ý`;
    this.showConfirmXinYKien(this.ykienKhac || defaultMessage, ReviewResult.KhongDongY);
  }

  showConfirmXinYKien(message: string, reviewResult: number) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = message;
    modalRef.componentInstance.show = true;
    modalRef.closed
      .pipe(
        filter(r => !!r),
        switchMap(() => this.xinYKien$(message, reviewResult)),
      )
      .subscribe(res => {
        // this.onKySo();
        this.toastr.success('Thay đổi trạng thái tờ trình thành công');
        this.navigateToComponentBWithParam();
      });
  }

  xinYKien$(comment: string, reviewResult: number) {
    const input = {
      docId: this.document?.id,
      userId: common.GetCurrentUserId(),
      comment,
      reviewResult,
      files: this.files,
      submitCount: this.document?.submitCount,
    };
    return this.documentReviewService.updateDocumentReview(input);
  }

  onThuHoi() {
    const message = this.ykienKhac || 'Tôi xác nhận thu hồi';
    const input = {
      documentId: this.document?.id || 0,
      note: message,
      comment: message,
      currentUserId: common.GetCurrentUserId(),
    };
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = message;
    modalRef.componentInstance.show = true;
    modalRef.closed
      .pipe(
        filter(r => !!r),
        switchMap(() => this.documentService.retrieve(input)),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công');
          this.navigateToComponentBWithParam();
        } else {
          this.toastr.error(res.message);
        }
      });
  }

  onTraVe() {
    const message = this.ykienKhac || 'Tôi xác nhận trả về';
    const toStatusCode = this.document?.statusCode as string;
    this.showDialogUser(ACTION_STATUS.TRA_VE, toStatusCode, message);
  }
  onChuyenXuLy() {
    const message = this.ykienKhac || 'Chuyển xử lý';
    const toStatusCode = this.document?.statusCode as string;
    this.showDialogUser(ACTION_STATUS.CHUYEN_XU_LY, toStatusCode, message);
  }
  onChuyenKySo() {
    const message = this.ykienKhac || 'Tôi xác nhận chuyển kí số';
    const toStatusCode = this.documentStatus?.KY_SO as string;
    this.showDialogUser(ACTION_STATUS.CHUYEN_KY_SO, toStatusCode, message);
  }

  onChuyenKySoBiThu() {
    let listUserId: string[] = [];
    const message = this.ykienKhac || 'Tôi xác nhận chuyển kí số';
    const modalRef = this.modalService.open(SelectUserDialogComponent, { size: 'lg' });
    modalRef.componentInstance.document = this.document;
    modalRef.componentInstance.state = ACTION_STATUS.CHUYEN_KY_SO;
    modalRef.closed
      .pipe(
        filter(r => !!r && r.length > 0),
        switchMap(userIds => {
          listUserId = userIds;
          return this.xinYKien$(this.ykienKhac || 'Tôi đồng ý', ReviewResult.DongY);
        }),
        switchMap(() => this.onUpdateToTrinh$(listUserId, this.documentStatus.KY_SO, message)),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công');
          this.navigateToComponentBWithParam();
        } else {
          this.toastr.error(res.message);
        }
      });
  }

  showDialogUser(
    stateUser: string,
    toStatusCode: string,
    comment: string,
    isReupdate: boolean = false,
  ) {
    const modalRef = this.modalService.open(SelectUserDialogComponent, { size: 'lg' });
    modalRef.componentInstance.document = this.document;
    modalRef.componentInstance.state = stateUser;
    modalRef.closed
      .pipe(
        filter(r => !!r && r.length > 0),
        switchMap(userIds => this.onUpdateToTrinh$(userIds, toStatusCode, comment, isReupdate)),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công');
          this.navigateToComponentBWithParam();
        }
        else {
          this.toastr.error(res.message);
        }
      });
  }

  onKhongBanHanh() {
    const message = this.ykienKhac || 'Tôi xác nhận không ban hành';
    const users = [common.GetCurrentUserId()];
    const toStatusCode = this.documentStatus.KHONG_BAN_HANH;
    this.showConfirmChuyenXuLy(users, toStatusCode, message);
  }

  showConfirmChuyenXuLy(
    users: string[],
    toStatusCode: string,
    comment: string,
    isReupdate: boolean = false,
  ) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = comment;
    modalRef.componentInstance.show = true;
    modalRef.closed
      .pipe(
        filter(r => !!r),
        switchMap(() => this.onUpdateToTrinh$(users, toStatusCode, comment, isReupdate)),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Thay đổi trạng thái tờ trình thành công');
          this.navigateToComponentBWithParam();
        }
      });
  }

  onUpdateToTrinh$(
    users: string[],
    toStatusCode: string,
    comment: string,
    isReupdate: boolean = false,
  ) {
    const payload: ToTrinhPayloadModel = {
      documentId: this.document?.id,
      fromStatusCode: this.document?.statusCode as string,
      userId: common.GetCurrentUserId(),
      users,
      toStatusCode,
      comment,
    };
    return isReupdate ? this.toTrinhService.reUpdate(payload, this.files) : this.toTrinhService.update(payload, this.files);
  }

  onInKetQua() {
    // todo
    console.log('In KET QUA');
  }

  onXinYKienLai() {
    const message = this.ykienKhac || 'Tôi xác nhận xin ý kiến lại';
    const toStatusCode = this.documentStatus.XIN_Y_KIEN_LAI;
    this.showDialogUser(ACTION_STATUS.XIN_Y_KIEN_LAI, toStatusCode, message, true);
  }

  triggerFileInput(): void {
    const fileInput = document.querySelector<HTMLInputElement>('#fileInputXuLy');
    if (fileInput) {
      fileInput.click();
    }
  }
  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const newFiles = Array.from(input.files); // Convert FileList to Array
      this.files = this.files.concat(newFiles);
    }
    input.value = '';
  }

  removeFile(index: number): void {
    this.files.splice(index, 1);
  }
  openModal() {
    this.isModalOpen = true;
  }
  closeModal() {
    this.isModalOpen = false;
    this.opinionText = '';
  }
  exportPrintResult() {
    const docId = this.document?.id || 0;
    this.documentService.printResult(docId, this.opinionText).subscribe(res => {
      const blob = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' }); 
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = 'KetQua.docx';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url); 
      this.closeModal();
    });
  }
  
}
