import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { ResultFileApprovalComponent } from 'src/app/components/result-file-approval/result-file-approval.component';
import { DocumentModel } from 'src/app/models/document.model';
import { ApprovalItemModel, ApproverModel, DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import { ExcelExportService } from 'src/app/services/excel-export.service';
import { GetFullFilePath } from 'src/app/utils/commonFunctions';
import { environment } from 'src/environments/environments';

@Component({
  selector: 'app-document-detail-ket-qua-y-kien',
  templateUrl: './document-detail-ket-qua-y-kien.component.html',
  styleUrls: ['./document-detail-ket-qua-y-kien.component.scss'],
})
export class DocumentDetailKetQuaYKienComponent implements OnInit {
  @Input() document?: DocumentModel;
  approvers: ApproverModel[] = [];
  itemsPerPage = 2;
  currentPage = 1;
  quickFilterStatus = '';
  listApproved: ApprovalItemModel[] = [];
  approvalsData: ApprovalItemModel[] = [];
  constructor(
    private excelExportService: ExcelExportService,
    private modalService: NgbModal,
    private documentService: DocumentService,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute

  ) {}

  ngOnInit(): void {
    this.getListApproved();
  }
  toggleApprovals(index: number) {
    this.listApproved[index].show = !this.listApproved[index].show;
  }
  getListApproved() {
    const id = this.document?.id || 0;
    this.documentService.getDocumentApprovalsById(id).subscribe(res => {
      if (res.isSuccess) {
        this.approvalsData = res.result;
        this.listApproved = [...this.approvalsData];
        this.approvers = this.listApproved.flatMap(item => item.approvers);
        this.route.queryParams.subscribe((params) => {
        const userId = params['userId'];
        if (userId) {
          const approver = this.approvers.find(a => a.userId === userId);
          if (approver) {
            this.showFilesModal(approver.files, approver);
          }
        }
      });  
      }
    });
  }
  exportApprovalExcel() {
    const transformedData: any[] = [];
    let globalIndex = 1;
    this.listApproved?.forEach(item => {
      item.approvers.forEach(approver => {
        const customObj: any = {
          STT: globalIndex++,
          'Lần trình': `Lần trình ${item.submitCount || ''}`,
          'Cán Bộ': approver.userName,
          'Quyết Định': approver.decision,
          'Ý kiến bổ sung': approver.comment && approver.comment !== '' ? approver.comment : '',
          'Đánh giá lúc': approver.createdAt ? new Date(approver.createdAt).toLocaleString() : '',
        };
        transformedData.push(customObj);
      });
    });
    this.excelExportService.exportToExcel(transformedData, 'Kết quả ý kiến', 'Tổng hợp kết quả');
  }

  exportApprovalExcelByGroup(data: ApprovalItemModel) {
    const transformedData: any[] = [];
    data.approvers.forEach((approver, index) => {
      const customObj: any = {
        STT: ++index,
        'Cán Bộ': approver.userName,
        'Quyết Định': approver.decision,
        'Ý kiến bổ sung': approver.comment || '',
        'Đánh giá lúc': approver.createdAt ? new Date(approver.createdAt).toLocaleString() : '',
      };
      transformedData.push(customObj);
    });
    this.excelExportService.exportToExcelCustom(
      transformedData,
      `${this.document?.id}_Kết quả ý kiến - Lần trình ${data.submitCount}`,
      'Tổng hợp kết quả',
    );
  }
  onChangeQuickFilter() {
    if (this.quickFilterStatus === '') {
      this.listApproved = [...this.approvalsData];
    } else {
      this.listApproved = this.approvalsData
        .map(item => {
          const filteredApprovers = item.approvers.filter(
            approver =>
              approver.decision.trim().toLowerCase() ===
              this.quickFilterStatus.trim().toLowerCase()
          );
          const reviewedCount = filteredApprovers.length; 
          const totalApprovers = item.totalApprovers; 
  
          return {
            ...item,
            approvers: filteredApprovers,
            show: filteredApprovers.length > 0,
            reviewedCount, 
            totalApprovers,
          };
        })
        .filter(item => item.approvers.length > 0); 
    }
  }
  showCommentDialog(approval?: DocumentApprovalModel) {
    const modalRef = this.modalService.open(ConfirmationComponent, {
      size: 'lg',
      backdrop: 'static',
    });
    modalRef.componentInstance.message = approval?.comment;
    if (approval?.filePath && approval?.filePath != '') {
      modalRef.componentInstance.attachmentFilePath = GetFullFilePath(approval?.filePath);
    }
  }
  exportSignedFiles(submitCount: number) {
    const docId = this.document?.id || 0;
    this.documentService.resultSignedFiles(docId, submitCount).subscribe(res => {
      if (res.isSuccess && res.result?.linkZipToView) {
        const pathFile = `${environment.hostUrl}${res.result.linkZipToView}`;
        const anchor = document.createElement('a');
        anchor.href = pathFile;
        anchor.target = '_blank';
        document.body.appendChild(anchor);
        anchor.click();
        document.body.removeChild(anchor); 
      } else {
        this.toastr.error('Xuất file thất bại !!');
      }
    });
  }
  showFilesModal(files: DocumentFileModel[], approver: ApproverModel) {
    if (this.modalService.hasOpenModals()) {
      return;
    }
    const fileName = "file";
    const userId = approver.userId;
    this.router.navigate([], {
      queryParams: { fileName, userId },
      queryParamsHandling: 'merge',
    });
    const modalRef = this.modalService.open(ResultFileApprovalComponent, {
      size: 'lg',
      backdrop: 'static',
    });
    modalRef.componentInstance.modalFiles = files;
  }
  
}
