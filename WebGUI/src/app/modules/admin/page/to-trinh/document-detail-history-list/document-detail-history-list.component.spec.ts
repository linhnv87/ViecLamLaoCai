import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentDetailHistoryListComponent } from './document-detail-history-list.component';

describe('DocumentDetailHistoryListComponent', () => {
  let component: DocumentDetailHistoryListComponent;
  let fixture: ComponentFixture<DocumentDetailHistoryListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocumentDetailHistoryListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocumentDetailHistoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
