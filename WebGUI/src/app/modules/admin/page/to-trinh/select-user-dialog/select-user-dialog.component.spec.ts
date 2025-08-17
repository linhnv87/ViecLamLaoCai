import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectUserDialogComponent } from './select-user-dialog.component';

describe('SelectUserDialogComponent', () => {
  let component: SelectUserDialogComponent;
  let fixture: ComponentFixture<SelectUserDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectUserDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SelectUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
