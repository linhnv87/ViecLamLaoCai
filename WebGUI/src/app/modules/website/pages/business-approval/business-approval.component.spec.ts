import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BusinessApprovalComponent } from './business-approval.component';

describe('BusinessApprovalComponent', () => {
  let component: BusinessApprovalComponent;
  let fixture: ComponentFixture<BusinessApprovalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BusinessApprovalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BusinessApprovalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});



