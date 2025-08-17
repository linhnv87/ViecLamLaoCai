import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.css']
})
export class ConfirmationComponent implements OnInit {
  @Input() show = true;
  @Input() message = 'Xác nhận hành động này';  
  @Input() attachmentFilePath = '';
  
  constructor(private activeModal: NgbActiveModal){}
  confirm(result: boolean){
    this.activeModal.close(result);
  }

  ngOnInit(): void {
      
  }
}
