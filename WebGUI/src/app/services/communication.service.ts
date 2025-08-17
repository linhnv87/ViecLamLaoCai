import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommunicationService {

  constructor() { }
  private reloadFunctionSubject = new Subject<void>();

  // Observable that child components can subscribe to
  reloadFunction$ = this.reloadFunctionSubject.asObservable();

  // Method called by child components to trigger a reload
  triggerReloadFunction() {
    this.reloadFunctionSubject.next();
  }
}
