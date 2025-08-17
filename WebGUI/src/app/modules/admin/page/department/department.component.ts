import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import * as common from 'src/app/utils/commonFunctions';
import { DepartmentService } from 'src/app/services/admin/department.service';
import { DepartmentModel } from 'src/app/models/department.model';


@Component({
  selector: 'app-department',
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.scss']
})
export class DepartmentComponent {



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
  departmentData: DepartmentModel[] = [];
  constructor(private route: ActivatedRoute, private router: Router, private departmentService: DepartmentService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private modalService: NgbModal
  ) {
    this.loadData();
  }
  loadData() {
    this.departmentService.getAll().subscribe((res) => {
      if (res.isSuccess) {
        this.departmentData = res.result as DepartmentModel[];
      } else {
        this.toastr.error(res.message);
      }
    });
  }
  delete(item: number) {
    if (item) {
      const modalRef = this.modalService.open(ConfirmationComponent);
      modalRef.result.then(result => {
        if(result){
          const deletePromise = this.departmentService.delete(item).subscribe(res => {
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
  actionOnItem = 0;
  toggleActions(id: number) {
    if (this.actionOnItem != id) {
      this.actionOnItem = id;
    } else {
      this.actionOnItem = 0;
    }
  }
  get filteredDepartmentData() {
    return this.departmentData.filter(doc =>
      doc.departmentName?.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }
  itemsPerPage: number = 10;
  currentPage: number = 1;
}
