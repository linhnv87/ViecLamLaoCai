export interface DocumentReviewModel {
  id?: number;
  title?: string;
  docId?: number;
  statusCode?: number;
  userId?: string;
  userName?: string;
  modified?: Date;
  deleted?: boolean;
  modifiedBy?: string;
  createdBy?: string;
  created?: Date;
  comment?: string;
  viewed?: boolean;
  submitCount?: number;
  filePath?: string;
  attachment?: File;
  reviewResult?: number;
  files?: File[];
}
