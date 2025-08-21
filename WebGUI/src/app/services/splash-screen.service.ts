import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface SplashConfig {
  type: 'loading' | 'login' | 'sso' | 'success' | 'error';
  title: string;
  message?: string;
  autoHide?: number;
  showProgress?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SplashScreenService {
  private isVisibleSubject = new BehaviorSubject<boolean>(false);
  private configSubject = new BehaviorSubject<SplashConfig>({
    type: 'loading',
    title: 'Việc Làm Lào Cai',
    message: 'Vui lòng chờ trong giây lát'
  });

  public isVisible$ = this.isVisibleSubject.asObservable();
  public config$ = this.configSubject.asObservable();

  constructor() { }

  /**
   * Show splash screen with config
   */
  show(config?: SplashConfig): void {
    // Use provided config or default config
    const defaultConfig: SplashConfig = {
      type: 'loading',
      title: 'Việc Làm Lào Cai',
      message: 'Vui lòng chờ trong giây lát'
    };
    
    const finalConfig = config || defaultConfig;
    this.configSubject.next(finalConfig);
    this.isVisibleSubject.next(true);
  }

  /**
   * Hide splash screen
   */
  hide(): void {
    this.isVisibleSubject.next(false);
  }





  /**
   * Auto hide splash screen after specified duration
   * @param duration Duration in milliseconds (default: 3000ms)
   */
  autoHide(duration: number = 3000): void {
    setTimeout(() => {
      this.hide();
    }, duration);
  }

  /**
   * Hide splash screen with fade effect
   * @param fadeDelay Delay before starting fade (default: 2500ms)
   * @param fadeDuration Duration of fade effect (default: 500ms)
   */
  hideWithFade(fadeDelay: number = 2500, fadeDuration: number = 500): void {
    setTimeout(() => {
      this.hide();
    }, fadeDelay);
  }

  /**
   * Show quick action feedback (optimized for user actions)
   * @param type Type of feedback
   * @param title Title message
   * @param message Optional message
   * @param duration Auto-hide duration (default: 1200ms)
   */
  showQuickFeedback(type: 'success' | 'error', title: string, message?: string, duration: number = 1200): void {
    this.show({
      type: type,
      title: title,
      message: message
    });
    
    setTimeout(() => {
      this.hide();
    }, duration);
  }

  /**
   * Show brief loading for quick operations
   * @param title Loading title
   * @param message Optional message
   * @param duration Loading duration (default: 800ms)
   * @param successTitle Success title after loading
   * @param successMessage Success message after loading
   */
  showBriefLoading(title: string, message?: string, duration: number = 800, successTitle?: string, successMessage?: string): void {
    this.show({
      type: 'loading',
      title: title,
      message: message
    });
    
    setTimeout(() => {
      if (successTitle) {
        this.show({
          type: 'success',
          title: successTitle,
          message: successMessage
        });
        setTimeout(() => {
          this.hide();
        }, 1000);
      } else {
        this.hide();
      }
    }, duration);
  }
}
