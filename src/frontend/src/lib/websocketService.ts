export class WebSocketService {
    private socket: WebSocket | null = null;
    private url: string;
    private onMessageCallback: (message: string) => Promise<void>;

    constructor(url: string, onMessageCallback: (message: string) => Promise<void>) {
        this.url = url;
        this.onMessageCallback = onMessageCallback;
    }

    connect(): void {
        this.socket = new WebSocket(this.url);

        this.socket.onopen = () => {
            console.log(`WebSocket connected to ${this.url}`);
        };

        this.socket.onmessage = async (event: MessageEvent) => {
            await this.onMessageCallback(event.data);
        };

        this.socket.onerror = (error: Event) => {
            console.error('WebSocket error:', error);
        };

        this.socket.onclose = (event: CloseEvent) => {
            console.log(`WebSocket closed: ${event.reason}`);
        };
    }

    sendMessage(message: string): void {
        if (this.socket && this.socket.readyState === WebSocket.OPEN) {
            this.socket.send(message);
        } else {
            console.error('WebSocket is not open');
        }
    }

    public isOpen(): boolean{
        return (this.socket && this.socket.readyState === WebSocket.OPEN) ?? false;
    }

    close(): void {
        if (this.socket) {
            this.socket.close();
        }
    }
}

export interface VideoDownloadInfo {
    videoId: number;
    downloadPercent: number;
    downloaded: boolean;
}

export interface Mp3CompletionInfo {
    videoId: number;
    completed: boolean;
}
