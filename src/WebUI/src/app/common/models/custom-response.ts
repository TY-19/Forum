export interface CustomResponse {
    succeeded: boolean,
    message: string | null,
}

export interface CustomResponseGeneric<T> extends CustomResponse {
    payload: T
}