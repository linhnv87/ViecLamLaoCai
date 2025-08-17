import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-plan-remind',
  templateUrl: './plan-remind.component.html',
  styleUrls: ['./plan-remind.component.scss'],
})
export class PlanRemindComponent {
  public dateEndApproval: string = '';
  public todayDate = new Date();
  public remendingDate?: Date;
  constructor(
    private activeModal: NgbActiveModal,
    private toastr: ToastrService,
  ) {}

  confirm(result: boolean) {
    if (result) {
      if (!this.remendingDate) {
        this.toastr.error('Vui lòng chọn thời gian nhắc lại');
      } else {
        this.activeModal.close(this.remendingDate);
      }
    } else {
      this.activeModal.close();
    }
  }
}
