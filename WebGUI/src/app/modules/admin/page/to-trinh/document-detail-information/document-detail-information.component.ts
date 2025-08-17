import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { filter, switchMap } from 'rxjs';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import { DATE_FORMAT, DOCUMENT_STATUS } from 'src/app/utils/constants';
import * as common from 'src/app/utils/commonFunctions';
import { AddFileComponent } from 'src/app/components/add-file/add-file.component';
import { DocumentFileService } from 'src/app/services/admin/document-file.service';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-document-detail-information',
  templateUrl: './document-detail-information.component.html',
  styleUrls: ['./document-detail-information.component.scss'],
})
export class DocumentDetailInformationComponent implements OnInit {
  @Input() document?: DocumentModel;
  @Input() listAttachment: DocumentFileModel[] = [];
  @Output() viewFile: EventEmitter<DocumentFileModel> = new EventEmitter<DocumentFileModel>();
  @Output() listUpdated = new EventEmitter<void>();
  documentStatus = DOCUMENT_STATUS;
  DATE_FORMAT = DATE_FORMAT;
  assigneeId = '';
  currentUserId = common.GetCurrentUserId();
  userRoles = common.GetRoleInfo();
  constructor(private documentService: DocumentService, 
    private modalService: NgbModal, 
    private route: ActivatedRoute,
    private documentFileService:DocumentFileService,
    private toastr: ToastrService) {}

  ngOnInit(): void {
    this.assigneeId = this.route.snapshot.queryParams?.['assigneeId'];
  }

  onViewFile(file: DocumentFileModel) {
    this.viewFile.emit(file);
  }
 
  openAddFileModal(id: number) {
    const modalRef = this.modalService.open(AddFileComponent,{size: 'lg'});
    modalRef.componentInstance.docId = id;
    modalRef.result.then((newFiles: DocumentFileModel[]) => {
      if (newFiles && newFiles.length > 0) {
        this.listUpdated.emit();
      }
    }).catch(() => {});
  }

  onDeleteFile(file: any) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Bạn có chắc chắn muốn xóa văn bản này không?';
    modalRef.componentInstance.show = false;
  
    modalRef.closed
      .pipe(
        filter(r => !!r), 
        switchMap(() => this.documentFileService.delete(file.id))
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Xóa file thành công!');
          this.listUpdated.emit();
        } else {
          this.toastr.error(res.message || 'Xóa file thất bại.');
        }
      });
  }
}
