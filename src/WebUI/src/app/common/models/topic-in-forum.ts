export interface TopicInForum {
    id: number;
    title: string;
    parentForumId: number | null;
    isClosed: boolean;
    isUnread: boolean;
    messagesCount: number;
}