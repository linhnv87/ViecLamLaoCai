import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import * as common from 'src/app/utils/commonFunctions';
import { DepartmentService } from 'src/app/services/admin/department.service';
import { DepartmentModel } from 'src/app/models/department.model';
import { UnitService } from 'src/app/services/admin/unit.service';
import { UnitModel } from 'src/app/models/unit.model';

@Component({
  selector: 'app-unit',
  templateUrl: './unit.component.html',
  styleUrls: ['./unit.component.scss']
})
export class UnitComponent {



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
  unitData: UnitModel[] = [];
  constructor(private route: ActivatedRoute, private router: Router, private unitSerive: UnitService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private modalService: NgbModal
  ) {
    this.loadData();
  }
  loadData() {
    this.unitSerive.getAll().subscribe((res) => {
      if (res.isSuccess) {
        this.unitData = res.result as UnitModel[];
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
          const deletePromise = this.unitSerive.delete(item).subscribe(res => {
            if (res.isSuccess) {
              this.toastr.success('Xóa đơn vị thành công')
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
  get filteredUnitData() {
    return this.unitData.filter(doc =>
      doc.name?.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }
  itemsPerPage: number = 10;
  currentPage: number = 1;
}
