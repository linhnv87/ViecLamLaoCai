export interface SMSLogModel {
    id: number;
    phoneNumber: string;
    receiverName: string;
    messageType: string;
    status: string;
    sentTime: Date;
    errorMessage:string;
  }
export interface SMSLogQueryModel {
    docId: number;
    type: number;
    isSucceeded?: boolean | null; 
}
export interface SMSLogGroupModel{
  submitCount: number; 
  smsLogs:SMSLogModel[];
  show?: boolean;
}