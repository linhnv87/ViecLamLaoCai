import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LaborMarketReportComponent } from './labor-market-report.component';

describe('LaborMarketReportComponent', () => {
  let component: LaborMarketReportComponent;
  let fixture: ComponentFixture<LaborMarketReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LaborMarketReportComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LaborMarketReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});



