import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { GroupDetailInputModel, GroupModel } from 'src/app/models/group.model';
import { GetAllInfoUserModel, SimplerRoleTempModel } from 'src/app/models/user.model';
import { GroupService } from 'src/app/services/admin/group.service';
import { UserService } from 'src/app/services/user.service';
import { displayUserRoles } from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.scss'],
})
export class GroupDetailComponent implements OnInit {
  @Input() group?: GroupModel;
  public listSelectedUser: GetAllInfoUserModel[] = [];
  public listAllUser: GetAllInfoUserModel[] = [];
  searchSelectedUser = '';
  searchAllUser = '';
  model: GroupModel = {
    groupName: '',
    isActive: false,
    isSMS: false,
  };
  constructor(
    private activeModal: NgbActiveModal,
    private toastr: ToastrService,
    private groupService: GroupService,
    private userService: UserService,
  ) {}

  ngOnInit(): void {
    this.getAllUser();
    if (this.group) {
      this.getListSelectedUser();
      this.model = { ...this.group };
    }
  }

  getAllUser() {
    this.userService.GetAllUserInfo().subscribe(res => {
      if (res.isSuccess) {
        this.listAllUser = res.result.map(user => {
          user.roleDescription = displayUserRoles(user.roles);
          return user;
        });
      }
    });
  }
  getListSelectedUser() {
    this.groupService.getUserOfGroup(this.group?.id || 0).subscribe(res => {
      if (res.isSuccess) {
        this.listSelectedUser = res.result.map(user => {
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
    const ob$ = this.group
      ? this.groupService.update(this.group.id || 0, this.prepareInput())
      : this.groupService.create(this.prepareInput());
    ob$.subscribe(res => {
      if (res.isSuccess) {
        const message = this.group ? 'Cập nhật nhóm thành công' : 'Tạo mới nhóm thành công';
        this.toastr.success(message);
        this.activeModal.close(true);
      } else {
        this.toastr.error(res.message);
      }
    });
  }

  prepareInput(): GroupDetailInputModel {
    return {
      ...this.model,
      groupDetails: this.listSelectedUser.map(user => ({ userId: user.userId })),
    };
  }

  isValidInput(): boolean {
    if (this.model.groupName === '') {
      this.toastr.warning('Tên nhóm không được để trống');
      return false;
    }
    if (this.listSelectedUser.length === 0) {
      this.toastr.warning('Danh sách cán bộ không được để trống');
      return false;
    }
    return true;
  }

  addUser(user: GetAllInfoUserModel) {
    if (!this.listSelectedUser.find(u => u.userId === user.userId)) {
      this.listSelectedUser.push(user);
    } else {
      this.toastr.warning('Cán bộ đã được chọn');
    }
  }

  addAllUser() {
    this.listAllUser.forEach(user => {
      if (!this.listSelectedUser.find(u => u.userId === user.userId)) {
        this.listSelectedUser.push(user);
      }
    });
  }

  removeUser(user: GetAllInfoUserModel) {
    this.listSelectedUser = this.listSelectedUser.filter(u => u.userId !== user.userId);
  }

  removeAllUser() {
    this.listSelectedUser = [];
  }
}
