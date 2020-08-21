import {Tag} from './tag';

export interface TagCategory {
    id: number;
    label: string;

    tags: Tag[];
}
