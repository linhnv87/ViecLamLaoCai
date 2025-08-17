import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { DocumentHistoryModel } from 'src/app/models/documentHistory.model';
import { DocumentHistoryService } from 'src/app/services/admin/document-history.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-document-history',
  templateUrl: './document-history.component.html',
  styleUrls: ['./document-history.component.css'],
})
export class DocumentHistoryComponent implements OnInit {
  itemsPerPage: number = 10;
  currentPage: number = 1;
  data: DocumentHistoryModel[] = [];
  userRole = common.GetRoleInfo();
  userId = common.GetCurrentUserId();

  constructor(
    private historyService: DocumentHistoryService,
    private toastrService: ToastrService,
    private modalService: NgbModal,
  ) {}
  ngOnInit(): void {
    this.getData();
  }

  getData() {
    if (this.userRole.banChapHanh) {
      const t = this.historyService.getAll().subscribe(res => {
        if (res.isSuccess) {
          this.data = res.result as DocumentHistoryModel[];
          console.log(this.data);
        }
      });
    } else if (this.userRole.chuyenVien) {
      const t = this.historyService.getByUserId(this.userId).subscribe(res => {
        if (res.isSuccess) {
          this.data = res.result as DocumentHistoryModel[];
          console.log(this.data);
        }
      });
    }
  }

  seeNote(item: DocumentHistoryModel) {
    const modalRef = this.modalService.open(ConfirmationComponent, {
      size: 'lg',
      backdrop: 'static',
    });
    modalRef.componentInstance.message = item?.note;
    modalRef.componentInstance.show = false;
  }
}
