export interface Subforum {
    id: number;
    name: string;
    parentForumId: number | null;
    category: string | null;
    description: string | null;
    isClosed: boolean;
    isUnread: boolean;
    subforumsCount: number;
    topicsCount: number;
}