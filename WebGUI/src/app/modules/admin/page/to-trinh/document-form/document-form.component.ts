import { Component, HostListener, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { filter, finalize, forkJoin, switchMap, tap } from 'rxjs';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { DocumentTypeModel } from 'src/app/models/document-type.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { FieldModel } from 'src/app/models/field.model';
import { GroupModel } from 'src/app/models/group.model';
import { GetAllInfoUserModel, SimplerRoleTempModel } from 'src/app/models/user.model';
import { DocumentFileService } from 'src/app/services/admin/document-file.service';
import { DocumentTypeService } from 'src/app/services/admin/document-type.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import { GroupService } from 'src/app/services/admin/group.service';
import { LoadingService } from 'src/app/services/loading.service';
import { NotificationService } from 'src/app/services/nofitication.service';
import * as common from 'src/app/utils/commonFunctions';
import { DOCUMENT_STATUS } from 'src/app/utils/constants';

@Component({
  selector: 'app-document-form',
  templateUrl: './document-form.component.html',
  styleUrls: ['./document-form.component.scss'],
})
export class DocumentFormComponent implements OnInit {
  userRoles = common.GetRoleInfo();
  documentStatus = DOCUMENT_STATUS;
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  data: DocumentModel = {
    id: 0,
    title: '',
    note: '',
    fieldId: 0,
    typeId: 0,
    assignedRoles: [],
    dateEndApproval: new Date(),
    remindDatetime: new Date(),
    deleted: false,
    created: new Date(),
    createdBy: this.userId,
    modified: new Date(),
    modifiedBy: this.userId,
  };
  showRemendingDate = false;
  selectedFileNamesArray: string[] = [];
  fieldData: FieldModel[] = [];
  CreateDate = moment(this.data.created).format('YYYY-MM-DDTHH:mm');
  DateEndApproval = this.data.dateEndApproval;
  remendingDate = this.data.remindDatetime;
  fileServer: DocumentFileModel[] = [];
  files: File[] = [];
  sideFiles: File[] = [];
  selectedFileNames: string = 'Chưa có file nào được chọn.';
  selectImg: boolean = false;
  typeOptions: DocumentTypeModel[] = [];
  todayDate = new Date();
  displayedColumns: string[] = ['select', 'name'];
  groupOptions: GroupModel[] = [];
  groupOptionsSMS: GroupModel[] = [];
  selectedDepartment = 0;
  selectedDepartmentSMS = 0;
  listUserOfGroup: GetAllInfoUserModel[] = [];
  listSelectedUser: GetAllInfoUserModel[] = [];
  listUserOfGroupSMS: GetAllInfoUserModel[] = [];
  listSelectedUserSMS: GetAllInfoUserModel[] = [];
  public documentId = '';
  dropdownOpen = false;
  searchText: string = '';
  searchAllUser = '';
  searchAllUserSMS = '';
  searchSelectedUser = '';
  searchSelectedUserSMS = '';
  selectedItem: any = null;
  selectedField: any = null;
  fieldDropdownOpen = false;
  fieldSearchText = '';
  filteredFieldData = [...this.fieldData];
  filteredOptions = [...this.typeOptions];
  isShowSmsList = false;
  listSmsUser = [];
  listSelectedSmsUser = [];
  constructor(
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private router: Router,
    private fieldService: FieldServiceService,
    private toastr: ToastrService,
    private loadingService: LoadingService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private documentTypeService: DocumentTypeService,
    private groupService: GroupService,
    private documentFileService: DocumentFileService,
  ) { }

  ngOnInit(): void {
    this.initOptions();
    this.loadData();
  }

  private loadData() {
    this.route.paramMap.subscribe(params => {
      this.documentId = params.get('id') || '';
      if (this.documentId) {
        forkJoin([
          this.documentService.getDocumentsById(Number(this.documentId)),
          this.documentService.GetDocumentAttachmentsById(Number(this.documentId)),
        ]).subscribe(([resDocument, resFiles]) => {
          if (resDocument.isSuccess) {
            this.data = resDocument.result;
            this.DateEndApproval = this.data.dateEndApproval;
            this.remendingDate = this.data.remindDatetime ?? new Date();
            this.listSelectedUser = this.data.assignedUsers || [];
            this.listSelectedUserSMS = this.data.smsUsers || [];
            this.selectedItem = this.typeOptions.find(item => item.id === this.data.typeId) || null;
            this.selectedField = this.fieldData.find(item => item.id === this.data.fieldId) || null;
          }
          if (resFiles.isSuccess) {
            this.fileServer = resFiles.result;
          }
        });
      }
    });
  }

  initOptions() {
    forkJoin([
      this.fieldService.getAll(),
      this.documentTypeService.getAll(),
      this.groupService.getAll(),
    ]).subscribe(([resField, resType, resGroup]) => {
      if (resField.isSuccess) {
        this.fieldData = resField.result as FieldModel[];
      } else {
        this.toastr.error(resField.message);
      }

      if (resType.isSuccess) {
        this.typeOptions = resType.result as DocumentTypeModel[];
      } else {
        this.toastr.error(resType.message);
      }

      if (resGroup.isSuccess) {
        this.groupOptions = resGroup.result.filter(i => i.isActive && !i.isSMS);
        this.groupOptionsSMS = resGroup.result.filter(i => i.isActive && i.isSMS);
      } else {
        this.toastr.error(resGroup.message);
      }
    });
  }

  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.files = this.files.concat(newFiles);
        const newFileNames = newFiles.map(file => file.name);
        this.selectedFileNamesArray = this.selectedFileNamesArray.concat(newFileNames);
        this.selectImg = true;
      }
    }
    inputElement.value = '';
  }
  onSideFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.sideFiles = this.sideFiles.concat(newFiles);
      }
    }
    inputElement.value = '';
  }

  moveFileUp(index: number): void {
    if (index > 0) {
      const temp = this.files[index];
      this.files[index] = this.files[index - 1];
      this.files[index - 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }

  moveSideFileUp(index: number): void {
    if (index > 0) {
      const temp = this.sideFiles[index];
      this.sideFiles[index] = this.sideFiles[index - 1];
      this.sideFiles[index - 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }

  moveFileDown(index: number): void {
    if (index < this.files.length - 1) {
      const temp = this.files[index];
      this.files[index] = this.files[index + 1];
      this.files[index + 1] = temp;
      this.updateSelectedFileNamesArray();
    }
  }
  moveSideFileDown(index: number): void {
    if (index < this.sideFiles.length - 1) {
      const temp = this.sideFiles[index];
      this.sideFiles[index] = this.sideFiles[index + 1];
      this.sideFiles[index + 1] = temp;
      // this.updateSelectedFileNamesArray();
    }
  }

  deleteFile(index: number): void {
    this.files.splice(index, 1);
    this.updateSelectedFileNamesArray();
  }

  deleteSideFile(index: number): void {
    this.sideFiles.splice(index, 1);
    // this.updateSelectedFileNamesArray();
  }

  updateSelectedFileNamesArray(): void {
    this.selectedFileNamesArray = this.files.map(file => file.name);
  }
  resetData(): void {
    this.data = {
      id: 0,
      title: '',
      note: '',
      fieldId: 0,
      dateEndApproval: new Date(),
      remindDatetime: new Date(),
      deleted: false,
      created: new Date(),
      createdBy: this.userId,
      modified: new Date(),
      modifiedBy: this.userId,
    };
    this.selectedFileNamesArray = [];
    this.files = [];
  }

  processTransition() {
    this.data.dateEndApproval = this.DateEndApproval;
    this.data.remindDatetime = this.remendingDate;
    const invalidForm = this.validateForm();
    if (invalidForm) {
      this.toastr.warning('Cần nhập đầy đủ thông tin');
      return;
    }
    if (!this.validateDate()) {
      return;
    }

    if (!this.showRemendingDate) {
      this.data.remindDatetime = undefined;
    }
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      if (result) {
        this.loadingService.setLoading(true);

        this.updateDocument$()
          .pipe(finalize(() => this.loadingService.setLoading(false)))
          .subscribe(res => {
            if (res.isSuccess) {
              this.navigateToComponentBWithParam();
            }
          });
      }
    });
  }

  submit(sendToApprove: boolean) {
    const invalidForm = this.validateForm();
    if (invalidForm) {
      this.toastr.warning('Cần nhập đầy đủ thông tin');
      return;
    }
    if (!this.validateDate()) {
      return;
    }
    if (this.isShowSmsList && (!this.listSelectedUserSMS || this.listSelectedUserSMS.length === 0)) {
      this.toastr.warning('Vui lòng chọn ít nhất một cán bộ nhận SMS!');
      return;
    }
  
    this.data.dateEndApproval = this.DateEndApproval;
    this.data.remindDatetime = this.remendingDate;
    this.data.users = this.listSelectedUser.map(user => user.userId);
    this.data.usersSMS = this.isShowSmsList
    ? this.listSelectedUserSMS.map(user => user.userId)
    : [];
    if (!this.showRemendingDate) {
      this.data.remindDatetime = undefined;
    }
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      if (result) {
        this.loadingService.setLoading(true);
        let ob$ = this.updateDocument$();
        if (!this.documentId) {
          ob$ = sendToApprove ? this.createAndSend$() : this.createDraft$();
        }
        ob$.pipe(finalize(() => this.loadingService.setLoading(false))).subscribe(res => {
          if (res.isSuccess) {
            if (sendToApprove) {
              this.notificationService.CreateNotification(1, res.result.id).subscribe(res => {
                if (res.isSuccess) {
                  this.toastr.info('Thông báo gửi duyệt đã được gửi cho cán bộ');
                }
              });
              this.notificationService.SendSMSV2(res.result.id, 1).subscribe(res => {
                console.log(res);
                if (res.isSuccess) {
                  this.toastr.info('Tin nhắn thông báo đã được gửi cho cán bộ');
                }
              });
            }
            this.navigateToComponentBWithParam();
          }
        });
      }
    });
  }

  createAndSend$() {
    this.data.statusCode = DOCUMENT_STATUS.XIN_Y_KIEN;
    return this.documentService.createAndSend(this.data, this.files, this.sideFiles).pipe(
      finalize(() => {
        this.toastr.success('Tờ trình đã được khởi tạo và gửi xin ý kiến');
      }),
    );
  }
  createDraft$() {
    this.data.statusCode = DOCUMENT_STATUS.DU_THAO;
    return this.documentService.createDraft(this.data, this.files, this.sideFiles).pipe(
      finalize(() => {
        this.toastr.success('Tờ trình đã được khởi tạo và tạm lưu');
      }),
    );
  }
  updateDocument$() {
    if (this.data.statusCode === DOCUMENT_STATUS.PHE_DUYET) {
      return this.documentService.update(this.data, this.files, this.sideFiles).pipe(
        finalize(() => {
          this.toastr.success('Tờ trình đã chuyển xử lý thành công');
        }),
      );
    } else {
      this.data.statusCode = DOCUMENT_STATUS.XIN_Y_KIEN;
      return this.documentService.update(this.data, this.files, this.sideFiles).pipe(
        finalize(() => {
          this.toastr.success('Tờ trình đã được gửi xin ý kiến lại thành công');
        }),
      );
    }
  }
  navigateToComponentBWithParam() {
    this.router.navigate(['/admin/Dashboard']);
  }

  validateForm(): boolean {
    const inValidFile =
      !this.files.length &&
      (!this.fileServer.length || !this.fileServer?.filter(i => i.fileType === 1)?.length);
    return (
      this.data.title?.trim() == '' ||
      this.data.fieldId == 0 ||
      this.data.note?.trim() == '' ||
      this.data.typeId == 0 ||
      this.listSelectedUser.length == 0 ||
      inValidFile
    );
  }

  validateDate() {

    const now = moment(); 
    const validDateApproval = moment(this.DateEndApproval).isAfter(now); 
    const validDateReminding = this.showRemendingDate
      ? moment(this.remendingDate).isBetween(
        now,
        moment(this.DateEndApproval),
        null,
        '[)'
      )
      : true;

    if (!validDateApproval) {
      this.toastr.warning('Hạn phê duyệt phải lớn hơn ngày tạo');
    }

    if (!validDateReminding) {
      this.toastr.warning('Thời gian nhắc phải lớn hơn ngày tạo và nhỏ hơn hạn phê duyệt');
    }

    return validDateApproval && validDateReminding;
  }

  onChangeDepartment(data: number) {
    if (data) {
      this.groupService.getUserOfGroup(data).subscribe(res => {
        if (res.isSuccess) {
          this.listUserOfGroup = res.result;
        } else {
          this.toastr.error(res.message);
        }
      });
    }
  }

  onChangeDepartmentSMS(data: number) {
    if (data) {
      this.groupService.getUserOfGroup(data).subscribe(res => {
        if (res.isSuccess) {
          this.listUserOfGroupSMS = res.result;
        } else {
          this.toastr.error(res.message);
        }
      });
    }
  }

  addUser(user: GetAllInfoUserModel) {
    if (!this.listSelectedUser.find(u => u.userId === user.userId)) {
      this.listSelectedUser.push(user);
    } else {
      this.toastr.warning('Cán bộ đã được chọn');
    }
  }
  addUserSMS(user: GetAllInfoUserModel) {
    if (!this.listSelectedUserSMS.find(u => u.userId === user.userId)) {
      this.listSelectedUserSMS.push(user);
    } else {
      this.toastr.warning('Cán bộ đã được chọn');
    }
  }

  addAllUser() {
    this.listUserOfGroup.forEach(user => {
      if (!this.listSelectedUser.find(u => u.userId === user.userId)) {
        this.listSelectedUser.push(user);
      }
    });
  }

  removeUser(user: GetAllInfoUserModel) {
    this.listSelectedUser = this.listSelectedUser.filter(u => u.userId !== user.userId);
  }
  removeUseSMS(user: GetAllInfoUserModel) {
    this.listSelectedUserSMS = this.listSelectedUserSMS.filter(u => u.userId !== user.userId);
  }
  removeAllUserSMS() {
    this.listSelectedUserSMS = [];
  }

  removeAllUser() {
    this.listSelectedUser = [];
  }

  displayUserRoles(roles?: SimplerRoleTempModel[]): string {
    const roleVales = roles?.map(role => role.description).join(', ') || '';
    return roleVales ? `(${roleVales})` : '';
  }

  dateApprovedChange() {
    if (moment(this.todayDate).isAfter(moment(this.DateEndApproval))) {
      this.toastr.warning('Hạn phê duyệt phải lớn hơn ngày tạo');
      setTimeout(() => {
        this.DateEndApproval = this.todayDate;
      });
    }
  }

  dateRemendingChange() {
    if (moment(this.todayDate).isAfter(moment(this.remendingDate))) {
      this.toastr.warning('Thời gian nhắc phải lớn hơn ngày tạo');
      setTimeout(() => {
        this.remendingDate = this.todayDate;
      });
    } else if (moment(this.remendingDate).isAfter(moment(this.DateEndApproval))) {
      this.toastr.warning('Thời gian nhắc phải nhỏ hơn hạn phê duyệt');
      setTimeout(() => {
        this.remendingDate = this.DateEndApproval;
      });
    }
  }

  deleteFileServer(id: number) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Bạn có chắc chắn muốn xóa văn bản này không?';
    modalRef.componentInstance.show = false;

    modalRef.closed
      .pipe(
        filter(r => !!r),
        switchMap(() => this.documentFileService.delete(id)),
        switchMap(() => this.documentService.GetDocumentAttachmentsById(Number(this.documentId))),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.fileServer = res.result;
          this.toastr.success('Xóa file thành công!');
        } else {
          this.toastr.error(res.message || 'Xóa file thất bại.');
        }
      });
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    if (!this.dropdownOpen) {
      this.filteredOptions = [...this.typeOptions];
    }
    this.dropdownOpen = !this.dropdownOpen;
  }

  filterOptions() {
    this.filteredOptions = this.typeOptions.filter(item =>
      item.name.toLowerCase().includes(this.searchText.toLowerCase()),
    );
  }

  onDropdownClick(event: Event) {
    event.stopPropagation();
  }

  selectItem(item: any, event: Event) {
    event.stopPropagation();
    this.selectedItem = item;
    this.data.typeId = item.id;
    this.dropdownOpen = false;
    this.searchText = '';
  }

  toggleFieldDropdown(event: Event) {
    event.stopPropagation();
    if (!this.fieldDropdownOpen) {
      this.filteredFieldData = [...this.fieldData];
    }
    this.fieldDropdownOpen = !this.fieldDropdownOpen;
  }

  selectField(item: any, event: Event) {
    event.stopPropagation();
    this.selectedField = item;
    this.data.fieldId = item.id;
    this.fieldDropdownOpen = false;
    this.fieldSearchText = '';
  }

  filterFieldOptions() {
    this.filteredFieldData = this.fieldData.filter(item =>
      item.title?.toLowerCase().includes(this.fieldSearchText.toLowerCase()),
    );
  }

  onFieldDropdownClick(event: Event) {
    event.stopPropagation();
  }

  @HostListener('document:click', ['$event'])
  closeDropdown() {
    this.fieldDropdownOpen = false;
    this.dropdownOpen = false;
  }
}
