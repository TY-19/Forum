export interface ForumsStructure {
    id: number,
    parentForumId: number | null,
    name: string,
    subElements: ForumsStructure[]
}