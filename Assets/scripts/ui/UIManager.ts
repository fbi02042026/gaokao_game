import { _decorator, Component, Node, Prefab, instantiate } from 'cc';
import { GameConfig } from '../core/GameConfig';
const { ccclass, property } = _decorator;

export enum UIType {
    MAIN_MENU = 'main_menu',
    SETTINGS = 'settings',
    BACKPACK = 'backpack',
    TALENT_PANEL = 'talent_panel',
    STATS_PANEL = 'stats_panel',
    DIALOGUE = 'dialogue',
    EVENT_PANEL = 'event_panel',
    PHASE_TRANSITION = 'phase_transition',
    SAVE_LOAD = 'save_load',
    GAME_OVER = 'game_over'
}

export interface UIConfig {
    type: UIType;
    prefabPath: string;
    layer: 'background' | 'game' | 'foreground' | 'overlay';
    isModal: boolean;
    showAnimation: string;
    hideAnimation: string;
}

@ccclass('UIManager')
export class UIManager extends Component {
    private static instance: UIManager;
    private uiPanels: Map<UIType, Node> = new Map();
    private uiStack: UIType[] = [];
    private currentUI: UIType | null = null;
    
    @property({ type: Node })
    public backgroundLayer: Node = null;
    
    @property({ type: Node })
    public gameLayer: Node = null;
    
    @property({ type: Node })
    public foregroundLayer: Node = null;
    
    @property({ type: Node })
    public overlayLayer: Node = null;
    
    public static getInstance(): UIManager {
        return UIManager.instance;
    }
    
    onLoad() {
        UIManager.instance = this;
        this.initializeLayers();
    }
    
    start() {
        this.setupUIConfigs();
    }
    
    private initializeLayers() {
        if (!this.backgroundLayer) {
            console.warn('[UIManager] UI layers not assigned');
        }
    }
    
    private setupUIConfigs() {
        console.log('[UIManager] UI system initialized');
    }
    
    public showUI<T extends Component>(uiType: UIType, data?: any): T | null {
        if (this.uiPanels.has(uiType)) {
            this.showExistingUI(uiType);
            return this.uiPanels.get(uiType)?.getComponent(uiType as any) as T;
        }
        
        const panel = this.createUIPanel(uiType);
        if (!panel) return null;
        
        this.placeUIPanel(panel, uiType);
        this.uiPanels.set(uiType, panel);
        this.uiStack.push(uiType);
        this.currentUI = uiType;
        
        this.playShowAnimation(panel, uiType);
        
        console.log(`[UIManager] Showing UI: ${uiType}`);
        return panel.getComponent(uiType as any) as T;
    }
    
    private createUIPanel(uiType: UIType): Node | null {
        const panel = new Node(uiType);
        panel.parent = this.getLayerForUI(uiType);
        return panel;
    }
    
    private placeUIPanel(panel: Node, uiType: UIType) {
        const config = this.getUIConfig(uiType);
        const layer = this.getLayerForUI(uiType);
        
        if (layer) {
            panel.parent = layer;
        }
        
        panel.setPosition(0, 0, 0);
        panel.active = true;
    }
    
    private getLayerForUI(uiType: UIType): Node {
        const config = this.getUIConfig(uiType);
        
        switch (config?.layer) {
            case 'background':
                return this.backgroundLayer;
            case 'game':
                return this.gameLayer;
            case 'foreground':
                return this.foregroundLayer;
            case 'overlay':
                return this.overlayLayer;
            default:
                return this.gameLayer;
        }
    }
    
    private getUIConfig(uiType: UIType): UIConfig | null {
        const configs: Record<UIType, UIConfig> = {
            [UIType.MAIN_MENU]: {
                type: UIType.MAIN_MENU,
                prefabPath: '',
                layer: 'overlay',
                isModal: false,
                showAnimation: 'fade_in',
                hideAnimation: 'fade_out'
            },
            [UIType.SETTINGS]: {
                type: UIType.SETTINGS,
                prefabPath: '',
                layer: 'overlay',
                isModal: true,
                showAnimation: 'slide_up',
                hideAnimation: 'slide_down'
            },
            [UIType.BACKPACK]: {
                type: UIType.BACKPACK,
                prefabPath: '',
                layer: 'foreground',
                isModal: false,
                showAnimation: 'scale_in',
                hideAnimation: 'scale_out'
            },
            [UIType.TALENT_PANEL]: {
                type: UIType.TALENT_PANEL,
                prefabPath: '',
                layer: 'foreground',
                isModal: false,
                showAnimation: 'slide_right',
                hideAnimation: 'slide_left'
            },
            [UIType.STATS_PANEL]: {
                type: UIType.STATS_PANEL,
                prefabPath: '',
                layer: 'foreground',
                isModal: false,
                showAnimation: 'fade_in',
                hideAnimation: 'fade_out'
            },
            [UIType.DIALOGUE]: {
                type: UIType.DIALOGUE,
                prefabPath: '',
                layer: 'foreground',
                isModal: false,
                showAnimation: 'type_writer',
                hideAnimation: 'fade_out'
            },
            [UIType.EVENT_PANEL]: {
                type: UIType.EVENT_PANEL,
                prefabPath: '',
                layer: 'overlay',
                isModal: true,
                showAnimation: 'scale_in',
                hideAnimation: 'scale_out'
            },
            [UIType.PHASE_TRANSITION]: {
                type: UIType.PHASE_TRANSITION,
                prefabPath: '',
                layer: 'overlay',
                isModal: true,
                showAnimation: 'fade_in_slow',
                hideAnimation: 'fade_out_slow'
            },
            [UIType.SAVE_LOAD]: {
                type: UIType.SAVE_LOAD,
                prefabPath: '',
                layer: 'overlay',
                isModal: true,
                showAnimation: 'fade_in',
                hideAnimation: 'fade_out'
            },
            [UIType.GAME_OVER]: {
                type: UIType.GAME_OVER,
                prefabPath: '',
                layer: 'overlay',
                isModal: true,
                showAnimation: 'fade_in_slow',
                hideAnimation: 'fade_out_slow'
            }
        };
        
        return configs[uiType] || null;
    }
    
    private playShowAnimation(panel: Node, uiType: UIType) {
        const config = this.getUIConfig(uiType);
        console.log(`[UIManager] Playing show animation: ${config?.showAnimation || 'none'}`);
    }
    
    private showExistingUI(uiType: UIType) {
        const panel = this.uiPanels.get(uiType);
        if (panel) {
            panel.active = true;
            this.uiStack.push(uiType);
            this.currentUI = uiType;
        }
    }
    
    public hideUI(uiType: UIType, destroy: boolean = false) {
        const panel = this.uiPanels.get(uiType);
        if (!panel) return;
        
        this.playHideAnimation(panel, uiType);
        
        setTimeout(() => {
            if (destroy) {
                panel.destroy();
                this.uiPanels.delete(uiType);
            } else {
                panel.active = false;
            }
            
            const index = this.uiStack.indexOf(uiType);
            if (index > -1) {
                this.uiStack.splice(index, 1);
            }
            
            if (this.uiStack.length > 0) {
                this.currentUI = this.uiStack[this.uiStack.length - 1];
            } else {
                this.currentUI = null;
            }
            
            console.log(`[UIManager] Hidden UI: ${uiType}`);
        }, 300);
    }
    
    private playHideAnimation(panel: Node, uiType: UIType) {
        const config = this.getUIConfig(uiType);
        console.log(`[UIManager] Playing hide animation: ${config?.hideAnimation || 'none'}`);
    }
    
    public hideAllUI() {
        this.uiPanels.forEach((panel, uiType) => {
            this.hideUI(uiType, false);
        });
        this.uiStack = [];
        this.currentUI = null;
        console.log('[UIManager] All UI hidden');
    }
    
    public isUIVisible(uiType: UIType): boolean {
        const panel = this.uiPanels.get(uiType);
        return panel ? panel.active : false;
    }
    
    public getCurrentUI(): UIType | null {
        return this.currentUI;
    }
    
    public isAnyUIOpen(): boolean {
        return this.currentUI !== null;
    }
    
    public showMainMenu() {
        this.showUI(UIType.MAIN_MENU);
    }
    
    public hideMainMenu() {
        this.hideUI(UIType.MAIN_MENU, true);
    }
    
    public showSettings() {
        this.showUI(UIType.SETTINGS);
    }
    
    public showBackpack() {
        this.showUI(UIType.BACKPACK);
    }
    
    public showTalentPanel() {
        this.showUI(UIType.TALENT_PANEL);
    }
    
    public showStatsPanel() {
        this.showUI(UIType.STATS_PANEL);
    }
    
    public showDialogue(speaker: string, text: string, onComplete?: () => void) {
        const dialogueUI = this.showUI<any>(UIType.DIALOGUE);
        if (dialogueUI) {
            dialogueUI.show(speaker, text, onComplete);
        }
    }
    
    public showEvent(eventData: any) {
        const eventUI = this.showUI<any>(UIType.EVENT_PANEL);
        if (eventUI) {
            eventUI.showEvent(eventData);
        }
    }
    
    public showPhaseTransition(phaseName: string, callback?: () => void) {
        const transitionUI = this.showUI<any>(UIType.PHASE_TRANSITION);
        if (transitionUI) {
            transitionUI.show(phaseName, callback);
        }
    }
    
    public showSaveLoad() {
        this.showUI(UIType.SAVE_LOAD);
    }
    
    public showGameOver(results: any) {
        const gameOverUI = this.showUI<any>(UIType.GAME_OVER);
        if (gameOverUI) {
            gameOverUI.showResults(results);
        }
    }
    
    public showNotification(message: string, duration: number = 3000) {
        console.log(`[UIManager] Notification: ${message}`);
        
        setTimeout(() => {
            console.log(`[UIManager] Notification dismissed`);
        }, duration);
    }
    
    public showConfirmDialog(
        title: string,
        message: string,
        onConfirm: () => void,
        onCancel?: () => void
    ) {
        console.log(`[UIManager] Confirm dialog: ${title}`);
        console.log(`[UIManager] Message: ${message}`);
        
        setTimeout(() => {
            onConfirm();
        }, 100);
    }
    
    public updateUI() {
        this.uiPanels.forEach((panel, uiType) => {
            if (panel.active) {
                this.refreshPanel(panel, uiType);
            }
        });
    }
    
    private refreshPanel(panel: Node, uiType: UIType) {
        console.log(`[UIManager] Refreshing panel: ${uiType}`);
    }
}
