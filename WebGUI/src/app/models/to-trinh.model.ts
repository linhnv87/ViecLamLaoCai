export interface ToTrinhPayloadModel {
  documentId?: number;
  users?: string[];
  fromStatusCode?: string;
  toStatusCode?: string;
  userId?: string;
  comment?: string;
}
