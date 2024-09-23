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

import { mapValues } from '../runtime';
/**
 * 
 * @export
 * @interface UsedStorageResponse
 */
export interface UsedStorageResponse {
    /**
     * 
     * @type {number}
     * @memberof UsedStorageResponse
     */
    usedStorage?: number;
}

/**
 * Check if a given object implements the UsedStorageResponse interface.
 */
export function instanceOfUsedStorageResponse(value: object): value is UsedStorageResponse {
    return true;
}

export function UsedStorageResponseFromJSON(json: any): UsedStorageResponse {
    return UsedStorageResponseFromJSONTyped(json, false);
}

export function UsedStorageResponseFromJSONTyped(json: any, ignoreDiscriminator: boolean): UsedStorageResponse {
    if (json == null) {
        return json;
    }
    return {
        
        'usedStorage': json['usedStorage'] == null ? undefined : json['usedStorage'],
    };
}

export function UsedStorageResponseToJSON(value?: UsedStorageResponse | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'usedStorage': value['usedStorage'],
    };
}

