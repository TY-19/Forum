export interface Forum {
    id: number,
    name: string,
    parentForumId: number | null,
    category: string | null,
    description: string | null,
    isClosed: boolean,
    isUnread: boolean,
    subcategories: string[],
    subforums: [],
    topics: []
}