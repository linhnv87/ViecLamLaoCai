export interface NotificationModel{
    id: number;
    notificationContent: string;
    notificationLink: string;
    type: number;
    forUserId: string;
    watched: boolean;
}