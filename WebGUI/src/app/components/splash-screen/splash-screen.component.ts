import { Component, OnInit, OnDestroy } from '@angular/core';
import { SplashScreenService, SplashConfig } from 'src/app/services/splash-screen.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-splash-screen',
  templateUrl: './splash-screen.component.html',
  styleUrls: ['./splash-screen.component.scss']
})
export class SplashScreenComponent implements OnInit, OnDestroy {
  isVisible = false;
  config: SplashConfig = {
    type: 'loading',
    title: 'Việc Làm Lào Cai',
    message: 'Vui lòng chờ trong giây lát'
  };

  private subscriptions: Subscription[] = [];

  constructor(private splashScreenService: SplashScreenService) { }

  ngOnInit(): void {
    this.subscriptions.push(
      this.splashScreenService.isVisible$.subscribe(
        visible => {
          this.isVisible = visible;
        }
      )
    );

    this.subscriptions.push(
      this.splashScreenService.config$.subscribe(
        config => {
          if (config) {
            this.config = config;
          }
        }
      )
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get splashClass(): string {
    return `splash-${this.config.type}`;
  }

  get iconClass(): string {
    switch (this.config.type) {
      case 'success':
        return 'icon-success';
      case 'error':
        return 'icon-error';
      default:
        return 'icon-spinner';
    }
  }
}
