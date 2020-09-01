import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name: 'enumList'
})
export class EnumListPipe implements PipeTransform {
    transform(enumValues) : any {
        let enumList = [];
        for (const key in enumValues) {
            if(enumValues.hasOwnProperty(key)) {
                if (!isNaN(parseInt(key, 10))) {
                    enumList.push({key: key, value: enumValues[key]});
                }
            }
        }
        return enumList;
    }
}
