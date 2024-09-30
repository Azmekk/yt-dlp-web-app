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
import type { Availability } from './Availability';
import {
    AvailabilityFromJSON,
    AvailabilityFromJSONTyped,
    AvailabilityToJSON,
} from './Availability';
import type { ChapterData } from './ChapterData';
import {
    ChapterDataFromJSON,
    ChapterDataFromJSONTyped,
    ChapterDataToJSON,
} from './ChapterData';
import type { SubtitleData } from './SubtitleData';
import {
    SubtitleDataFromJSON,
    SubtitleDataFromJSONTyped,
    SubtitleDataToJSON,
} from './SubtitleData';
import type { ThumbnailData } from './ThumbnailData';
import {
    ThumbnailDataFromJSON,
    ThumbnailDataFromJSONTyped,
    ThumbnailDataToJSON,
} from './ThumbnailData';
import type { FormatData } from './FormatData';
import {
    FormatDataFromJSON,
    FormatDataFromJSONTyped,
    FormatDataToJSON,
} from './FormatData';
import type { MetadataType } from './MetadataType';
import {
    MetadataTypeFromJSON,
    MetadataTypeFromJSONTyped,
    MetadataTypeToJSON,
} from './MetadataType';
import type { CommentData } from './CommentData';
import {
    CommentDataFromJSON,
    CommentDataFromJSONTyped,
    CommentDataToJSON,
} from './CommentData';
import type { LiveStatus } from './LiveStatus';
import {
    LiveStatusFromJSON,
    LiveStatusFromJSONTyped,
    LiveStatusToJSON,
} from './LiveStatus';

/**
 * 
 * @export
 * @interface VideoData
 */
export interface VideoData {
    /**
     * 
     * @type {MetadataType}
     * @memberof VideoData
     */
    resultType?: MetadataType;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    extractor?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    extractorKey?: string | null;
    /**
     * 
     * @type {Array<VideoData>}
     * @memberof VideoData
     */
    entries?: Array<VideoData> | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    id?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    title?: string | null;
    /**
     * 
     * @type {Array<FormatData>}
     * @memberof VideoData
     */
    formats?: Array<FormatData> | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    url?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    extension?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    format?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    formatID?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    playerUrl?: string | null;
    /**
     * 
     * @type {boolean}
     * @memberof VideoData
     */
    direct?: boolean;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    altTitle?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    displayID?: string | null;
    /**
     * 
     * @type {Array<ThumbnailData>}
     * @memberof VideoData
     */
    thumbnails?: Array<ThumbnailData> | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    thumbnail?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    description?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    uploader?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    license?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    creator?: string | null;
    /**
     * 
     * @type {Date}
     * @memberof VideoData
     */
    releaseTimestamp?: Date | null;
    /**
     * 
     * @type {Date}
     * @memberof VideoData
     */
    releaseDate?: Date | null;
    /**
     * 
     * @type {Date}
     * @memberof VideoData
     */
    timestamp?: Date | null;
    /**
     * 
     * @type {Date}
     * @memberof VideoData
     */
    uploadDate?: Date | null;
    /**
     * 
     * @type {Date}
     * @memberof VideoData
     */
    modifiedTimestamp?: Date | null;
    /**
     * 
     * @type {Date}
     * @memberof VideoData
     */
    modifiedDate?: Date | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    uploaderID?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    uploaderUrl?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    channel?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    channelID?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    channelUrl?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    channelFollowerCount?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    location?: string | null;
    /**
     * 
     * @type {{ [key: string]: Array<SubtitleData> | null; }}
     * @memberof VideoData
     */
    subtitles?: { [key: string]: Array<SubtitleData> | null; } | null;
    /**
     * 
     * @type {{ [key: string]: Array<SubtitleData> | null; }}
     * @memberof VideoData
     */
    automaticCaptions?: { [key: string]: Array<SubtitleData> | null; } | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    duration?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    viewCount?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    concurrentViewCount?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    likeCount?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    dislikeCount?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    repostCount?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    averageRating?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    commentCount?: number | null;
    /**
     * 
     * @type {Array<CommentData>}
     * @memberof VideoData
     */
    comments?: Array<CommentData> | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    ageLimit?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    webpageUrl?: string | null;
    /**
     * 
     * @type {Array<string>}
     * @memberof VideoData
     */
    categories?: Array<string> | null;
    /**
     * 
     * @type {Array<string>}
     * @memberof VideoData
     */
    tags?: Array<string> | null;
    /**
     * 
     * @type {Array<string>}
     * @memberof VideoData
     */
    cast?: Array<string> | null;
    /**
     * 
     * @type {boolean}
     * @memberof VideoData
     */
    isLive?: boolean | null;
    /**
     * 
     * @type {boolean}
     * @memberof VideoData
     */
    wasLive?: boolean | null;
    /**
     * 
     * @type {LiveStatus}
     * @memberof VideoData
     */
    liveStatus?: LiveStatus;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    startTime?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    endTime?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    playableInEmbed?: string | null;
    /**
     * 
     * @type {Availability}
     * @memberof VideoData
     */
    availability?: Availability;
    /**
     * 
     * @type {Array<ChapterData>}
     * @memberof VideoData
     */
    chapters?: Array<ChapterData> | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    chapter?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    chapterNumber?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    chapterId?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    series?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    seriesId?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    season?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    seasonNumber?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    seasonId?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    episode?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    episodeNumber?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    episodeId?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    track?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    trackNumber?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    trackId?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    artist?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    genre?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    album?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    albumType?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    albumArtist?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    discNumber?: number | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    releaseYear?: string | null;
    /**
     * 
     * @type {string}
     * @memberof VideoData
     */
    composer?: string | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    sectionStart?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    sectionEnd?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    storyboardFragmentRows?: number | null;
    /**
     * 
     * @type {number}
     * @memberof VideoData
     */
    storyboardFragmentColumns?: number | null;
}



/**
 * Check if a given object implements the VideoData interface.
 */
export function instanceOfVideoData(value: object): value is VideoData {
    return true;
}

export function VideoDataFromJSON(json: any): VideoData {
    return VideoDataFromJSONTyped(json, false);
}

export function VideoDataFromJSONTyped(json: any, ignoreDiscriminator: boolean): VideoData {
    if (json == null) {
        return json;
    }
    return {
        
        'resultType': json['resultType'] == null ? undefined : MetadataTypeFromJSON(json['resultType']),
        'extractor': json['extractor'] == null ? undefined : json['extractor'],
        'extractorKey': json['extractorKey'] == null ? undefined : json['extractorKey'],
        'entries': json['entries'] == null ? undefined : ((json['entries'] as Array<any>).map(VideoDataFromJSON)),
        'id': json['id'] == null ? undefined : json['id'],
        'title': json['title'] == null ? undefined : json['title'],
        'formats': json['formats'] == null ? undefined : ((json['formats'] as Array<any>).map(FormatDataFromJSON)),
        'url': json['url'] == null ? undefined : json['url'],
        'extension': json['extension'] == null ? undefined : json['extension'],
        'format': json['format'] == null ? undefined : json['format'],
        'formatID': json['formatID'] == null ? undefined : json['formatID'],
        'playerUrl': json['playerUrl'] == null ? undefined : json['playerUrl'],
        'direct': json['direct'] == null ? undefined : json['direct'],
        'altTitle': json['altTitle'] == null ? undefined : json['altTitle'],
        'displayID': json['displayID'] == null ? undefined : json['displayID'],
        'thumbnails': json['thumbnails'] == null ? undefined : ((json['thumbnails'] as Array<any>).map(ThumbnailDataFromJSON)),
        'thumbnail': json['thumbnail'] == null ? undefined : json['thumbnail'],
        'description': json['description'] == null ? undefined : json['description'],
        'uploader': json['uploader'] == null ? undefined : json['uploader'],
        'license': json['license'] == null ? undefined : json['license'],
        'creator': json['creator'] == null ? undefined : json['creator'],
        'releaseTimestamp': json['releaseTimestamp'] == null ? undefined : (new Date(json['releaseTimestamp'])),
        'releaseDate': json['releaseDate'] == null ? undefined : (new Date(json['releaseDate'])),
        'timestamp': json['timestamp'] == null ? undefined : (new Date(json['timestamp'])),
        'uploadDate': json['uploadDate'] == null ? undefined : (new Date(json['uploadDate'])),
        'modifiedTimestamp': json['modifiedTimestamp'] == null ? undefined : (new Date(json['modifiedTimestamp'])),
        'modifiedDate': json['modifiedDate'] == null ? undefined : (new Date(json['modifiedDate'])),
        'uploaderID': json['uploaderID'] == null ? undefined : json['uploaderID'],
        'uploaderUrl': json['uploaderUrl'] == null ? undefined : json['uploaderUrl'],
        'channel': json['channel'] == null ? undefined : json['channel'],
        'channelID': json['channelID'] == null ? undefined : json['channelID'],
        'channelUrl': json['channelUrl'] == null ? undefined : json['channelUrl'],
        'channelFollowerCount': json['channelFollowerCount'] == null ? undefined : json['channelFollowerCount'],
        'location': json['location'] == null ? undefined : json['location'],
        'subtitles': json['subtitles'] == null ? undefined : json['subtitles'],
        'automaticCaptions': json['automaticCaptions'] == null ? undefined : json['automaticCaptions'],
        'duration': json['duration'] == null ? undefined : json['duration'],
        'viewCount': json['viewCount'] == null ? undefined : json['viewCount'],
        'concurrentViewCount': json['concurrentViewCount'] == null ? undefined : json['concurrentViewCount'],
        'likeCount': json['likeCount'] == null ? undefined : json['likeCount'],
        'dislikeCount': json['dislikeCount'] == null ? undefined : json['dislikeCount'],
        'repostCount': json['repostCount'] == null ? undefined : json['repostCount'],
        'averageRating': json['averageRating'] == null ? undefined : json['averageRating'],
        'commentCount': json['commentCount'] == null ? undefined : json['commentCount'],
        'comments': json['comments'] == null ? undefined : ((json['comments'] as Array<any>).map(CommentDataFromJSON)),
        'ageLimit': json['ageLimit'] == null ? undefined : json['ageLimit'],
        'webpageUrl': json['webpageUrl'] == null ? undefined : json['webpageUrl'],
        'categories': json['categories'] == null ? undefined : json['categories'],
        'tags': json['tags'] == null ? undefined : json['tags'],
        'cast': json['cast'] == null ? undefined : json['cast'],
        'isLive': json['isLive'] == null ? undefined : json['isLive'],
        'wasLive': json['wasLive'] == null ? undefined : json['wasLive'],
        'liveStatus': json['liveStatus'] == null ? undefined : LiveStatusFromJSON(json['liveStatus']),
        'startTime': json['startTime'] == null ? undefined : json['startTime'],
        'endTime': json['endTime'] == null ? undefined : json['endTime'],
        'playableInEmbed': json['playableInEmbed'] == null ? undefined : json['playableInEmbed'],
        'availability': json['availability'] == null ? undefined : AvailabilityFromJSON(json['availability']),
        'chapters': json['chapters'] == null ? undefined : ((json['chapters'] as Array<any>).map(ChapterDataFromJSON)),
        'chapter': json['chapter'] == null ? undefined : json['chapter'],
        'chapterNumber': json['chapterNumber'] == null ? undefined : json['chapterNumber'],
        'chapterId': json['chapterId'] == null ? undefined : json['chapterId'],
        'series': json['series'] == null ? undefined : json['series'],
        'seriesId': json['seriesId'] == null ? undefined : json['seriesId'],
        'season': json['season'] == null ? undefined : json['season'],
        'seasonNumber': json['seasonNumber'] == null ? undefined : json['seasonNumber'],
        'seasonId': json['seasonId'] == null ? undefined : json['seasonId'],
        'episode': json['episode'] == null ? undefined : json['episode'],
        'episodeNumber': json['episodeNumber'] == null ? undefined : json['episodeNumber'],
        'episodeId': json['episodeId'] == null ? undefined : json['episodeId'],
        'track': json['track'] == null ? undefined : json['track'],
        'trackNumber': json['trackNumber'] == null ? undefined : json['trackNumber'],
        'trackId': json['trackId'] == null ? undefined : json['trackId'],
        'artist': json['artist'] == null ? undefined : json['artist'],
        'genre': json['genre'] == null ? undefined : json['genre'],
        'album': json['album'] == null ? undefined : json['album'],
        'albumType': json['albumType'] == null ? undefined : json['albumType'],
        'albumArtist': json['albumArtist'] == null ? undefined : json['albumArtist'],
        'discNumber': json['discNumber'] == null ? undefined : json['discNumber'],
        'releaseYear': json['releaseYear'] == null ? undefined : json['releaseYear'],
        'composer': json['composer'] == null ? undefined : json['composer'],
        'sectionStart': json['sectionStart'] == null ? undefined : json['sectionStart'],
        'sectionEnd': json['sectionEnd'] == null ? undefined : json['sectionEnd'],
        'storyboardFragmentRows': json['storyboardFragmentRows'] == null ? undefined : json['storyboardFragmentRows'],
        'storyboardFragmentColumns': json['storyboardFragmentColumns'] == null ? undefined : json['storyboardFragmentColumns'],
    };
}

export function VideoDataToJSON(value?: VideoData | null): any {
    if (value == null) {
        return value;
    }
    return {
        
        'resultType': MetadataTypeToJSON(value['resultType']),
        'extractor': value['extractor'],
        'extractorKey': value['extractorKey'],
        'entries': value['entries'] == null ? undefined : ((value['entries'] as Array<any>).map(VideoDataToJSON)),
        'id': value['id'],
        'title': value['title'],
        'formats': value['formats'] == null ? undefined : ((value['formats'] as Array<any>).map(FormatDataToJSON)),
        'url': value['url'],
        'extension': value['extension'],
        'format': value['format'],
        'formatID': value['formatID'],
        'playerUrl': value['playerUrl'],
        'direct': value['direct'],
        'altTitle': value['altTitle'],
        'displayID': value['displayID'],
        'thumbnails': value['thumbnails'] == null ? undefined : ((value['thumbnails'] as Array<any>).map(ThumbnailDataToJSON)),
        'thumbnail': value['thumbnail'],
        'description': value['description'],
        'uploader': value['uploader'],
        'license': value['license'],
        'creator': value['creator'],
        'releaseTimestamp': value['releaseTimestamp'] == null ? undefined : ((value['releaseTimestamp'] as any).toISOString()),
        'releaseDate': value['releaseDate'] == null ? undefined : ((value['releaseDate'] as any).toISOString()),
        'timestamp': value['timestamp'] == null ? undefined : ((value['timestamp'] as any).toISOString()),
        'uploadDate': value['uploadDate'] == null ? undefined : ((value['uploadDate'] as any).toISOString()),
        'modifiedTimestamp': value['modifiedTimestamp'] == null ? undefined : ((value['modifiedTimestamp'] as any).toISOString()),
        'modifiedDate': value['modifiedDate'] == null ? undefined : ((value['modifiedDate'] as any).toISOString()),
        'uploaderID': value['uploaderID'],
        'uploaderUrl': value['uploaderUrl'],
        'channel': value['channel'],
        'channelID': value['channelID'],
        'channelUrl': value['channelUrl'],
        'channelFollowerCount': value['channelFollowerCount'],
        'location': value['location'],
        'subtitles': value['subtitles'],
        'automaticCaptions': value['automaticCaptions'],
        'duration': value['duration'],
        'viewCount': value['viewCount'],
        'concurrentViewCount': value['concurrentViewCount'],
        'likeCount': value['likeCount'],
        'dislikeCount': value['dislikeCount'],
        'repostCount': value['repostCount'],
        'averageRating': value['averageRating'],
        'commentCount': value['commentCount'],
        'comments': value['comments'] == null ? undefined : ((value['comments'] as Array<any>).map(CommentDataToJSON)),
        'ageLimit': value['ageLimit'],
        'webpageUrl': value['webpageUrl'],
        'categories': value['categories'],
        'tags': value['tags'],
        'cast': value['cast'],
        'isLive': value['isLive'],
        'wasLive': value['wasLive'],
        'liveStatus': LiveStatusToJSON(value['liveStatus']),
        'startTime': value['startTime'],
        'endTime': value['endTime'],
        'playableInEmbed': value['playableInEmbed'],
        'availability': AvailabilityToJSON(value['availability']),
        'chapters': value['chapters'] == null ? undefined : ((value['chapters'] as Array<any>).map(ChapterDataToJSON)),
        'chapter': value['chapter'],
        'chapterNumber': value['chapterNumber'],
        'chapterId': value['chapterId'],
        'series': value['series'],
        'seriesId': value['seriesId'],
        'season': value['season'],
        'seasonNumber': value['seasonNumber'],
        'seasonId': value['seasonId'],
        'episode': value['episode'],
        'episodeNumber': value['episodeNumber'],
        'episodeId': value['episodeId'],
        'track': value['track'],
        'trackNumber': value['trackNumber'],
        'trackId': value['trackId'],
        'artist': value['artist'],
        'genre': value['genre'],
        'album': value['album'],
        'albumType': value['albumType'],
        'albumArtist': value['albumArtist'],
        'discNumber': value['discNumber'],
        'releaseYear': value['releaseYear'],
        'composer': value['composer'],
        'sectionStart': value['sectionStart'],
        'sectionEnd': value['sectionEnd'],
        'storyboardFragmentRows': value['storyboardFragmentRows'],
        'storyboardFragmentColumns': value['storyboardFragmentColumns'],
    };
}

