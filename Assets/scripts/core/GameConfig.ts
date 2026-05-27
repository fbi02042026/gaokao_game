import { _decorator, Component } from 'cc';
const { ccclass, property } = _decorator;

@ccclass('GameConfig')
export class GameConfig extends Component {
    public static readonly GAME_TITLE = '高考人生模拟器';
    public static readonly VERSION = '1.0.0';
    
    public static readonly ASPECT_RATIO_WIDTH = 9;
    public static readonly ASPECT_RATIO_HEIGHT = 16;
    
    public static readonly GAME_PHASES = {
        HIGH_SCHOOL: 'high_school',
        GAOKAO: 'gaokao',
        CHOOSE_MAJOR: 'choose_major',
        UNIVERSITY: 'university',
        CAREER: 'career'
    };
    
    public static readonly CYCLE_DAYS = 2;
    public static readonly CYCLE_HOURS = 72;
    
    public static readonly MAX_PLAYTHROUGH_HOURS = 30;
    
    public static readonly PLATFORM_SDK = {
        WECHAT: 'wechat',
        DOUYIN: 'douyin',
        TAPTAP: 'tapTap',
        NATIVE: 'native'
    };
    
    public static readonly SAVE_FILE_NAME = 'game_save.json';
    public static readonly AUTO_SAVE_ENABLED = true;
    
    start() {
        this.loadGameSettings();
    }
    
    private loadGameSettings() {
        console.log(`[GameConfig] ${GameConfig.GAME_TITLE} v${GameConfig.VERSION}`);
        console.log('[GameConfig] Target Platform: Multi-platform Mini Games');
        console.log('[GameConfig] Aspect Ratio: 9:16 (Portrait)');
    }
}
