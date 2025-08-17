import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from '../admin/admin.component';
import { AuthComponent } from './auth.component';
import { SsoComponent } from './sso/sso.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: AuthComponent,    
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
