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
 * @interface ThumbnailData
 */
export interface ThumbnailData {
    /**
     * 
     * @type {string}
     * @memberof ThumbnailData
     */
    id?: string | null;
    /**
     * 
     * @type {string}
     * @memberof ThumbnailData
     */
    url?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ThumbnailData
     */
    preference?: number | null;
    /**
     * 
     * @type {number}
     * @memberof ThumbnailData
     */
    width?: number | null;
    /**
     * 
     * @type {number}
     * @memberof ThumbnailData
     */
    height?: number | null;
    /**
     * 
     * @type {string}
     * @memberof ThumbnailData
     */
    resolution?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ThumbnailData
     */
    filesize?: number | null;
}

/**
 * Check if a given object implements the ThumbnailData interface.
 */
export function instanceOfThumbnailData(value: object): value is ThumbnailData {
    return true;
}

export function ThumbnailDataFromJSON(json: any): ThumbnailData {
    return ThumbnailDataFromJSONTyped(json, false);
}

export function ThumbnailDataFromJSONTyped(json: any, ignoreDiscriminator: boolean): ThumbnailData {
    if (json == null) {
        return json;
    }
    return {
        
        'id': json['id'] == null ? undefined : json['id'],
        'url': json['url'] == null ? undefined : json['url'],
        'preference': json['preference'] == null ? undefined : json['preference'],
        'width': json['width'] == null ? undefined : json['width'],
        'height': json['height'] == null ? undefined : json['height'],
        'resolution': json['resolution'] == null ? undefined : json['resolution'],
        'filesize': json['filesize'] == null ? undefined : json['filesize'],
    };
}

export function ThumbnailDataToJSON(value?: ThumbnailData | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'id': value['id'],
        'url': value['url'],
        'preference': value['preference'],
        'width': value['width'],
        'height': value['height'],
        'resolution': value['resolution'],
        'filesize': value['filesize'],
    };
}

