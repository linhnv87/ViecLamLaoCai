import { Component, OnInit } from '@angular/core';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import { CommunicationService } from 'src/app/services/communication.service';
import * as common from 'src/app/utils/commonFunctions';
import { DOCUMENT_STATUS } from 'src/app/utils/constants';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  userRoles = common.GetRoleInfo();
  documents: DocumentModel[] = [];
  dashboardInfo: any = {};

  constructor(
    private documentService: DocumentService,
    private communicationService: CommunicationService,
  ) {}

  ngOnInit(): void {
    this.communicationService.triggerReloadFunction();
    this.getData();
  }

  getData() {
    this.documentService.getList().subscribe(res => {
      if (res.isSuccess) {
        this.documents = res.result;
        if (this.userRoles.chuyenVien) {
          this.documents = this.documents.filter(x => x.createdBy == this.userId);
        }
        if (this.userRoles.vanThu) {
          const fields = common.GetCurrentUserInfo().fieldIds;
          this.documents = this.documents.filter(x => fields.includes(x.fieldId));
        }

        this.documents = res.result;

        this.dashboardInfo.totalCount = this.documents.length;
        this.dashboardInfo.duThaoCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.DU_THAO,
        ).length;
        this.dashboardInfo.xinYKienCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.XIN_Y_KIEN,
        ).length;
        this.dashboardInfo.pheDuyetCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.PHE_DUYET,
        ).length;
        this.dashboardInfo.choKySoCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.KY_SO,
        ).length;
        this.dashboardInfo.choBanHanhCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.CHO_BAN_HANH,
        ).length;
        this.dashboardInfo.banHanhcount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.BAN_HANH,
        ).length;
        this.dashboardInfo.khongBanHanhCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.KHONG_BAN_HANH,
        ).length;
        this.dashboardInfo.traLaiCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.TRA_LAI,
        ).length;
      }
    });
  }

  itemsPerPage: number = 10;
  currentPage: number = 1;
}
