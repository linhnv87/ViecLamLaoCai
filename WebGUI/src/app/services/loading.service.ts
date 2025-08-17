import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  constructor() {}
  private loading$ = new Subject<boolean>();

  getLoading() {
    return this.loading$.asObservable();
  }

  setLoading(status: boolean) {
    this.loading$.next(status);
  }
}
