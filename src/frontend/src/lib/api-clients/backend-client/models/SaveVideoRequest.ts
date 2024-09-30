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
import type { VideoDuration } from './VideoDuration';
import {
    VideoDurationFromJSON,
    VideoDurationFromJSONTyped,
    VideoDurationToJSON,
} from './VideoDuration';
import type { VideoDimensions } from './VideoDimensions';
import {
    VideoDimensionsFromJSON,
    VideoDimensionsFromJSONTyped,
    VideoDimensionsToJSON,
} from './VideoDimensions';

/**
 * 
 * @export
 * @interface SaveVideoRequest
 */
export interface SaveVideoRequest {
    /**
     * 
     * @type {string}
     * @memberof SaveVideoRequest
     */
    videoUrl: string;
    /**
     * 
     * @type {string}
     * @memberof SaveVideoRequest
     */
    videoName: string;
    /**
     * 
     * @type {VideoDimensions}
     * @memberof SaveVideoRequest
     */
    videoDimensions?: VideoDimensions;
    /**
     * 
     * @type {VideoDuration}
     * @memberof SaveVideoRequest
     */
    videoDuration?: VideoDuration;
}

/**
 * Check if a given object implements the SaveVideoRequest interface.
 */
export function instanceOfSaveVideoRequest(value: object): value is SaveVideoRequest {
    if (!('videoUrl' in value) || value['videoUrl'] === undefined) return false;
    if (!('videoName' in value) || value['videoName'] === undefined) return false;
    return true;
}

export function SaveVideoRequestFromJSON(json: any): SaveVideoRequest {
    return SaveVideoRequestFromJSONTyped(json, false);
}

export function SaveVideoRequestFromJSONTyped(json: any, ignoreDiscriminator: boolean): SaveVideoRequest {
    if (json == null) {
        return json;
    }
    return {
        
        'videoUrl': json['videoUrl'],
        'videoName': json['videoName'],
        'videoDimensions': json['videoDimensions'] == null ? undefined : VideoDimensionsFromJSON(json['videoDimensions']),
        'videoDuration': json['videoDuration'] == null ? undefined : VideoDurationFromJSON(json['videoDuration']),
    };
}

export function SaveVideoRequestToJSON(value?: SaveVideoRequest | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'videoUrl': value['videoUrl'],
        'videoName': value['videoName'],
        'videoDimensions': VideoDimensionsToJSON(value['videoDimensions']),
        'videoDuration': VideoDurationToJSON(value['videoDuration']),
    };
}

