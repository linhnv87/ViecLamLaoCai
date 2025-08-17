import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentDetailInformationComponent } from './document-detail-information.component';

describe('DocumentDetailInformationComponent', () => {
  let component: DocumentDetailInformationComponent;
  let fixture: ComponentFixture<DocumentDetailInformationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentDetailInformationComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentDetailInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
