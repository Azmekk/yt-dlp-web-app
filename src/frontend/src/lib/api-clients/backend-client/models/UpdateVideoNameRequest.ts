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
 * @interface UpdateVideoNameRequest
 */
export interface UpdateVideoNameRequest {
    /**
     * 
     * @type {number}
     * @memberof UpdateVideoNameRequest
     */
    videoId?: number;
    /**
     * 
     * @type {string}
     * @memberof UpdateVideoNameRequest
     */
    newName?: string | null;
}

/**
 * Check if a given object implements the UpdateVideoNameRequest interface.
 */
export function instanceOfUpdateVideoNameRequest(value: object): value is UpdateVideoNameRequest {
    return true;
}

export function UpdateVideoNameRequestFromJSON(json: any): UpdateVideoNameRequest {
    return UpdateVideoNameRequestFromJSONTyped(json, false);
}

export function UpdateVideoNameRequestFromJSONTyped(json: any, ignoreDiscriminator: boolean): UpdateVideoNameRequest {
    if (json == null) {
        return json;
    }
    return {
        
        'videoId': json['videoId'] == null ? undefined : json['videoId'],
        'newName': json['newName'] == null ? undefined : json['newName'],
    };
}

export function UpdateVideoNameRequestToJSON(value?: UpdateVideoNameRequest | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'videoId': value['videoId'],
        'newName': value['newName'],
    };
}

