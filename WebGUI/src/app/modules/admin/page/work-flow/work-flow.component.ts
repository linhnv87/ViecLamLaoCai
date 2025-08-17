import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupModel } from 'src/app/models/group.model';
import { GroupService } from 'src/app/services/admin/group.service';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { filter, switchMap } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { WorkFlowDetailComponent } from './work-flow-detail/work-flow-detail.component';
import { ReviewerWorkflowModel } from 'src/app/models/work-flow.model';
import { WorkFlowService } from 'src/app/services/admin/work-flow.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-work-flow',
  templateUrl: './work-flow.component.html',
  styleUrls: ['./work-flow.component.scss']
})
export class WorkFlowComponent implements OnInit {
  workflowData: ReviewerWorkflowModel[] = [];
  userMap: { [key: string]: string } = {};
  itemsPerPage: number = 10;
  currentPage: number = 1;
  stateMap: Record<string, string> = {
    'chuyen-xu-ly': 'Chuyển xử lý',
    'chuyen-ky-so': 'Chuyển ký số',
    'xin-y-kien-lai': 'Xin ý kiến lại',
    'tra-ve': 'Trả về'
  };
  statusNameMap: { [key: number]: string } = {
    1: 'Dự thảo',
    2: 'Xin ý kiến',
    3: 'Phê duyệt',
    4: 'Ký số',
    5: 'Chờ ban hành',
    6: 'Ban hành'
  };
  searchText: string = '';
  constructor(
    private readonly router: Router,
    private readonly workflowService: WorkFlowService,
    private modalService: NgbModal,
    private toastr: ToastrService,
    private userService: UserService,
  ) {}

  ngOnInit(): void {
    this.getUsers();
    this.getList();
  }

  getStatusName(statusId: number): string {
    return this.statusNameMap[statusId] || '';
  }

  getStateLabel(state: string): string {
    return this.stateMap[state] || state; }
  getList() {
    this.workflowService.getAll().subscribe(res => {
      if (res.isSuccess) {
        this.workflowData = res.result;
      }
    });
  }

  getUsers() {
    this.userService.GetAllUserInfo().subscribe(res => {
      if (res.isSuccess) {
        this.userMap = res.result.reduce((map, user) => {
          map[user.userId] = user.userFullName; 
          return map;
        }, {} as { [key: string]: string });
      }
    });
  }
  gotoHome() {
    this.router.navigate(['/admin/Dashboard']);
  }

  editWorkFlow(workflows: ReviewerWorkflowModel) {
    const modalRef = this.modalService.open(WorkFlowDetailComponent, { size: 'lg' });
    modalRef.componentInstance.workflows = workflows;
    modalRef.closed.subscribe(res => {
      if (res) {
        this.getList();
      }
    });
  }

  addWorkFlow() {
    const modalRef = this.modalService.open(WorkFlowDetailComponent, { size: 'lg' });
    modalRef.closed.subscribe(res => {
      if (res) {
        this.getList();
      }
    });
  }

  deleteWorkFlow(group: ReviewerWorkflowModel) {
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Bạn có chắc chắn muốn xoá luồng?';
    modalRef.componentInstance.show = false;
    modalRef.closed
      .pipe(
        filter(r => !!r),
        switchMap(() => this.workflowService.delete(group.id || 0)),
      )
      .subscribe(res => {
        if (res.isSuccess) {
          this.toastr.success('Xoá luồng thành công!');
          this.getList();
        } else {
          this.toastr.error(res.message);
        }
      });
  }

  get filteredWorkflowData() {
    return this.workflowData.filter(item =>
      item.name?.toLowerCase().includes(this.searchText.toLowerCase()) || 
      (this.userMap[item.userId ?? '']?.toLowerCase() || '').includes(this.searchText.toLowerCase()) 
    );
  }
}
