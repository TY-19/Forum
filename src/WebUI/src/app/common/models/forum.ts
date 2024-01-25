import { Subforum } from "./subforum";
import { TopicInForum } from "./topic-in-forum";

export interface Forum {
    id: number,
    name: string,
    parentForumId: number | null,
    category: string | null,
    description: string | null,
    isClosed: boolean,
    isUnread: boolean,
    subcategories: string[],
    subforums: Subforum[],
    topics: TopicInForum[]
}