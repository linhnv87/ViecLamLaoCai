import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { CommentModel } from 'src/app/models/comment.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { CommentService } from 'src/app/services/admin/comment.service';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import { environment } from 'src/environments/environments';
import * as common from 'src/app/utils/commonFunctions';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import { NotificationService } from 'src/app/services/nofitication.service';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { DocumentFileService } from 'src/app/services/admin/document-file.service';
import { CommunicationService } from 'src/app/services/communication.service';

@Component({
  selector: 'app-xu-ly',
  templateUrl: './xu-ly.component.html',
  styleUrls: ['./xu-ly.component.scss'],
})
export class XuLyComponent implements OnInit, OnChanges {
  userId = common.GetCurrentUserId();
  userRoles = common.GetCurrentUserInfo().roles;
  roleInfo = common.GetRoleInfo();

  isAuthor = false;

  approvalAction = true;
  specialistAction = false;
  generalAction = false;

  documentTitle = '';
  docId: number | undefined | null;
  document: DocumentModel | undefined | null;

  DateEndApproval: string = '';
  files: File[] = [];
  sideFiles: File[] = [];
  selectedFileNames: string = 'Chưa có file nào được chọn.';
  selectImg: boolean = false;

  documentApproval: DocumentApprovalModel = {
    id: 0,
    title: '',
    docId: 0,
    statusCode: 0,
    userId: this.userId,
    modified: new Date(),
    deleted: false,
    modifiedBy: this.userId,
    createdBy: this.userId,
    created: new Date(),
    comment: '',
  };

  isShow: boolean = false;

  fieldData: FieldModel[] = [];

  todayForFile = new Date();

  todayDate = '';

  approvals: DocumentApprovalModel[] = [];

  groupedApprovals: any[] = [];
  groupedFiles: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private communicationService: CommunicationService,
    private router: Router,
    private documentApprovalService: DocumentApprovalService,
    private commentService: CommentService,
    private toastr: ToastrService,
    private modalService: NgbModal,
    private documentFileService: DocumentFileService,
    private notificationService: NotificationService,
    private fieldService: FieldServiceService,
  ) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.todayDate = tomorrow.toISOString().split('T')[0];
  }

  selectedFileNamesArray: string[] = [];

  loadFieldData() {
    this.fieldService.getAll().subscribe(res => {
      if (res.isSuccess) {
        this.fieldData = res.result as FieldModel[];
      }
    });
  }

  submit(send: boolean) {
    if (this.document) {
      this.document.dateEndApproval = moment(this.DateEndApproval).toDate();
      var checkForm =
        this.document.title?.trim() == '' ||
        this.document.fieldId == 0 ||
        this.document.note?.trim() == '';
      if (!checkForm) {
        if (send) {
          // if(this.document.statusCode == 5){
          //   this.document.statusCode = 4
          // }else{
          //   this.document.statusCode = 3;
          // }
          // this.document.statusCode = 3;
        }
        const modalRef = this.modalService.open(ConfirmationComponent);
        modalRef.componentInstance.message = `Xác nhận hành động này`;
        modalRef.result.then(result => {
          if (result) {
            if (!this.document?.documentFiles) this.document!.documentFiles = [];
            const addDataPromise = this.documentService
              .update(this.document!, this.files, this.sideFiles)
              .subscribe(res => {
                if (res.isSuccess) {
                  this.toastr.success('Gửi thành công');
                  // this.getDocumentById(this.docId!);
                  this.navigateToComponentBWithParam();
                  if (send) this.communicationService.triggerReloadFunction();
                } else {
                  this.toastr.error(res.message);
                }
              });
          }
        });
      } else {
        this.toastr.warning('Cần nhập đầy đủ thông tin');
      }
    }
  }
  showCommentDialog(approval?: DocumentApprovalModel) {
    const modalRef = this.modalService.open(ConfirmationComponent, {
      size: 'lg',
      backdrop: 'static',
    });
    modalRef.componentInstance.message = approval?.comment;
    if (approval?.filePath && approval?.filePath != '') {
      modalRef.componentInstance.attachmentFilePath = common.GetFullFilePath(approval?.filePath);
    }
  }
  ngOnInit(): void {
    this.loadFieldData();
    if (this.userRoles.includes('Approver')) {
      this.approvalAction = true;
      this.specialistAction = false;
      this.generalAction = false;
    }
    if (this.userRoles.includes('Specialist')) {
      this.approvalAction = false;
      this.specialistAction = true;
      this.generalAction = false;
    }
    if (this.userRoles.includes('General Specialist')) {
      this.approvalAction = false;
      this.specialistAction = false;
      this.generalAction = true;
    }
    if (this.userRoles.includes('Admin')) {
      this.approvalAction = true;
      this.specialistAction = true;
      this.generalAction = true;
    }
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);
      this.getDocumentById(this.docId!);
      this.GetApprovalData();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.route.snapshot.paramMap.get('id') != null) {
      this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);
      this.getDocumentById(this.docId!);
      this.GetApprovalData();
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

  toggleApprovals(index: number) {
    this.groupedApprovals[index].show = !this.groupedApprovals[index].show;
  }

  toggleFiles(index: number) {
    this.groupedFiles[index].show = !this.groupedFiles[index].show;
  }

  getDocumentById(id: number) {
    const dataPromise = this.documentService.getDocumentsById(id).subscribe(res => {
      if (res.isSuccess) {
        this.document = res.result as DocumentModel;
        this.isShow = true;
        this.documentTitle = this.document.title!;
        this.document.documentFiles!.forEach(x => {
          x.filePath = environment.hostUrl + x.filePath;
          x.filePathToView = environment.hostUrl + x.filePathToView;
        });
        if (this.document.createdBy?.toString().toLowerCase() === this.userId.toLowerCase()) {
          this.isAuthor = true;
        }
        this.DateEndApproval = moment(this.document.dateEndApproval).format('yyyy-MM-DD');
        if (this.document.approvals) {
          this.groupedApprovals = Object.entries(
            this.groupBy(this.document.approvals!, 'submitCount'),
          ).map(([submitCount, data]) => ({
            submitCount,
            data,
          }));
          this.groupedApprovals = this.groupedApprovals.map((e: any) => {
            return { ...e, show: false };
          });
          this.groupedApprovals[0].show = true;
        }
        if (this.document.documentFiles!.length > 0) {
          this.groupedFiles = Object.entries(
            this.groupBy(this.document.documentFiles!, 'version'),
          ).map(([version, data]) => ({
            version,
            data,
          }));
          this.groupedFiles = this.groupedFiles.sort(x => x.version).reverse();
          this.groupedFiles = this.groupedFiles.map((e: any) => {
            return { ...e, show: false };
          });
          this.groupedFiles.forEach(e => {
            e.data = [...e.data].sort(this.sortFile);
          });
          this.groupedFiles[0].show = true;
        }
      } else {
        this.navigateToComponentBWithParam();
      }
    });
  }
  sortFile(a: any, b: any) {
    var res = 0;
    if (a.fileType > b.fileType) res = -1;
    else {
      if (a.id < b.id) res = -1;
    }
    // console.log([a.fileType, b.fileType, res]);
    return res;
  }

  GetApprovalData() {
    this.documentApprovalService
      .GetSingleByUserIdAndDocId(this.userId, this.docId!)
      .subscribe(res => {
        if (res.isSuccess) {
          this.documentApproval = res.result;
        }
      });
  }

  deleteFile(file: DocumentFileModel) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = `Xác nhận xóa tệp đính kèm "${file.fileName}"`;
    modalRef.result.then(result => {
      if (result) {
        this.documentFileService.delete(file.id).subscribe(res => {
          console.log(res);

          if (res.isSuccess) {
            this.toastr.info('Xóa tệp thành công');
            console.log(this.document?.documentFiles?.findIndex(x => x.id == file.id));
            this.document?.documentFiles?.splice(
              this.document.documentFiles.findIndex(x => x.id == file.id),
              1,
            );
            if (this.document!.documentFiles!.length > 0) {
              this.groupedFiles.forEach(g => {
                g.data.splice(
                  g.data.findIndex((x: any) => x.id == file.id),
                  1,
                );
              });
              //this.groupedFiles[0].show = true;
            }
          }
        });
      }
    });
  }
  // Xu ly form documentApproval
  Approved() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 4;
    this.documentApproval.title = 'Duyệt';
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if (res.isSuccess) {
        this.toastr.success('Đánh giá tờ trình thành công');
        this.navigateToComponentBWithParam();
      }
    });
  }

  ApprovedAndIdea() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 5;
    this.documentApproval.title = 'Duyệt và ý kiến';
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if (res.isSuccess) {
        this.toastr.success('Đánh giá tờ trình thành công');
        this.notificationService.CreateNotification(3, this.docId!, this.userId).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.info('Đánh giá đã được gửi đến chuyên viên');
          }
        });
        this.navigateToComponentBWithParam();
      }
    });
  }

  NotApproved() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 6;
    this.documentApproval.title = 'Không duyệt';
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if (res.isSuccess) {
        this.toastr.success('Đánh giá tờ trình thành công');
        this.navigateToComponentBWithParam();
      }
    });
  }

  TemporarySave() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 2;
    this.documentApproval.title = 'Lưu tạm';
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if (res.isSuccess) {
        this.toastr.success('Đánh giá tờ trình thành công');
        this.navigateToComponentBWithParam();
      }
    });
  }

  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.files = this.files.concat(newFiles);
        const newFileNames = newFiles.map(file => file.name);
        this.selectedFileNamesArray = this.selectedFileNamesArray.concat(newFileNames);
        this.selectImg = true;
      }
    }
  }
  onSideFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.sideFiles = this.sideFiles.concat(newFiles);
      }
    }
  }

  moveFileUp(index: number): void {
    if (index > 0) {
      const temp = this.files[index];
      this.files[index] = this.files[index - 1];
      this.files[index - 1] = temp;
      this.updateSelectedFileNamesArray();
    }
  }
  moveSideFileUp(index: number): void {
    if (index > 0) {
      const temp = this.sideFiles[index];
      this.sideFiles[index] = this.sideFiles[index - 1];
      this.sideFiles[index - 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }
  moveSideFileDown(index: number): void {
    if (index < this.sideFiles.length - 1) {
      const temp = this.sideFiles[index];
      this.sideFiles[index] = this.sideFiles[index + 1];
      this.sideFiles[index + 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }

  moveFileDown(index: number): void {
    if (index < this.files.length - 1) {
      const temp = this.files[index];
      this.files[index] = this.files[index + 1];
      this.files[index + 1] = temp;
      this.updateSelectedFileNamesArray();
    }
  }

  deleteUploadSideFile(index: number): void {
    this.sideFiles.splice(index, 1);
    // this.updateSelectedFileNamesArray();
  }

  deleteUploadFile(index: number): void {
    this.files.splice(index, 1);
    this.updateSelectedFileNamesArray();
  }

  updateSelectedFileNamesArray(): void {
    this.selectedFileNamesArray = this.files.map(file => file.name);
  }
}
