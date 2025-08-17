import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-handler-select',
  templateUrl: './handler-select.component.html',
  styleUrls: ['./handler-select.component.css']
})
export class HandlerSelectComponent implements OnChanges {
  handler = 0;//Author - 1: General Specialist
  constructor(private activeModal: NgbActiveModal){}
  ngOnInit(): void {
      
  } 

  ngOnChanges(changes: SimpleChanges): void {

  }

  onCheck(val: number){
    this.handler = val;
  }
  closeModal(){
    this.activeModal.dismiss();
  }
  confirmAction(){    
    this.activeModal.close(this.handler)
  }  
}
