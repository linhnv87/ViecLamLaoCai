export interface BaseResponseModel<T> {
  result: T;
  isSuccess: boolean;
  statusCode: number;
  message: string;
}
