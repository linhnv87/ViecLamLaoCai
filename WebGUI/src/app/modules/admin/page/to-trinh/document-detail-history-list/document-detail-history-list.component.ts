import { Component, Input, OnInit } from '@angular/core';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentHistoryModel } from 'src/app/models/documentHistory.model';
import { DocumentHistoryService } from 'src/app/services/admin/document-history.service';
import {
  DATE_FORMAT,
  DOCUMENT_STATUS,
  DOCUMENT_STATUS_OPTIONS,
  MONTH_OPTIONS,
  YEAR_OPTIONS,
} from 'src/app/utils/constants';
@Component({
  selector: 'app-document-detail-history-list',
  templateUrl: './document-detail-history-list.component.html',
  styleUrls: ['./document-detail-history-list.component.scss'],
})
export class DocumentDetailHistoryListComponent implements OnInit {
  @Input() document?: DocumentModel;
  dataHis:DocumentHistoryModel[]=[];
  dummyData = [
    {
      id: 1,
      createdAt: '16/12/2024 14:12',
      user: 'Nguyễn Văn A',
      comment: 'KC được CVP ký số văn bản gửi phát hành',
    },
    { id: 2, createdAt: '16/12/2024 11:12', user: 'Nguyễn Văn B', comment: 'Thu hồi' },
    { id: 3, createdAt: '16/12/2024 10:12', user: 'Nguyễn Văn D', comment: 'Trả lại' },
    {
      id: 4,
      createdAt: '16/12/2024 9:12',
      user: 'Nguyễn Văn E',
      comment: 'Chuyển xử lý',
    },
    {
      id: 5,
      createdAt: '15/12/2024 9:16',
      user: 'Trần Y Tế',
      comment: 'Đồng ý (Đã ký số)',
    },
    {
      id: 6,
      createdAt: '15/12/2024 9:12',
      user: 'Nguyễn Công Thương',
      comment: 'Không đồng ý (Đã ký số)',
    },
    {
      id: 7,
      createdAt: '14/12/2024 9:12',
      user: 'Nguyễn Chuyên Viên',
      comment: 'Trình xin ý kiến BTV',
    },
  ];
  DATE_FORMAT = DATE_FORMAT;

  constructor(private documentHistoryService: DocumentHistoryService) {}

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    const id = this.document?.id || 0;
    this.documentHistoryService.getByDocumentId(id).subscribe(res => {
      if(res.isSuccess){
       this.dataHis = res.result as DocumentHistoryModel[];
      }
    });
  }
}
