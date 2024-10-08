export function formatBytes(a: number, b = 2) {
    if (!+a) return "0 Bytes";
    const c = 0 > b ? 0 : b;
    const d = Math.floor(Math.log(a) / Math.log(1000));
    return `${parseFloat((a / Math.pow(1000, d)).toFixed(c))} ${["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"][d]}`;
}

export function formatDate(dateString: string) {
	const date = new Date(dateString);

	const monthNames = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];

	const year = date.getFullYear();
	const month = monthNames[date.getMonth()];
	const day = String(date.getDate()).padStart(2, '0');
	const hours = String(date.getHours()).padStart(2, '0');
	const minutes = String(date.getMinutes()).padStart(2, '0');

	return `${year}-${month}-${day} ${hours}:${minutes}`;
}

const randomString = (length: number): string => {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
};

export function getFormattedVideoName(): string{
	const unixNano = BigInt(Date.now()) * BigInt(1e6);
	return `${unixNano}${randomString(5)}`
}

export function getResolutionDimensions(value: number): { width: number; height: number } | null {
    const resolutions: { [key: number]: { width: number; height: number } } = {
        2160: { width: 3840, height: 2160 },
        1440: { width: 2560, height: 1440 },
        1080: { width: 1920, height: 1080 },
        720: { width: 1280, height: 720 },
        480: { width: 854, height: 480 },
        360: { width: 640, height: 360 },
        240: { width: 426, height: 240 },
        144: { width: 256, height: 144 },
    };

    return resolutions[value] || null;
}

export function secondsToTimeString(seconds: number): string {
    const hours = Math.floor(seconds / 3600).toString().padStart(2, '0');
    const minutes = Math.floor((seconds % 3600) / 60).toString().padStart(2, '0');
    const secs = Math.floor(seconds % 60).toString().padStart(2, '0');
    const milliseconds = Number(((seconds % 1) * 1000).toFixed(3).padStart(3, '0'));

    return milliseconds > 0 ? `${hours}:${minutes}:${secs}.${milliseconds}` : `${hours}:${minutes}:${secs}`;
}

export function timeStringToSeconds(timeString: string): number {
    const timeParts = timeString.split(':');
    const hours = parseInt(timeParts[0], 10);
    const minutes = parseInt(timeParts[1], 10);
    const secondsParts = timeParts[2].split('.');
    const seconds = parseInt(secondsParts[0], 10);
    const milliseconds = secondsParts[1] ? parseFloat(`0.${secondsParts[1]}`) : 0;

    return hours * 3600 + minutes * 60 + seconds + milliseconds;
}