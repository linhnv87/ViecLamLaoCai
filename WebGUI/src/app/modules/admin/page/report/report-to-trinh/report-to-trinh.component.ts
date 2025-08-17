import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import {
  ReportReviewModel,
  ReportReviewPagination,
  ReportReviewRequestModel,
} from 'src/app/models/report-review.model';
import { ReportReviewService } from 'src/app/services/admin/report-review.service';
import { displayUserRoles } from 'src/app/utils/commonFunctions';
import { ReportDetailComponent, ReportType } from '../report-detail/report-detail.component';

@Component({
  selector: 'app-report-to-trinh',
  templateUrl: './report-to-trinh.component.html',
  styleUrls: ['./report-to-trinh.component.scss'],
})
export class ReportToTrinhComponent {
  reportReview: ReportReviewModel[] = [];
  pagination: ReportReviewPagination = {
    pageSize: 10,
    pageNumber: 1,
    totalCount: 0,
    totalPage: 0,
  };
  filterForm?: FormGroup;
  constructor(
    private readonly router: Router,
    private readonly reportReviewService: ReportReviewService,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private modalService: NgbModal,
  ) {
    this.iniFormFilter();
  }

  ngOnInit(): void {
    this.getList();
  }
  iniFormFilter() {
    this.filterForm = this.fb.group({
      keyword: [''],
      fromDate: [null],
      toDate: [null],
    });
  }

  getList() {
    this.reportReviewService.getAllToTrinh(this.getPayload()).subscribe(res => {
      if (res.isSuccess) {
        this.reportReview = res.result.data;
        this.reportReview = this.reportReview.map(item => {
          item.roleDescription = displayUserRoles(item.roles, true);
          return item;
        });
        this.pagination = res.result.pagination;
      }
    });
  }

  getPayload(): ReportReviewRequestModel {
    const { keyword, fromDate, toDate } = this.filterForm?.getRawValue();
    return {
      keyword,
      fromDate: fromDate ? moment(fromDate).format('YYYY-MM-DD') : undefined,
      toDate: toDate ? moment(toDate).format('YYYY-MM-DD') : undefined,
      pageNumber: this.pagination.pageNumber,
      pageSize: this.pagination.pageSize,
    };
  }

  gotoHome() {
    this.router.navigate(['/admin/Dashboard']);
  }

  clear() {
    this.filterForm?.patchValue({
      keyword: null,
      fromDate: null,
      toDate: null,
    });
    this.getList();
  }

  getData(pageNumber: number) {
    this.pagination.pageNumber = pageNumber;
    this.getList();
  }

  exportExcel() {
    const payload: ReportReviewRequestModel = {
      ...this.getPayload(),
      type: 'EXCEL',
      fileName: 'Thống kê báo báo',
      sheetName: 'Thống kê tờ trình',
      pageNumber: 1, 
      pageSize: 1000    
    };

    this.reportReviewService.exportFileToTrinh(payload).subscribe(res => {
      if (res) {
        const a = document.createElement('a');
        const objectUrl = URL.createObjectURL(res);
        a.href = objectUrl;
        a.download = 'Thống kê báo cáo.xlsx';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(objectUrl);
      } else {
        this.toastr.error('Xuất file thất bại !!');
      }
    });
  }
  viewDetail(report: ReportReviewModel) {
    const { fromDate, toDate } = this.getPayload();
    const modalRef = this.modalService.open(ReportDetailComponent, {
      size: 'lg',
      centered: true,
      backdrop: 'static',
      keyboard: false,
    });
    modalRef.componentInstance.userId = report.userId;
    modalRef.componentInstance.type = ReportType.TO_TRINH;
    modalRef.componentInstance.fromDate = fromDate ?? null;
    modalRef.componentInstance.toDate = toDate ?? null;
    modalRef.closed.subscribe(() => { });
  }
}
