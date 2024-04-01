export interface Category {
    id: number
    name: string;
    parentForumId: number | null;
    position: number;
}