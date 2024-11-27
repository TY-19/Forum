export interface LoginResponse {
    succeeded: boolean,
    message: string,
    token?: string,
    userName?: string,
    userProfileId?: number,
    roles: string[]
}