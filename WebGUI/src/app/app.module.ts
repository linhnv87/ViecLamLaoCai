import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ModalNotificationComponent } from './components/modal-notification/modal-notification.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';
import { ConfirmationComponent } from './components/confirmation/confirmation.component';
import { CommentComponent } from './components/comment/comment.component';
import { NgChartsModule } from 'ng2-charts';
import { UploadDocumentComponent } from './components/upload-document/upload-document.component';
import { PriorityDocumentComponent } from './components/priority-document/priority-document.component';
import { HandlerSelectComponent } from './components/handler-select/handler-select.component';
import {
  NgxMatDatetimePickerModule,
  NgxMatNativeDateModule,
  NgxMatTimepickerModule,
} from '@angular-material-components/datetime-picker';
import { NgxMomentDateModule } from '@angular-material-components/moment-adapter';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { ResultFileApprovalComponent } from './components/result-file-approval/result-file-approval.component';
import { AddFileComponent } from './components/add-file/add-file.component';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@NgModule({
  declarations: [
    AppComponent,
    ModalNotificationComponent,
    ConfirmationComponent,
    CommentComponent,
    UploadDocumentComponent,
    PriorityDocumentComponent,
    HandlerSelectComponent,
    ResultFileApprovalComponent,
    AddFileComponent,
    // PieChartComponent,
    // LineChartComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatTooltipModule,
    HttpClientModule,
    ToastrModule.forRoot({
      positionClass: 'toast-top', // Set the position class
    }),
    NgbModule,
    NgxPaginationModule,
    NgChartsModule,
    NgxMatDatetimePickerModule,
    NgxMatTimepickerModule,
    NgxMomentDateModule,
    NgxMatNativeDateModule,
    NgxExtendedPdfViewerModule
  ],
  providers: [{ provide: MAT_DATE_LOCALE, useValue: 'vi' }],
  bootstrap: [AppComponent],
})
export class AppModule {}
