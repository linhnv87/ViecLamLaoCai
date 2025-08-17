import { Component, HostListener, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { GroupDetailInputModel, GroupModel } from 'src/app/models/group.model';
import { GetAllInfoUserModel, SimplerRoleTempModel } from 'src/app/models/user.model';
import { ReviewerWorkflowModel, WorkFLowDetailInputModel } from 'src/app/models/work-flow.model';
import { GroupService } from 'src/app/services/admin/group.service';
import { WorkFlowService } from 'src/app/services/admin/work-flow.service';
import { UserService } from 'src/app/services/user.service';
import { displayUserRoles } from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-work-flow-detail',
  templateUrl: './work-flow-detail.component.html',
  styleUrls: ['./work-flow-detail.component.scss']
})
export class WorkFlowDetailComponent implements OnInit {

  @Input() workflows?: ReviewerWorkflowModel;
  public listSelectedUserWorkFlow: GetAllInfoUserModel[] = [];
  public listAllUserWorkFlow: GetAllInfoUserModel[] = [];
  searchSelectedUser = '';
  searchAllUser = '';
  selectedWorkFlowItem: any = null;
  workFlowDropdownOpen = false;
  workFlowSearchText = '';
  model: ReviewerWorkflowModel = {
    name: '',
    state: '',
    statusId: 0,
    userId: '',
    isSign: false,
    description: '',
    defaultUserId: '',
    nextWorkflowId: 0,
    prevWorkflowId: 0,
    usersDto: [],
  };
  userWorkFlowList: GetAllInfoUserModel[] = [];
  filteredUserWorkFlowData = [...this.userWorkFlowList];
  statusList = [
    { id: 1, name: 'Dự thảo' },
    { id: 2, name: 'Xin ý kiến' },
    { id: 3, name: 'Phê duyệt' },
    { id: 4, name: 'Ký số' },
    { id: 5, name: 'Chờ ban hành' },
    { id: 6, name: 'Ban hành' }
  ];
  constructor(
    private activeModal: NgbActiveModal,
    private toastr: ToastrService,
    private workflowService: WorkFlowService,
    private userService: UserService,
  ) { }

  ngOnInit(): void {
    this.getAllUser();
    this.getAllUserWorkFlow();
    if (this.workflows) {
      this.getListSelectedUser();
      this.model = { ...this.workflows };
    }
  }

  getAllUser() {
    this.userService.GetAllUserInfo().subscribe(res => {
      if (res.isSuccess) {
        this.listAllUserWorkFlow = res.result.map(user => {
          user.roleDescription = displayUserRoles(user.roles);
          return user;
        });
      }
    });
  }
  getAllUserWorkFlow() {
    this.userService.GetAllUserInfo().subscribe(res => {
      if (res.isSuccess) {
        this.userWorkFlowList = res.result.map(user => {
          user.roleDescription = displayUserRoles(user.roles);
          return user;
        });
        this.selectedWorkFlowItem = this.userWorkFlowList.find(item => item.userId === this.model.userId) || null;
      }
    });
  }
  getListSelectedUser() {
    this.workflowService.getGetUsersByWorkflowId(this.workflows?.id || 0).subscribe(res => {
      if (res.isSuccess) {
        this.listSelectedUserWorkFlow = res.result.map(user => {
          user.roleDescription = displayUserRoles(user.roles);
          return user;
        });
      }
    });
  }

  confirm(result: boolean) {
    this.activeModal.close(result);
  }

  onSave() {
    if (!this.isValidInput()) {
      return;
    }
    const ob$ = this.workflows
      ? this.workflowService.update(this.workflows.id || 0, this.prepareInput())
      : this.workflowService.create(this.prepareInput());
    ob$.subscribe({
      next: (res) => {
        if (res.isSuccess) {
          const message = this.workflows ? 'Cập nhật luồng thành công' : 'Tạo mới luồng thành công';
          this.toastr.success(message);
          this.activeModal.close(true);
        } else {
          this.toastr.error(res.message);
        }
      },
      error: (err) => {
        if (err.status === 409) {
          this.toastr.error('Đã có cấu hình luồng này rồi');
        } else {
          this.toastr.error(err.error?.message || 'Đã xảy ra lỗi');
        }
      }
    });
  }

  prepareInput(): WorkFLowDetailInputModel {
    return {
      ...this.model,
      usersDto: this.listSelectedUserWorkFlow.map(user => ({ userId: user.userId })),
    };
  }

  isValidInput(): boolean {
    const validations = [
      { condition: !this.model?.name?.trim(), message: 'Tên luồng không được để trống' },
      { condition: !this.model?.userId?.trim(), message: 'Cán bộ được cấu hình không được để trống' },
      { condition: !this.model?.state?.trim(), message: 'Thao tác không được để trống' },
      { condition: this.model?.statusId! <= 0, message: 'Trạng thái xử lý  không được để trống' },
      { condition: this.listSelectedUserWorkFlow.length === 0, message: 'Danh sách cán bộ không được để trống' }
    ];
    const isEmptyForm = validations.every(v => v.condition);

    if (isEmptyForm) {
      this.toastr.warning('Cần nhập đầy đủ thông tin');
      return false;
    }
    for (const validation of validations) {
      if (validation.condition) {
        this.toastr.warning(validation.message);
        return false;
      }
    }
    return true;
  }

  addUser(user: GetAllInfoUserModel) {
    if (this.model.userId === user.userId) {
      this.toastr.warning('Cán bộ đang được chọn cấu hình!');
      return;
    }

    if (!this.listSelectedUserWorkFlow.find(u => u.userId === user.userId)) {
      this.listSelectedUserWorkFlow.push(user);
    } else {
      this.toastr.warning('Cán bộ đã được chọn');
    }
  }

  addAllUser() {
    this.listAllUserWorkFlow.forEach(user => {
      if (this.model.userId === user.userId) {
        this.toastr.warning(`Không thể thêm ${user.userFullName} vì đã được chọn!`);
        return;
      }
      if (!this.listSelectedUserWorkFlow.find(u => u.userId === user.userId)) {
        this.listSelectedUserWorkFlow.push(user);
      }
    });
  }

  removeUser(user: GetAllInfoUserModel) {
    this.listSelectedUserWorkFlow = this.listSelectedUserWorkFlow.filter(u => u.userId !== user.userId);
  }

  removeAllUser() {
    this.listSelectedUserWorkFlow = [];
  }

  toggleWorkFLowDropdown(event: Event) {
    event.stopPropagation();
    if (!this.workFlowDropdownOpen) {
      this.filteredUserWorkFlowData = [...this.listAllUserWorkFlow];
    }
    this.workFlowDropdownOpen = !this.workFlowDropdownOpen;
  }

  selectWorkFlow(item: any, event: Event) {
    event.stopPropagation();
    const existingIndex = this.listSelectedUserWorkFlow.findIndex(u => u.userId === item.userId);
    if (existingIndex !== -1) {
      this.listSelectedUserWorkFlow.splice(existingIndex, 1);
    }
    this.selectedWorkFlowItem = item;
    this.model.userId = item.userId;
    this.workFlowDropdownOpen = false;
    this.workFlowSearchText = '';
  }

  filterWorkFlowOptions() {
    this.filteredUserWorkFlowData = this.listAllUserWorkFlow.filter(item =>
      item.userFullName?.toLowerCase().includes(this.workFlowSearchText.toLowerCase())
    );
  }

  onWorkFlowDropdownClick(event: Event) {
    event.stopPropagation();
  }

  @HostListener('document:click', ['$event'])
  closeDropdown() {
    this.workFlowDropdownOpen = false;
  }
}
