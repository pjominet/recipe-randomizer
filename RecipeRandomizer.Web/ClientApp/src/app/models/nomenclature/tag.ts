import {TagCategory} from '@app/models/nomenclature/tagCategory';

export interface Tag {
    id: number;
    label: string;

    tagCategory: TagCategory;
}
