export interface statisticaldetailmodel{
    id: number;
    title: string;
    field: string;
    submittedDate: Date;
    deadlineDate: Date;
    approvals: string[];
    declines: string[];
    noResponses: string[];
  }