import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlanRemindComponent } from './plan-remind.component';

describe('PlanRemindComponent', () => {
  let component: PlanRemindComponent;
  let fixture: ComponentFixture<PlanRemindComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlanRemindComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlanRemindComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
