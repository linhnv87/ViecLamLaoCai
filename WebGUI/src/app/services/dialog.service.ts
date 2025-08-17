import { Injectable } from '@angular/core';
import { MatDialog} from '@angular/material/dialog';
import { ConfirmationComponent } from '../components/confirmation/confirmation.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  constructor(private dialog: MatDialog) { }
  openConfirmationDialog(confirmMessage: string): Observable<boolean>{
    const dialogRef = this.dialog.open(ConfirmationComponent);
    dialogRef.componentInstance.message = confirmMessage;
    return dialogRef.afterClosed();
  }  
}
