export interface User {
    id: string,
    userName: string | null,
    email: string | null,
    userProfileId: number | null,
    roles: string[]
}