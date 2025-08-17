import { SimplerRoleTempModel } from './user.model';

export interface ReportReviewModel {
  userId?: string;
  noReview: number;
  disAgreed: number;
  agreed: number;
  other: number;
  userFullName: string;
  phoneNumber: string;
  roles?: SimplerRoleTempModel[];
  roleDescription?: string;
  totalOfDocID: number;
  departmentName?: string;
  unitName?: string;
  release?: number;
  noRelease?: number;
}

export interface ReportReviewResponseModel {
  pagination: ReportReviewPagination;
  data: ReportReviewModel[];
}

export interface ReportReviewPagination {
  pageSize: number;
  pageNumber: number;
  totalCount: number;
  totalPage: number;
}

export interface ReportReviewRequestModel {
  keyword?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber: number | null;
  pageSize: number | null;
  fileName?: string;
  sheetName?: string;
  type?: 'EXCEL' | 'PDF';
}

export interface DocumentReport {
  id: number;
  title: string;
  note: string;
  statusCode: string;
  assigneeID?: string;
}
