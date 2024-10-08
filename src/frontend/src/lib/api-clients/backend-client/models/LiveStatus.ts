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
export const LiveStatus = {
    NUMBER_0: 0,
    NUMBER_1: 1,
    NUMBER_2: 2,
    NUMBER_3: 3,
    NUMBER_4: 4,
    NUMBER_5: 5
} as const;
export type LiveStatus = typeof LiveStatus[keyof typeof LiveStatus];


export function instanceOfLiveStatus(value: any): boolean {
    for (const key in LiveStatus) {
        if (Object.prototype.hasOwnProperty.call(LiveStatus, key)) {
            if (LiveStatus[key as keyof typeof LiveStatus] === value) {
                return true;
            }
        }
    }
    return false;
}

export function LiveStatusFromJSON(json: any): LiveStatus {
    return LiveStatusFromJSONTyped(json, false);
}

export function LiveStatusFromJSONTyped(json: any, ignoreDiscriminator: boolean): LiveStatus {
    return json as LiveStatus;
}

export function LiveStatusToJSON(value?: LiveStatus | null): any {
    return value as any;
}

