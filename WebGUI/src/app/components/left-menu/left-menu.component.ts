import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { AdminMenu, menuItems, MenuModel, SystemMenu } from 'src/app/config/admin-menu';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import { CommunicationService } from 'src/app/services/communication.service';
import { UserService } from 'src/app/services/user.service';
import * as common from 'src/app/utils/commonFunctions';
import { DOCUMENT_STATUS, ROLE_OPTIONS, ROLES } from 'src/app/utils/constants';

@Component({
  selector: 'app-left-menu',
  templateUrl: './left-menu.component.html',
  styleUrls: ['./left-menu.component.css'],
})
export class LeftMenuComponent implements OnInit, OnChanges {
  userRoles = common.GetRoleInfo();
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();

  rolesData: any[] = [];
  menuItems = menuItems as MenuModel[];
  systemItems = SystemMenu as MenuModel[];
  reportItems = [
    {
      path: '/admin/report/y-kien',
      text: 'Thống kê ý kiến',
      icon: 'fas fa-layer-group',
      children: [],
      permissions:[ROLES.CHANH_VAN_PHONG, ROLES.PHO_CHANH_VAN_PHONG, ROLES.PHO_BI_THU,ROLES.BI_THU],
    },
    {
      path: '/admin/report/to-trinh',
      text: 'Thống kê tờ trình',
      icon: 'fas fa-layer-group',
      children: [],
      permissions:[ROLES.CHANH_VAN_PHONG, ROLES.PHO_CHANH_VAN_PHONG, ROLES.PHO_BI_THU,ROLES.BI_THU],
    },
  ] as MenuModel[];
  guiItems = [
    {
      icon: 'fas fa-book',
      tooltip: 'Hướng dẫn sử dụng',
      text: 'Dành cho Cán bộ được xin ý kiến',
      href: '/assets/Images/HDSD phần mềm Biểu quyết TU.pdf',
      
    },
    {
      icon: 'fas fa-book',
      tooltip: 'Hướng dẫn sử dụng',
      text: 'Dành cho Chuyên viên; Phó Trưởng phòng; Trưởng phòng; Phó chánh văn phòng',
      href: '/assets/Images/HDSD phần mềm Biểu quyết TU.pdf',
    },
  ];

  contactItems = [
    {
      icon: 'fas fa-phone',
      tooltip: 'Gọi cho Đc Huy',
      text: 'Đc Huy: 0389927661',
      href: 'tel:0389927661',
    },
    {
      icon: 'fas fa-phone',
      tooltip: 'Gọi cho Đc Kiều Anh',
      text: 'Đc Kiều Anh: 0946610776',
      href: 'tel:0946610776',
    },
    // {
    //   icon: 'fas fa-phone',
    //   tooltip: 'Gọi cho Đc Phương',
    //   text: 'Đc Phương: 0945527218',
    //   href: 'tel:0945527218',
    // },
  ];
  constructor(
    private documentService: DocumentService,
    private communicationService: CommunicationService,
    private userService: UserService,
    private router: Router,
  ) {
    this.communicationService.reloadFunction$.subscribe(() => {
      this.getData();
    });
  }

  ngOnInit(): void {
    this.formatData();
    this.checkLogin();
    this.getData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.getData();
  }

  // toggleRole(selectedItem: string) {
  //   this.userInfo.selectedRole = selectedItem;
  //   const userInfo = JSON.stringify(this.userInfo);
  //   localStorage.setItem(LOCAL_STORAGE_KEYS.USER_INFO, userInfo);
  //   window.location.reload();
  // }

  formatData() {
    this.rolesData = this.userInfo.roles?.map((role: string) => {
      return {
        role,
        text: ROLE_OPTIONS.find(x => x.value == role)?.text,
      };
    });
    // this.rolesData = ROLE_OPTIONS.map((role: any) => {
    //   return {
    //     role: role.value,
    //     text: role.text,
    //   };
    // });
  }

  documents: DocumentModel[] = [];
  dashboardInfo: any = {};

  getData() {
    this.documentService.getList(undefined, true).subscribe(res => {
      if (res.isSuccess) {
        this.documents = res.result;
        //Filter documents by current role when switching admin's role
        //
        this.dashboardInfo.totalCount = this.documents.filter(
          x => x.statusCode !== DOCUMENT_STATUS.DU_THAO,
        ).length;
        this.dashboardInfo.duThaoCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.DU_THAO,
        ).length;
        this.dashboardInfo.xinYKienCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.XIN_Y_KIEN,
        ).length;
        this.dashboardInfo.xinYKienLaiCount = this.documents.filter(
          x => x.statusCode == DOCUMENT_STATUS.XIN_Y_KIEN_LAI,
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
  checkLogin() {
    const userInfo = this.userService.GetLocalStorageUserInfo();
    if (userInfo == null) this.router.navigate(['/auth']);
    else {
      if (this.userRoles.admin) {
        this.menuItems = AdminMenu as MenuModel[];
      } else {
        this.menuItems = menuItems as MenuModel[];
      }
    }
  }
}
