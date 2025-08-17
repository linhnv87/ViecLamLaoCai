import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DocumentModel } from 'src/app/models/document.model';
import { SMSLogGroupModel, SMSLogModel, SMSLogQueryModel } from 'src/app/models/sms-log.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import { SmsLogService } from 'src/app/services/admin/sms-log.service';
import {
  DATE_FORMAT,
} from 'src/app/utils/constants';
import * as common from 'src/app/utils/commonFunctions';
@Component({
  selector: 'app-track-message-result',
  templateUrl: './track-message-result.component.html',
  styleUrls: ['./track-message-result.component.scss']
})
export class TrackMessageResultComponent implements OnInit {
  documents: DocumentModel[] = [];
  filteredDocuments = [...this.documents];
  selectedDocumentId: number = 0;
  selectedType: number = 0;
  addedMessages: SMSLogGroupModel[] = [];
  reminderMessages: SMSLogGroupModel[] = [];
  filteredAddedMessages: SMSLogGroupModel[] = [];
  filteredReminderMessages: SMSLogGroupModel[] = [];
  currentPageAdded: number = 1;
  currentPageReminder: number = 1;
  dateFilterForm: FormGroup;
  itemsPerPageAdd = 30;
  itemsPerPageReminder = 30;
  DATE_FORMAT = DATE_FORMAT;
  id = "";
  userRoles = common.GetRoleInfo();
  selectedStatus: boolean | null = null;
  constructor(
    private messageService: SmsLogService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private documentService: DocumentService) {
    this.dateFilterForm = this.fb.group({
      fromDate: [''],
      toDate: [''],
      status: ['']
    });
  }
  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.id = id;
        this.fetchMessagesByDocId();
      }
    });
    this.getListDocument();
  }
  toggleItemReminder(index: number) {
    this.reminderMessages[index].show = !this.reminderMessages[index].show;
  }
  toggleItemAdded(index: number) {
    this.addedMessages[index].show = !this.addedMessages[index].show;
  }

  onPageChangeAdded(pageNumber: number): void {
    this.currentPageAdded = pageNumber;
  }
  getListDocument() {
    this.documentService
      .getList("xin-y-kien")
      .subscribe(res => {
        if (res.isSuccess) {
          this.documents = res.result as DocumentModel[];
        }
      });
  }
  filterDocuments() {
    this.resetData();
    const fromDateValue = this.dateFilterForm.get('fromDate')?.value;
    const toDateValue = this.dateFilterForm.get('toDate')?.value;
    if (!fromDateValue || !toDateValue) {
      this.resetData();
      this.toastr.error('Vui lòng chọn đầy đủ Từ ngày và Đến ngày!');
      return;
    }
    const fromDate = new Date(fromDateValue);
    const toDate = new Date(toDateValue);
    if (isNaN(fromDate.getTime()) || isNaN(toDate.getTime())) {
      this.resetData();
      this.toastr.error('Định dạng ngày không hợp lệ!');
      return;
    }
    toDate.setHours(23, 59, 59, 999);
    const fromTime = fromDate.getTime();
    const toTime = toDate.getTime();
    if (fromTime > toTime) {
      this.resetData();
      this.toastr.error('Ngày "Từ ngày" phải nhỏ hơn hoặc bằng "Đến ngày"!');
      return;
    }

    this.filteredDocuments = this.documents.filter(document => {
      const docTime = new Date(document.created ?? '').getTime();
      return docTime >= fromTime && docTime <= toTime;
    });
    if (this.filteredDocuments.length === 0) {
      this.toastr.warning('Không tìm thấy tài liệu trong khoảng ngày này!');
    }
  }

  onPageChangeReminder(pageNumber: number) {
    this.currentPageReminder = pageNumber;
  }
  onStatusChange(event: any) {
    this.resetData();
    const selectedStatus = event.target.value.trim();
    this.selectedStatus = selectedStatus === 'true' ? true : selectedStatus === 'false' ? false : null;
  }
  fetchMessagesByDocId(status: boolean | null = null) {
    const documentId = this.selectedDocumentId || Number(this.id);
    if (!documentId) {
      return;
    }
    const queryaddModel: SMSLogQueryModel = {
      docId: documentId,
      type: 0,
    };
    if (status !== null) {
      queryaddModel.isSucceeded = status;
    }
    // Gọi API cho Adder Messages
    this.messageService.getAllLogsWithUserNames(queryaddModel).subscribe(
      res => {
        if (res.isSuccess) {
          const messages = res.result as SMSLogGroupModel[];
          this.addedMessages = messages;
          this.filterDataByDocument();
        } else {
          this.toastr.error('Failed to fetch added messages:', res.message);
        }
      }
    );
    const queryRemindModel: SMSLogQueryModel = {
      docId: documentId,
      type: 1,
    };
    if (status !== null) {
      queryRemindModel.isSucceeded = status;
    }
    // Gọi API cho Reminder Messages
    this.messageService.getAllLogsWithUserNames(queryRemindModel).subscribe(
      res => {
        if (res.isSuccess) {
          const messages = res.result as SMSLogGroupModel[];
          this.reminderMessages = messages;
          this.filterDataByDocument();
        } else {
          this.toastr.error('Failed to fetch reminder messages:', res.message);
        }
      },
    );
  }
  filterDataByDocument(): void {
    this.filteredAddedMessages = this.addedMessages;
    this.filteredReminderMessages = this.reminderMessages;
  }
  resetData() {
    // this.dateFilterForm.reset();
    this.filteredDocuments = [];
    this.addedMessages = [];
    this.reminderMessages = [];
    this.filteredAddedMessages = [];
    this.filteredReminderMessages = [];
  }
}
