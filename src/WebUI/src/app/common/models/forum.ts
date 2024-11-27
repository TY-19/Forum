import { Category } from "./category";
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
    subcategories: Category[],
    subforums: Subforum[],
    topics: TopicInForum[]
}