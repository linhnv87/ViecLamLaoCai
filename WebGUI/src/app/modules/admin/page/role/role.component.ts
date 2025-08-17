import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { DepartmentService } from 'src/app/services/admin/department.service';
import { DepartmentModel } from 'src/app/models/department.model';
import { RoleService } from 'src/app/services/role.service';
import { GetAllRoleModel } from 'src/app/models/role.model';


@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss']
})
export class RoleComponent {
  userRoles = common.GetRoleInfo();
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  itemId: string | undefined | null;
  isModalOpen: boolean = false;  
  waitForResponse = false;

  files: File[] = [];
  statusCode?: number;
  openModal() {
    this.isModalOpen = true;
  }
  closeModal() {
    this.isModalOpen = false;
  }
  searchText: string = '';
  roleData: GetAllRoleModel[] = [];
  constructor(private route: ActivatedRoute, private router: Router, private roleService: RoleService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private modalService: NgbModal
  ) {
    this.loadData();
  }
  loadData() {
    this.roleService.GetAllRoleInfo().subscribe((res) => {
      if (res.isSuccess) {
        this.roleData = res.result as GetAllRoleModel[];
      } else {
        this.toastr.error(res.message);
      }
    });
  }
  delete(item: string) {
    if (item) {
      const modalRef = this.modalService.open(ConfirmationComponent);
      modalRef.result.then(result => {
        if(result){
          const deletePromise = this.roleService.delete(item).subscribe(res => {
            if (res.isSuccess) {
              this.toastr.success('Xóa tờ trình thành công')
              this.loadData();
            } else {
              this.toastr.error(res.message);
            }
          })
        }
      })

    }
  }
  actionOnItem = "";
  toggleActions(id: string) {
    if (this.actionOnItem != id) {
      this.actionOnItem = id;
    } else {
      this.actionOnItem = "";
    }
  }
  get filteredRoleData() {
    return this.roleData.filter(doc =>
      doc.description?.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }
  itemsPerPage: number = 10;
  currentPage: number = 1;
}
