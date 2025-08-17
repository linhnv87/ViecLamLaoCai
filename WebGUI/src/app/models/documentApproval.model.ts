import { Guid } from "guid-typescript";
import { DocumentFileModel } from "./documentFile.model";

export interface DocumentApprovalModel {
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
    viewed? :boolean;
    submitCount?: number;
    filePath?: string;
    attachment?: File;
}

export interface DocumentApprovalSummaryModel{
    title: string;
    status: string;
    field: string;
    submittedAt: Date;
    submitter: string;
    deadlineAt: Date;
    endAt: Date;
    approvals: string[];
    declines: string[];
    noResponses: string[];
    maxRows?: number;
    totalVote?: number;
    yayVote?: number;
    nayVote?: number;
}
export interface ApproverModel {
    userName: string;
    decision: string;
    createdAt: string;
    comment: string;
    files:DocumentFileModel[];
    userId:string;
  }

export interface ApprovalItemModel {
    submitCount: number;
    approvers: ApproverModel[];
    totalApprovers:number;
    reviewedCount:number;
    show?: boolean;
}

export interface DocumentApprovalSummaryModel_V2{
    id: number;
    title: string;
    status: number;
    fieldId: number;
    field: string;
    authorId: string;
    author: string;
    submitCount: number;
    isPassed: boolean;
    submittedAt: Date;    
    deadlineAt: Date;
    endAt: Date;
    approvals: string[];
    declines: string[];
    noResponses: string[];
    maxRows?: number;
    totalVote?: number;
    yayVote?: number;
    nayVote?: number;
}

export interface DocumentApprovalQueryModel{
    title?: string;
    status?: number;
    fieldId?: number;
    authorId?: string;
    submitFrom?: string;
    submitTo?: string;
    isPassed?: boolean;
}