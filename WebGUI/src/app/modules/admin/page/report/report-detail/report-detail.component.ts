import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DocumentReport } from 'src/app/models/report-review.model';
import { ReportReviewService } from 'src/app/services/admin/report-review.service';

export const ReportType = {
  Y_KIEN: 'xin-y-kien',
  TO_TRINH: 'to-trinh',
};
@Component({
  selector: 'app-report-detail',
  templateUrl: './report-detail.component.html',
  styleUrls: ['./report-detail.component.scss'],
})
export class ReportDetailComponent implements OnInit {
  public userId: string = '';
  public type: string = '';
  public fromDate: string | null = null;
  public toDate: string | null = null;

  public listDocument: DocumentReport[] = [];
  constructor(
    private activeModal: NgbActiveModal,
    private readonly reportReviewService: ReportReviewService,
    private readonly router: Router,
  ) { }

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    if (!this.userId) return;
  
    this.reportReviewService.getListDocumentById(this.userId, this.type, this.fromDate, this.toDate)
      .subscribe(res => {
        this.listDocument = res.result;
      });
  }
  

  confirm(result: boolean) {
    this.activeModal.close(result);
  }

  onViewDocument(document: DocumentReport) {
    // this.activeModal.close(false);
    const url = this.router.serializeUrl(
      this.router.createUrlTree([
        `/admin/to-trinh/${document.statusCode}/document-detail/${document.id}`
      ], { queryParams: { assigneeId: document.assigneeID } })
    );
  
    window.open(url, '_blank');
  }
  
}
