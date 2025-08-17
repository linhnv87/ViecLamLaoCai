import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { filter, finalize, forkJoin, of, switchMap } from 'rxjs';
import { CommentComponent } from 'src/app/components/comment/comment.component';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { UploadDocumentComponent } from 'src/app/components/upload-document/upload-document.component';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentRetrievalModel } from 'src/app/models/documentHistory.model';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { FieldModel } from 'src/app/models/field.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import { CommunicationService } from 'src/app/services/communication.service';
import { NotificationService } from 'src/app/services/nofitication.service';
import * as common from 'src/app/utils/commonFunctions';
import {
  DATE_FORMAT,
  DOCUMENT_STATUS,
  DOCUMENT_STATUS_OPTIONS,
  MONTH_OPTIONS,
  YEAR_OPTIONS,
} from 'src/app/utils/constants';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { DocumentTypeService } from 'src/app/services/admin/document-type.service';
import { LoadingService } from 'src/app/services/loading.service';
import { PlanRemindComponent } from './plan-remind/plan-remind.component';
import { ToTrinhPayloadModel } from 'src/app/models/to-trinh.model';
import { ToTrinhService } from 'src/app/services/admin/to-trinh.service';

@Component({
  selector: 'app-to-trinh',
  templateUrl: './to-trinh.component.html',
  styleUrls: ['./to-trinh.component.scss'],
})
export class ToTrinhComponent implements OnInit {
  userRoles = common.GetRoleInfo();
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();

  fieldsOptions: FieldModel[] = [];
  typeOptions: DocumentTypeModel[] = [];
  currentYear = new Date().getFullYear();
  documentStatusOptions = DOCUMENT_STATUS_OPTIONS;
  yearOptions = YEAR_OPTIONS;
  monthOptions = MONTH_OPTIONS;
  documentStatus = DOCUMENT_STATUS;
  DATE_FORMAT = DATE_FORMAT;

  itemsPerPage: number = 5;
  currentPage: number = 1;
  actionOnItem = -1;
  code: string | null = '';
  documents: DocumentModel[] = [];
  quickFilterStatus = null;
  filterForm?: FormGroup;
  currentUserId = common.GetCurrentUserId();
  public isXinYKien: boolean = false;
  constructor(
    private communicationService: CommunicationService,
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private router: Router,
    private fieldService: FieldServiceService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private documentTypeService: DocumentTypeService,
    private loadingService: LoadingService,
    private toTrinhService: ToTrinhService,
  ) {
    this.iniFormFilter();
  }
  ngOnInit() {
    this.initOptions();
    this.loadData();

  }

  iniFormFilter() {
    this.filterForm = this.fb.group({
      keyword: [''],
      status: [''],
      fromDate: [null],
      toDate: [null],
      type: [0],
      field: [0],
      year: [0],
      month: [0],
    });
  }
  search() {
    this.loadData();
  }

  initOptions() {
    forkJoin([this.fieldService.getAll(), this.documentTypeService.getAll()]).subscribe(
      ([resField, resType]) => {
        if (resField.isSuccess) {
          this.fieldsOptions = resField.result as FieldModel[];
        }

        if (resType.isSuccess) {
          this.typeOptions = resType.result as DocumentTypeModel[];
        }
      },
    );
  }

  loadData() {

    this.route.paramMap.subscribe(params => {
      const statusCode = params.get('statusCode');
      this.isXinYKien = statusCode === 'xin-y-kien';
      this.code = statusCode;
      if (statusCode == null || statusCode == 'tong-to-trinh') {
        this.getListDocument();
      } else {
        this.getListDocument(statusCode);
      }
    });
  }

  public getListDocument(status?: string) {
    this.processApproving();
    this.loadingService.setLoading(true);
    this.documentService
      .getList(status)
      .pipe(finalize(() => this.loadingService.setLoading(false)))
      .subscribe(res => {
        if (res.isSuccess) {
          this.documents = res.result as DocumentModel[];
          if (!status) {
            this.documents = this.documents.filter(doc =>
              doc.statusCode !== 'du-thao'
            );
          }
          this.documents.sort(
            (a, b) => new Date(b.modified!).getTime() - new Date(a.modified!).getTime(),
          );
          this.filterData();
        }
      });
  }

  actionToShow = 0;
  showAction(id: number) {
    if (this.actionToShow != id) {
      this.actionToShow = id;
    } else {
      this.actionToShow = 0;
    }
  }

  remind(id: number) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      if (result) {
        this.notificationService.CreateNotification(2, id).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Thông báo sẽ được gửi tới cho cán bộ', 'Nhắc duyệt thành công');
          }
        });
        this.notificationService.SendSMSV2(id, 2).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Tin nhắn sẽ được gửi tới cho cán bộ', 'Nhắc duyệt thành công');
          }
        });
      }
    });
  }

  planRemind(doc: DocumentModel) {
    const modalRef = this.modalService.open(PlanRemindComponent);
    modalRef.componentInstance.dateEndApproval = doc.dateEndApproval;
    modalRef.componentInstance.remendingDate = doc.remindDatetime || new Date().toISOString();
    modalRef.closed
      .pipe(
        filter(Boolean),
        switchMap(res => {
          if (res) {
            doc.remindDatetime = res;
            return this.documentService.update(doc);
          }
          return of(null);
        }),
      )
      .subscribe(res => {
        if (res) {
          this.toastr.success('Hẹn lịch nhắc thành công!');
        }
      });
  }

  publishDocument(id: number) {
    const modalRef = this.modalService.open(UploadDocumentComponent);
    modalRef.componentInstance.docId = id;
    modalRef.result.then(result => {
      if (result) {
        // this.notificationService.SendSMS(id, 7)
        this.toastr.success('Ban hành nghị quyết thành công');
        this.router.navigate(['/admin/to-trinh/11']);
      }
    });
  }

  updateStatus(docId: number, status: number) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      if (result) {
        this.documentService.updateStatus(docId, status, 0).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Cập nhật trạng thái tờ trình thảnh công');
            // this.router.navigate(['/admin/to-trinh/' + status]);
            this.communicationService.triggerReloadFunction();
            if (status == StatusCode.Pending) {
              this.notificationService.CreateNotification(1, docId).subscribe(res => {
                if (res.isSuccess) {
                  this.toastr.info('Thông báo gửi duyệt đã được gửi cho cán bộ');
                }
              });
            }
            this.loadData();
          }
        });
      }
    });
  }

  retrieve(docId: number) {
    const modalRef = this.modalService.open(CommentComponent);
    modalRef.componentInstance.title = 'Lý do thu hồi';
    modalRef.componentInstance.showUpload = false;
    modalRef.result.then(result => {
      if (result) {
        console.log(result);
        var request: DocumentRetrievalModel = {
          documentId: docId,
          note: result.comment,
        };
        this.documentService.retrieve(request).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Thu hồi tờ trình thảnh công');
            this.loadData();
            this.communicationService.triggerReloadFunction();
          } else {
            this.toastr.error(res.message);
          }
        });
      }
    });
  }

  returnDoc(docId: number) {
    const modalRef = this.modalService.open(CommentComponent);
    modalRef.componentInstance.title = 'Lý do thu hồi';
    modalRef.componentInstance.showUpload = false;
    modalRef.result.then(result => {
      if (result) {
        console.log(result);
        var request: DocumentRetrievalModel = {
          documentId: docId,
          note: result.comment,
        };
        this.documentService.returnDoc(request).subscribe(res => {
          if (res.isSuccess) {
            this.notificationService.SendSMS(docId, 9).subscribe(r => {
              if (r.isSuccess) {
                this.toastr.success('Tin nhắn thông báo sẽ được gửi tới cho chuyên viên');
              }
            });
            this.toastr.success('Trả lại tờ trình thảnh công');
            this.loadData();
            this.communicationService.triggerReloadFunction();
          } else {
            this.toastr.error(res.message);
          }
        });
      }
    });
  }

  deleteDocument(docId: number) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      if (result) {
        this.documentService.delete(docId).subscribe(res => {
          if (res.isSuccess) {
            this.toastr.success('Xóa tờ trình thảnh công');
            this.loadData();
            this.communicationService.triggerReloadFunction();
          }
        });
      }
    });
  }

  gotoHome() {
    this.router.navigate(['/admin/Dashboard']);
  }

  clear() {
    this.filterForm?.patchValue({
      keyword: null,
      status: '',
      fromDate: null,
      toDate: null,
      type: 0,
      field: 0,
      year: 0,
      month: 0,
    });
    this.search();
  }

  filterData() {
    const { keyword, status, fromDate, toDate, type, field, year, month } =
      this.filterForm?.getRawValue();
    if (!!keyword) {
      this.documents = this.documents.filter(x =>
        x.title?.toLowerCase().includes(keyword.toLowerCase()),
      );
    }

    if (!!status) {
      this.documents = this.documents.filter(x => x.statusCode === status);
    }

    if (!!+field) {
      this.documents = this.documents.filter(x => x.fieldId === +field);
    }
    if (!!+type) {
      this.documents = this.documents.filter(x => x.typeId === +type);
    }

    if (!!year) {
      this.documents = this.documents.filter(x => new Date(x.created!).getFullYear() == year);
    }

    if (!!month && !!Number(month)) {
      this.documents = this.documents.filter(x => new Date(x.created!).getMonth() + 1 == month);
    }

    if (fromDate && moment(fromDate).isValid()) {
      this.documents = this.documents.filter(d =>
        moment(d.modified || d.created).isSameOrAfter(moment(fromDate)),
      );
    }
    if (toDate && moment(toDate).isValid()) {
      this.documents = this.documents.filter(d =>
        moment(d.modified || d.created).isSameOrBefore(moment(toDate)),
      );
    }
  }

  onXinYKienLai(id: number) {
    this.router.navigate(['admin/to-trinh', id, 'edit']);
  }

  onXinYKien(id: number) {
    this.router.navigate(['admin/to-trinh', id, 'edit']);
  }

  onThuHoi(document: DocumentModel) {
    const modalRef = this.modalService.open(CommentComponent);
    modalRef.componentInstance.title = 'Lý do thu hồi';
    modalRef.componentInstance.showUpload = false;
    modalRef.closed
      .pipe(
        switchMap(res => {
          if (res) {
            const payload: DocumentRetrievalModel = {
              documentId: document.id,
              currentUserId: common.GetCurrentUserId(),
              comment: res.comment,
              note: res.comment,
            };
            return this.documentService.retrieve(payload);
          } else {
            return of(null);
          }
        }),
      )
      .subscribe(result => {
        if (!result) return;
        if (result?.isSuccess) {
          this.toastr.success('Thu hồi tờ trình thành công');
          this.loadData();
          this.communicationService.triggerReloadFunction();
        } else {
          this.toastr.error(result.message);
        }
      });
  }

  processApproving() {
    this.documentService.processApproving().subscribe();
  }  
}
