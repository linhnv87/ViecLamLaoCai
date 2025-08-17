import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationModel } from 'src/app/models/notification.model';
import { NotificationService } from 'src/app/services/nofitication.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-modal-notification',
  templateUrl: './modal-notification.component.html',
  styleUrls: ['./modal-notification.component.css']
})
export class ModalNotificationComponent implements OnInit {
  
  userId = common.GetCurrentUserId();
  data: NotificationModel[] = []

  itemsPerPage = 5;
  currentPage = 1;
  constructor(private activeModal: NgbActiveModal, private notificationService: NotificationService, private router: Router){

  }

  ngOnInit(): void {
      this.getData();
  }

  getData(){
    this.notificationService.GetNotification(this.userId).subscribe(res => {
      if(res.isSuccess){
        this.data = res.result;
      }
    })
  }

  closeModal(): void {
    this.activeModal.dismiss();
  }

  watchNoti(item: NotificationModel){
    if(item.watched){
      this.router.navigate([item.notificationLink]);
        this.activeModal.dismiss();
    }else{
      this.notificationService.UpdateStatus(item.id).subscribe(res => {
        if(res.isSuccess){
          this.router.navigate([item.notificationLink]);
          this.activeModal.dismiss();
        }
      })
    }
  }
  
}
