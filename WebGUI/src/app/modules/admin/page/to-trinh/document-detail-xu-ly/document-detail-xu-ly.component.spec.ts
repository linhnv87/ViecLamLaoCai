import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentDetailXuLyComponent } from './document-detail-xu-ly.component';

describe('DocumentDetailXuLyComponent', () => {
  let component: DocumentDetailXuLyComponent;
  let fixture: ComponentFixture<DocumentDetailXuLyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentDetailXuLyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentDetailXuLyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
