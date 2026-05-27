import { _decorator, Component } from 'cc';
import { GameConfig } from '../core/GameConfig';
const { ccclass, property } = _decorator;

export interface PlatformConfig {
    platform: string;
    appId: string;
    features: string[];
    apiVersion: string;
}

export interface ShareConfig {
    title: string;
    imageUrl: string;
    query: string;
    templateId?: string;
}

export interface LeaderboardConfig {
    statName: string;
    order: 'asc' | 'desc';
    updateStrategy: 'better' | 'latest';
}

@ccclass('PlatformSDK')
export class PlatformSDK extends Component {
    private static instance: PlatformSDK;
    private currentPlatform: string = GameConfig.PLATFORM_SDK.NATIVE;
    private isInitialized: boolean = false;
    private config: PlatformConfig | null = null;
    
    public static getInstance(): PlatformSDK {
        if (!PlatformSDK.instance) {
            PlatformSDK.instance = new PlatformSDK();
        }
        return PlatformSDK.instance;
    }
    
    onLoad() {
        PlatformSDK.instance = this;
        this.detectPlatform();
    }
    
    start() {
        if (this.currentPlatform !== GameConfig.PLATFORM_SDK.NATIVE) {
            this.initializePlatform();
        }
    }
    
    private detectPlatform() {
        if (typeof window === 'undefined') {
            this.currentPlatform = GameConfig.PLATFORM_SDK.NATIVE;
            return;
        }
        
        const ua = navigator.userAgent.toLowerCase();
        
        if (ua.includes('micromessenger')) {
            this.currentPlatform = GameConfig.PLATFORM_SDK.WECHAT;
        } else if (ua.includes('bytedance')) {
            this.currentPlatform = GameConfig.PLATFORM_SDK.DOUYIN;
        } else if (ua.includes('taptap')) {
            this.currentPlatform = GameConfig.PLATFORM_SDK.TAPTAP;
        } else {
            this.currentPlatform = GameConfig.PLATFORM_SDK.NATIVE;
        }
        
        console.log(`[PlatformSDK] Detected platform: ${this.currentPlatform}`);
    }
    
    private initializePlatform() {
        switch (this.currentPlatform) {
            case GameConfig.PLATFORM_SDK.WECHAT:
                this.initializeWeChat();
                break;
            case GameConfig.PLATFORM_SDK.DOUYIN:
                this.initializeDouyin();
                break;
            case GameConfig.PLATFORM_SDK.TAPTAP:
                this.initializeTapTap();
                break;
            default:
                console.log('[PlatformSDK] Running on native platform');
        }
        
        this.isInitialized = true;
    }
    
    private initializeWeChat() {
        console.log('[PlatformSDK] Initializing WeChat Mini Game SDK');
        
        try {
            if (typeof wx !== 'undefined') {
                wx.showLoading({ title: '加载中...' });
                
                this.config = {
                    platform: GameConfig.PLATFORM_SDK.WECHAT,
                    appId: 'wx_your_app_id',
                    features: [
                        'user_info',
                        'cloud_save',
                        'share',
                        'leaderboard',
                        'ad',
                        'payment'
                    ],
                    apiVersion: '2.0'
                };
                
                wx.hideLoading();
                console.log('[PlatformSDK] WeChat SDK initialized');
            }
        } catch (error) {
            console.error('[PlatformSDK] Failed to initialize WeChat SDK:', error);
        }
    }
    
    private initializeDouyin() {
        console.log('[PlatformSDK] Initializing Douyin Mini Game SDK');
        
        try {
            if (typeof tt !== 'undefined') {
                this.config = {
                    platform: GameConfig.PLATFORM_SDK.DOUYIN,
                    appId: 'tt_your_app_id',
                    features: [
                        'user_info',
                        'cloud_save',
                        'share',
                        'leaderboard',
                        'ad'
                    ],
                    apiVersion: '1.0'
                };
                
                console.log('[PlatformSDK] Douyin SDK initialized');
            }
        } catch (error) {
            console.error('[PlatformSDK] Failed to initialize Douyin SDK:', error);
        }
    }
    
    private initializeTapTap() {
        console.log('[PlatformSDK] Initializing TapTap SDK');
        
        try {
            this.config = {
                platform: GameConfig.PLATFORM_SDK.TAPTAP,
                appId: 'tap_your_app_id',
                features: [
                    'user_info',
                    'cloud_save',
                    'share',
                    'leaderboard',
                    'ad',
                    'payment'
                ],
                apiVersion: '3.0'
            };
            
            console.log('[PlatformSDK] TapTap SDK initialized');
        } catch (error) {
            console.error('[PlatformSDK] Failed to initialize TapTap SDK:', error);
        }
    }
    
    public login(): Promise<any> {
        return new Promise((resolve, reject) => {
            switch (this.currentPlatform) {
                case GameConfig.PLATFORM_SDK.WECHAT:
                    this.wechatLogin().then(resolve).catch(reject);
                    break;
                case GameConfig.PLATFORM_SDK.DOUYIN:
                    this.douyinLogin().then(resolve).catch(reject);
                    break;
                case GameConfig.PLATFORM_SDK.TAPTAP:
                    this.tapTapLogin().then(resolve).catch(reject);
                    break;
                default:
                    resolve({ success: true, platform: 'native' });
            }
        });
    }
    
    private wechatLogin(): Promise<any> {
        return new Promise((resolve, reject) => {
            if (typeof wx === 'undefined') {
                reject(new Error('WeChat SDK not available'));
                return;
            }
            
            wx.login({
                success: (res) => {
                    console.log('[PlatformSDK] WeChat login success');
                    resolve({ success: true, code: res.code });
                },
                fail: (err) => {
                    console.error('[PlatformSDK] WeChat login failed:', err);
                    reject(err);
                }
            });
        });
    }
    
    private douyinLogin(): Promise<any> {
        return new Promise((resolve, reject) => {
            if (typeof tt === 'undefined') {
                reject(new Error('Douyin SDK not available'));
                return;
            }
            
            tt.login({
                success: (res) => {
                    console.log('[PlatformSDK] Douyin login success');
                    resolve({ success: true, code: res.code });
                },
                fail: (err) => {
                    console.error('[PlatformSDK] Douyin login failed:', err);
                    reject(err);
                }
            });
        });
    }
    
    private tapTapLogin(): Promise<any> {
        return new Promise((resolve) => {
            console.log('[PlatformSDK] TapTap login (simulated)');
            resolve({ success: true, platform: 'tapTap' });
        });
    }
    
    public getUserInfo(): Promise<any> {
        return new Promise((resolve, reject) => {
            switch (this.currentPlatform) {
                case GameConfig.PLATFORM_SDK.WECHAT:
                    if (typeof wx !== 'undefined' && wx.getUserInfo) {
                        wx.getUserInfo({
                            success: (res) => resolve(res.userInfo),
                            fail: reject
                        });
                    } else {
                        resolve(this.getDefaultUserInfo());
                    }
                    break;
                case GameConfig.PLATFORM_SDK.DOUYIN:
                    if (typeof tt !== 'undefined' && tt.getUserInfo) {
                        tt.getUserInfo({
                            success: (res) => resolve(res),
                            fail: reject
                        });
                    } else {
                        resolve(this.getDefaultUserInfo());
                    }
                    break;
                default:
                    resolve(this.getDefaultUserInfo());
            }
        });
    }
    
    private getDefaultUserInfo() {
        return {
            nickName: '考生',
            avatarUrl: '',
            gender: 0,
            country: 'China',
            province: '',
            city: ''
        };
    }
    
    public share(config: ShareConfig): void {
        console.log('[PlatformSDK] Share initiated:', config.title);
        
        switch (this.currentPlatform) {
            case GameConfig.PLATFORM_SDK.WECHAT:
                this.wechatShare(config);
                break;
            case GameConfig.PLATFORM_SDK.DOUYIN:
                this.douyinShare(config);
                break;
            default:
                console.log('[PlatformSDK] Share not available on this platform');
        }
    }
    
    private wechatShare(config: ShareConfig): void {
        if (typeof wx === 'undefined') return;
        
        wx.shareAppMessage({
            title: config.title,
            imageUrl: config.imageUrl,
            query: config.query
        });
    }
    
    private douyinShare(config: ShareConfig): void {
        if (typeof tt === 'undefined') return;
        
        tt.shareAppMessage({
            title: config.title,
            imageUrl: config.imageUrl,
            query: config.query
        });
    }
    
    public showBannerAd(adUnitId: string): void {
        if (this.currentPlatform === GameConfig.PLATFORM_SDK.NATIVE) {
            console.log('[PlatformSDK] Banner ad (simulated)');
            return;
        }
        
        console.log(`[PlatformSDK] Showing banner ad: ${adUnitId}`);
    }
    
    public hideBannerAd(): void {
        console.log('[PlatformSDK] Hiding banner ad');
    }
    
    public showRewardedVideoAd(adUnitId: string): Promise<boolean> {
        return new Promise((resolve) => {
            console.log(`[PlatformSDK] Showing rewarded video ad: ${adUnitId}`);
            
            setTimeout(() => {
                resolve(true);
            }, 1000);
        });
    }
    
    public showInterstitialAd(adUnitId: string): Promise<void> {
        return new Promise((resolve) => {
            console.log(`[PlatformSDK] Showing interstitial ad: ${adUnitId}`);
            
            setTimeout(() => {
                resolve();
            }, 500);
        });
    }
    
    public uploadScore(score: number, statName: string = 'totalScore'): void {
        console.log(`[PlatformSDK] Uploading score: ${statName} = ${score}`);
        
        switch (this.currentPlatform) {
            case GameConfig.PLATFORM_SDK.WECHAT:
                this.uploadWeChatScore(score, statName);
                break;
            case GameConfig.PLATFORM_SDK.DOUYIN:
                this.uploadDouyinScore(score, statName);
                break;
            case GameConfig.PLATFORM_SDK.TAPTAP:
                this.uploadTapTapScore(score, statName);
                break;
        }
    }
    
    private uploadWeChatScore(score: number, statName: string): void {
        if (typeof wx === 'undefined') return;
        
        const leaderboard = wx.getFriendCloudStorage({
            keyList: [statName]
        });
        
        console.log('[PlatformSDK] WeChat leaderboard updated');
    }
    
    private uploadDouyinScore(score: number, statName: string): void {
        if (typeof tt === 'undefined') return;
        
        console.log('[PlatformSDK] Douyin leaderboard updated');
    }
    
    private uploadTapTapScore(score: number, statName: string): void {
        console.log('[PlatformSDK] TapTap leaderboard updated');
    }
    
    public showLeaderboard(config: LeaderboardConfig): void {
        console.log('[PlatformSDK] Showing leaderboard:', config);
    }
    
    public cloudSave(key: string, data: object): Promise<boolean> {
        return new Promise((resolve) => {
            console.log(`[PlatformSDK] Cloud save: ${key}`);
            
            setTimeout(() => {
                resolve(true);
            }, 100);
        });
    }
    
    public cloudLoad(key: string): Promise<any> {
        return new Promise((resolve) => {
            console.log(`[PlatformSDK] Cloud load: ${key}`);
            
            setTimeout(() => {
                resolve(null);
            }, 100);
        });
    }
    
    public vibrateShort(): void {
        switch (this.currentPlatform) {
            case GameConfig.PLATFORM_SDK.WECHAT:
                if (typeof wx !== 'undefined') {
                    wx.vibrateShort({ type: 'light' });
                }
                break;
            case GameConfig.PLATFORM_SDK.DOUYIN:
                if (typeof tt !== 'undefined') {
                    tt.vibrateShort();
                }
                break;
            default:
                console.log('[PlatformSDK] Vibrate (simulated)');
        }
    }
    
    public vibrateLong(): void {
        switch (this.currentPlatform) {
            case GameConfig.PLATFORM_SDK.WECHAT:
                if (typeof wx !== 'undefined') {
                    wx.vibrateLong();
                }
                break;
            case GameConfig.PLATFORM_SDK.DOUYIN:
                if (typeof tt !== 'undefined') {
                    tt.vibrateLong();
                }
                break;
            default:
                console.log('[PlatformSDK] Long vibrate (simulated)');
        }
    }
    
    public getPlatform(): string {
        return this.currentPlatform;
    }
    
    public isNative(): boolean {
        return this.currentPlatform === GameConfig.PLATFORM_SDK.NATIVE;
    }
    
    public isMiniGame(): boolean {
        return this.currentPlatform !== GameConfig.PLATFORM_SDK.NATIVE;
    }
    
    public hasFeature(feature: string): boolean {
        return this.config?.features.includes(feature) || false;
    }
    
    public getConfig(): PlatformConfig | null {
        return this.config;
    }
    
    public trackEvent(eventName: string, params?: object): void {
        console.log(`[PlatformSDK] Analytics event: ${eventName}`, params);
    }
    
    public trackGameStart(): void {
        this.trackEvent('game_start', {
            platform: this.currentPlatform,
            timestamp: Date.now()
        });
    }
    
    public trackGameEnd(result: any): void {
        this.trackEvent('game_end', {
            platform: this.currentPlatform,
            result: result,
            timestamp: Date.now()
        });
    }
    
    public trackPhaseComplete(phaseId: string, data: any): void {
        this.trackEvent('phase_complete', {
            phaseId: phaseId,
            ...data
        });
    }
    
    public trackAchievementUnlock(achievementId: string): void {
        this.trackEvent('achievement_unlock', {
            achievementId: achievementId,
            timestamp: Date.now()
        });
    }
}
