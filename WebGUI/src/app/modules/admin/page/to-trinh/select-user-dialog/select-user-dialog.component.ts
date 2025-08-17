import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { DocumentModel } from 'src/app/models/document.model';
import { SimplerRoleTempModel } from 'src/app/models/user.model';
import { UserWorkFlowModel } from 'src/app/models/work-flow.model';
import { WorkFlowService } from 'src/app/services/admin/work-flow.service';
import { displayUserRoles } from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-select-user-dialog',
  templateUrl: './select-user-dialog.component.html',
  styleUrls: ['./select-user-dialog.component.scss'],
})
export class SelectUserDialogComponent implements OnInit {
  @Input() state = '';
  @Input() document?: DocumentModel;

  public listUser: UserWorkFlowModel[] = [];
  constructor(
    private activeModal: NgbActiveModal,
    private workFlowService: WorkFlowService,
    private toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    this.getUserWorkFlow();
  }

  confirm(result: boolean) {
    this.activeModal.close(result);
  }
  onSave() {
    if (!this.validateListUserSend()) {
      return;
    }
    const userIds = this.listUser?.filter(i => i.isDefault)?.map(x => x.userId || '');
    this.activeModal.close(userIds);
  }

  changeUserSend(item: UserWorkFlowModel, checked: boolean) {
    this.listUser.forEach(data => {
      if (checked) {
        data.isDefault = data.userId === item.userId;
      }
    });
  }
  public validateListUserSend() {
    const valid = this.listUser.some(x => x.isDefault);
    if (!valid) {
      this.toastr.error('Vui lòng chọn người nhận');
    }
    return valid;
  }

  public getUserWorkFlow() {
    this.workFlowService
      .getWorkFlowByStatus(this.document?.statusCode as string, this.state)
      .subscribe(res => {
        if (res.isSuccess) {
          let users = res.result?.usersDto || [];
          if (this.state === 'xin-y-kien-lai' && this.document?.createdBy) {
            users = users.filter(u => u.userId === this.document?.createdBy);
          }
  
          this.listUser = users.map((item, index) => {
            item.isDefault = index === 0;
            item.roleDescription = displayUserRoles(item.roles, true);
            return item;
          });
        }
      });
  }
  
}
