import { _decorator, Component, Node, Color, Vec3 } from 'cc';
import { SaveSystem } from '../systems/SaveSystem';
import { PlatformSDK } from '../platform/PlatformSDK';
const { ccclass, property } = _decorator;

@ccclass('MainMenuScene')
export class MainMenuScene extends Component {
    @property({ type: Node })
    public titleNode: Node = null;
    
    @property({ type: Node })
    public newGameButton: Node = null;
    
    @property({ type: Node })
    public continueButton: Node = null;
    
    @property({ type: Node })
    public settingsButton: Node = null;
    
    @property({ type: Node })
    public quitButton: Node = null;
    
    private hasSaveData: boolean = false;
    
    onLoad() {
        this.checkSaveData();
        this.setupButtons();
    }
    
    start() {
        this.animateTitle();
        this.checkAutoLogin();
    }
    
    private checkSaveData() {
        const SaveSystem = require('../systems/SaveSystem').SaveSystem.getInstance();
        this.hasSaveData = SaveSystem.hasSaveData();
        
        if (this.continueButton) {
            this.continueButton.active = this.hasSaveData;
        }
        
        console.log(`[MainMenu] Save data exists: ${this.hasSaveData}`);
    }
    
    private setupButtons() {
        if (this.newGameButton) {
            this.newGameButton.on('click', this.onNewGame, this);
        }
        
        if (this.continueButton) {
            this.continueButton.on('click', this.onContinue, this);
        }
        
        if (this.settingsButton) {
            this.settingsButton.on('click', this.onSettings, this);
        }
        
        if (this.quitButton) {
            this.quitButton.on('click', this.onQuit, this);
        }
    }
    
    private checkAutoLogin() {
        const PlatformSDK = require('../platform/PlatformSDK').PlatformSDK.getInstance();
        
        if (PlatformSDK.isMiniGame()) {
            PlatformSDK.login()
                .then(() => {
                    console.log('[MainMenu] Platform login success');
                    this.trackLoginEvent();
                })
                .catch((err) => {
                    console.warn('[MainMenu] Platform login failed:', err);
                });
        }
    }
    
    private trackLoginEvent() {
        const PlatformSDK = require('../platform/PlatformSDK').PlatformSDK.getInstance();
        PlatformSDK.trackEvent('menu_view', {
            has_save_data: this.hasSaveData
        });
    }
    
    private animateTitle() {
        if (!this.titleNode) return;
        
        console.log('[MainMenu] Playing title animation');
        
        const originalPos = this.titleNode.getPosition();
        
        setTimeout(() => {
            console.log('[MainMenu] Title animation completed');
        }, 100);
    }
    
    private onNewGame() {
        console.log('[MainMenu] New game clicked');
        
        const SaveSystem = require('../systems/SaveSystem').SaveSystem.getInstance();
        
        if (this.hasSaveData) {
            this.showConfirmDialog(
                '开始新游戏',
                '确定要开始新游戏吗？当前进度将被覆盖。',
                () => this.startNewGame()
            );
        } else {
            this.startNewGame();
        }
    }
    
    private startNewGame() {
        const SaveSystem = require('../systems/SaveSystem').SaveSystem.getInstance();
        const PlatformSDK = require('../platform/PlatformSDK').PlatformSDK.getInstance();
        
        SaveSystem.createNewGame();
        
        PlatformSDK.trackEvent('new_game_start', {
            playthrough: 1
        });
        
        this.loadGameScene();
    }
    
    private onContinue() {
        console.log('[MainMenu] Continue clicked');
        
        const SaveSystem = require('../systems/SaveSystem').SaveSystem.getInstance();
        const playerData = SaveSystem.loadGame();
        
        if (playerData) {
            this.loadGameScene();
            
            const PlatformSDK = require('../platform/PlatformSDK').PlatformSDK.getInstance();
            PlatformSDK.trackEvent('continue_game', {
                playthrough: playerData.currentPlaythrough
            });
        } else {
            this.showNotification('读取存档失败');
        }
    }
    
    private onSettings() {
        console.log('[MainMenu] Settings clicked');
        
        const UIManager = require('../ui/UIManager').UIManager.getInstance();
        if (UIManager) {
            UIManager.showSettings();
        }
    }
    
    private onQuit() {
        console.log('[MainMenu] Quit clicked');
        
        const PlatformSDK = require('../platform/PlatformSDK').PlatformSDK.getInstance();
        
        if (PlatformSDK.isMiniGame()) {
            this.showConfirmDialog(
                '退出游戏',
                '确定要退出游戏吗？',
                () => this.quitGame()
            );
        } else {
            this.quitGame();
        }
    }
    
    private loadGameScene() {
        console.log('[MainMenu] Loading game scene');
    }
    
    private quitGame() {
        const PlatformSDK = require('../platform/PlatformSDK').PlatformSDK.getInstance();
        
        if (PlatformSDK.isMiniGame()) {
            console.log('[MainMenu] Quitting mini game');
        } else {
            console.log('[MainMenu] Quitting application');
        }
    }
    
    private showConfirmDialog(title: string, message: string, onConfirm: () => void) {
        console.log(`[MainMenu] Confirm dialog: ${title}`);
        console.log(`[MainMenu] Message: ${message}`);
        
        setTimeout(() => {
            onConfirm();
        }, 100);
    }
    
    private showNotification(message: string) {
        console.log(`[MainMenu] Notification: ${message}`);
    }
    
    update(deltaTime: number) {
    }
}
