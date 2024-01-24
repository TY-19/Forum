export interface ForumCreate {
    name: string,
    parentForumId: number | null,
    category: string | null,
    description: string | null
}