export interface BaseResponseModel<T>{
    statusCode: number,
    isSuccess: boolean,
    result: T,
    message: string 
}