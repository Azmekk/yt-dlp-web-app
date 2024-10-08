/* tslint:disable */
/* eslint-disable */
/**
 * YT-DLP-Web-App-Backend
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


/**
 * 
 * @export
 */
export const Mp3Status = {
    NONE: 0,
    INPROGRESS: 1,
    COMPLETED: 2
} as const;
export type Mp3Status = typeof Mp3Status[keyof typeof Mp3Status];


export function instanceOfMp3Status(value: any): boolean {
    for (const key in Mp3Status) {
        if (Object.prototype.hasOwnProperty.call(Mp3Status, key)) {
            if (Mp3Status[key as keyof typeof Mp3Status] === value) {
                return true;
            }
        }
    }
    return false;
}

export function Mp3StatusFromJSON(json: any): Mp3Status {
    return Mp3StatusFromJSONTyped(json, false);
}

export function Mp3StatusFromJSONTyped(json: any, ignoreDiscriminator: boolean): Mp3Status {
    return json as Mp3Status;
}

export function Mp3StatusToJSON(value?: Mp3Status | null): any {
    return value as any;
}

