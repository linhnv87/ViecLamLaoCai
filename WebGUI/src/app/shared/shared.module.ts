import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { DashboardSidebarComponent } from './dashboard-sidebar/dashboard-sidebar.component';
import { SplashScreenComponent } from '../components/splash-screen/splash-screen.component';

@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    DashboardSidebarComponent,
    SplashScreenComponent
  ],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    HeaderComponent,
    FooterComponent,
    DashboardSidebarComponent,
    SplashScreenComponent
  ]
})
export class SharedModule { }
