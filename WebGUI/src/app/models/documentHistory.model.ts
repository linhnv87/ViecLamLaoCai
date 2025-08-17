export interface DocumentHistoryModel {
  id: number;
  documentId: number;
  documentTitle?: string;
  note?: string;
  historyStatus: number;
  created?: Date;
  nameUser?: string;
  comment?: string;
}

export interface DocumentRetrievalModel {
  documentId: number;
  note: string;
  currentUserId?: string;
  comment?: string;
}
