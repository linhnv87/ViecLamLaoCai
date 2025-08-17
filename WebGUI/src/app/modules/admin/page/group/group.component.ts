import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupModel } from 'src/app/models/group.model';
import { GroupService } from 'src/app/services/admin/group.service';
import { GroupDetailComponent } from './group-detail/group-detail.component';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { filter, switchMap } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-group',
  templateUrl: './group.component.html',
  styleUrls: ['./group.component.scss'],
})
export class GroupComponent implements OnInit {
  groups: GroupModel[] = [];
  itemsPerPage: number = 10;
  currentPage: number = 1;
  searchText: string = '';
  constructor(
    private readonly router: Router,
    private readonly groupService: GroupService,
    private modalService: NgbModal,
    private toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.groupService.getAll().subscribe(res => {
      if (res.isSuccess) {
        this.groups = res.result;
      }
    });
  }

  gotoHome() {
    this.router.navigate(['/admin/Dashboard']);
  }

  editGroup(group: GroupModel) {
    const modalRef = this.modalService.open(GroupDetailComponent, { size: 'lg' });
    modalRef.componentInstance.group = group;
    modalRef.closed.subscribe(res => {
      if (res) {
        this.getList();
      }
    });
  }
  addGroup() {
    const modalRef = this.modalService.open(GroupDetailComponent, { size: 'lg' });
    modalRef.closed.subscribe(res => {
      if (res) {
        this.getList();
      }
    });
  }

  deleteGroup(group: GroupModel) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Bạn có chắc chắn muốn xoá nhóm?';
    modalRef.componentInstance.show = false;
    modalRef.closed
      .pipe(
        filter(r => !!r),
        switchMap(() => this.groupService.delete(group.id || 0)),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Xoá nhóm thành công!');
          this.getList();
        } else {
          this.toastr.error(res.message);
        }
      });
  }
  get filterGroupData() {
    return this.groups.filter(item =>
      item.groupName?.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }
}
