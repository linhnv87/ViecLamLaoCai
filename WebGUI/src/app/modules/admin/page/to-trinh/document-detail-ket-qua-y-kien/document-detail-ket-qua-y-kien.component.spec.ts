import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentDetailKetQuaYKienComponent } from './document-detail-ket-qua-y-kien.component';

describe('DocumentDetailKetQuaYKienComponent', () => {
  let component: DocumentDetailKetQuaYKienComponent;
  let fixture: ComponentFixture<DocumentDetailKetQuaYKienComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentDetailKetQuaYKienComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentDetailKetQuaYKienComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
