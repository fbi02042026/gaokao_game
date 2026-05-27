import { _decorator, Component } from 'cc';
import { GameConfig } from '../core/GameConfig';
import { TalentSystem } from './TalentSystem';
import { TimeSystem } from './TimeSystem';
const { ccclass, property } = _decorator;

export interface PlayerData {
    playerId: string;
    playthroughCount: number;
    currentPlaythrough: number;
    stats: PlayerStats;
    achievements: string[];
    choices: ChoiceRecord[];
    relationships: Relationship[];
    inventories: InventoryItem[];
    settings: GameSettings;
    meta: SaveMeta;
}

export interface PlayerStats {
    intelligence: number;
    physical: number;
    emotion: number;
    social: number;
    luck: number;
    creativity: number;
    memory: number;
    willpower: number;
    gaokaoScore: number;
    totalScore: number;
}

export interface ChoiceRecord {
    choiceId: string;
    choiceText: string;
    timestamp: number;
    gameDay: number;
    phase: string;
    consequence: string;
    wasOptimal: boolean;
}

export interface Relationship {
    characterId: string;
    characterName: string;
    level: number;
    intimacy: number;
    type: 'friend' | 'family' | 'romance' | 'teacher' | 'classmate';
    events: string[];
}

export interface InventoryItem {
    itemId: string;
    name: string;
    description: string;
    type: 'item' | 'gift' | 'book' | 'equipment';
    quantity: number;
    effect?: string;
}

export interface GameSettings {
    musicVolume: number;
    sfxVolume: number;
    voiceVolume: number;
    language: string;
    autoSave: boolean;
    showTutorial: boolean;
    notificationEnabled: boolean;
}

export interface SaveMeta {
    saveVersion: string;
    gameVersion: string;
    createdAt: number;
    lastPlayedAt: number;
    totalPlayTime: number;
    deviceInfo: string;
    platform: string;
}

@ccclass('SaveSystem')
export class SaveSystem extends Component {
    private static instance: SaveSystem;
    private currentSaveData: PlayerData | null = null;
    private autoSaveTimer: number = 0;
    private readonly AUTO_SAVE_INTERVAL = 300000;
    
    public static getInstance(): SaveSystem {
        if (!SaveSystem.instance) {
            SaveSystem.instance = new SaveSystem();
        }
        return SaveSystem.instance;
    }
    
    onLoad() {
        SaveSystem.instance = this;
    }
    
    start() {
        this.loadSettings();
        
        if (GameConfig.AUTO_SAVE_ENABLED) {
            this.scheduleAutoSave();
        }
    }
    
    update(deltaTime: number) {
        if (GameConfig.AUTO_SAVE_ENABLED) {
            this.autoSaveTimer += deltaTime * 1000;
            
            if (this.autoSaveTimer >= this.AUTO_SAVE_INTERVAL) {
                this.autoSave();
                this.autoSaveTimer = 0;
            }
        }
    }
    
    public createNewGame(): PlayerData {
        this.currentSaveData = {
            playerId: this.generatePlayerId(),
            playthroughCount: 0,
            currentPlaythrough: 1,
            stats: this.initializeStats(),
            achievements: [],
            choices: [],
            relationships: this.initializeRelationships(),
            inventories: this.initializeInventory(),
            settings: this.getDefaultSettings(),
            meta: this.createSaveMeta()
        };
        
        console.log('[SaveSystem] Created new game');
        this.applyNewGamePlusModifiers();
        this.autoSave();
        
        return this.currentSaveData;
    }
    
    private initializeStats(): PlayerStats {
        return {
            intelligence: 50,
            physical: 50,
            emotion: 50,
            social: 50,
            luck: 50,
            creativity: 50,
            memory: 50,
            willpower: 50,
            gaokaoScore: 0,
            totalScore: 0
        };
    }
    
    private initializeRelationships(): Relationship[] {
        return [
            {
                characterId: 'mom',
                characterName: '妈妈',
                level: 1,
                intimacy: 70,
                type: 'family',
                events: []
            },
            {
                characterId: 'dad',
                characterName: '爸爸',
                level: 1,
                intimacy: 70,
                type: 'family',
                events: []
            },
            {
                characterId: 'classmate_1',
                characterName: '小明',
                level: 1,
                intimacy: 50,
                type: 'classmate',
                events: []
            }
        ];
    }
    
    private initializeInventory(): InventoryItem[] {
        return [
            {
                itemId: 'textbook_chinese',
                name: '语文课本',
                description: '高中语文必修教材',
                type: 'book',
                quantity: 1,
                effect: 'study_chinese'
            },
            {
                itemId: 'textbook_math',
                name: '数学课本',
                description: '高中数学必修教材',
                type: 'book',
                quantity: 1,
                effect: 'study_math'
            },
            {
                itemId: 'textbook_english',
                name: '英语课本',
                description: '高中英语必修教材',
                type: 'book',
                quantity: 1,
                effect: 'study_english'
            }
        ];
    }
    
    private getDefaultSettings(): GameSettings {
        return {
            musicVolume: 0.8,
            sfxVolume: 1.0,
            voiceVolume: 1.0,
            language: 'zh_CN',
            autoSave: true,
            showTutorial: true,
            notificationEnabled: true
        };
    }
    
    private createSaveMeta(): SaveMeta {
        return {
            saveVersion: '1.0.0',
            gameVersion: GameConfig.VERSION,
            createdAt: Date.now(),
            lastPlayedAt: Date.now(),
            totalPlayTime: 0,
            deviceInfo: 'Tuanjie Engine',
            platform: this.detectPlatform()
        };
    }
    
    private detectPlatform(): string {
        if (typeof window !== 'undefined') {
            const ua = navigator.userAgent.toLowerCase();
            if (ua.includes('micromessenger')) return GameConfig.PLATFORM_SDK.WECHAT;
            if (ua.includes('bytedance')) return GameConfig.PLATFORM_SDK.DOUYIN;
        }
        return GameConfig.PLATFORM_SDK.NATIVE;
    }
    
    private generatePlayerId(): string {
        return 'player_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    }
    
    private applyNewGamePlusModifiers() {
        if (this.currentSaveData && this.currentSaveData.playthroughCount > 0) {
            console.log('[SaveSystem] Applying New Game+ modifiers');
            
            const TalentSystem = require('./TalentSystem').TalentSystem.getInstance();
            TalentSystem.unlockTalent('memory_past_life');
            
            Object.keys(this.currentSaveData.stats).forEach(stat => {
                if (typeof this.currentSaveData!.stats[stat] === 'number') {
                    this.currentSaveData!.stats[stat] = Math.min(
                        100,
                        this.currentSaveData!.stats[stat] * 1.1
                    );
                }
            });
            
            console.log('[SaveSystem] New Game+ modifiers applied');
        }
    }
    
    public recordChoice(choice: Omit<ChoiceRecord, 'timestamp' | 'gameDay'>) {
        if (!this.currentSaveData) return;
        
        const TimeSystem = require('./TimeSystem').TimeSystem.getInstance();
        
        const choiceRecord: ChoiceRecord = {
            ...choice,
            timestamp: Date.now(),
            gameDay: TimeSystem.getGameDay(),
            phase: TimeSystem.getCurrentPhase()?.id || 'unknown'
        };
        
        this.currentSaveData.choices.push(choiceRecord);
        console.log(`[SaveSystem] Recorded choice: ${choice.choiceText}`);
    }
    
    public updateStat(statName: keyof PlayerStats, delta: number) {
        if (!this.currentSaveData) return;
        
        if (statName in this.currentSaveData.stats) {
            const currentValue = this.currentSaveData.stats[statName];
            this.currentSaveData.stats[statName] = Math.max(0, Math.min(100, currentValue + delta));
            console.log(`[SaveSystem] Stat ${statName}: ${currentValue} -> ${this.currentSaveData.stats[statName]}`);
        }
    }
    
    public updateRelationship(characterId: string, intimacyDelta: number) {
        if (!this.currentSaveData) return;
        
        const relationship = this.currentSaveData.relationships.find(r => r.characterId === characterId);
        if (relationship) {
            relationship.intimacy = Math.max(0, Math.min(100, relationship.intimacy + intimacyDelta));
            
            if (relationship.intimacy >= 90) relationship.level = 5;
            else if (relationship.intimacy >= 70) relationship.level = 4;
            else if (relationship.intimacy >= 50) relationship.level = 3;
            else if (relationship.intimacy >= 30) relationship.level = 2;
            else relationship.level = 1;
            
            console.log(`[SaveSystem] Relationship ${relationship.characterName}: ${relationship.intimacy} (Level ${relationship.level})`);
        }
    }
    
    public addItem(item: InventoryItem) {
        if (!this.currentSaveData) return;
        
        const existingItem = this.currentSaveData.inventories.find(i => i.itemId === item.itemId);
        if (existingItem) {
            existingItem.quantity += item.quantity;
        } else {
            this.currentSaveData.inventories.push(item);
        }
        
        console.log(`[SaveSystem] Added item: ${item.name} (x${item.quantity})`);
    }
    
    public removeItem(itemId: string, quantity: number = 1): boolean {
        if (!this.currentSaveData) return false;
        
        const itemIndex = this.currentSaveData.inventories.findIndex(i => i.itemId === itemId);
        if (itemIndex === -1) return false;
        
        const item = this.currentSaveData.inventories[itemIndex];
        if (item.quantity < quantity) return false;
        
        item.quantity -= quantity;
        if (item.quantity <= 0) {
            this.currentSaveData.inventories.splice(itemIndex, 1);
        }
        
        return true;
    }
    
    public autoSave() {
        if (!this.currentSaveData) return;
        
        this.currentSaveData.meta.lastPlayedAt = Date.now();
        const saveJson = JSON.stringify(this.currentSaveData, null, 2);
        
        this.saveToStorage(GameConfig.SAVE_FILE_NAME, saveJson);
        console.log('[SaveSystem] Auto-saved game');
    }
    
    public saveGame(): boolean {
        if (!this.currentSaveData) {
            console.warn('[SaveSystem] No game data to save');
            return false;
        }
        
        try {
            this.currentSaveData.meta.lastPlayedAt = Date.now();
            const saveJson = JSON.stringify(this.currentSaveData, null, 2);
            
            this.saveToStorage(GameConfig.SAVE_FILE_NAME, saveJson);
            
            this.callPlatformSaveAPI();
            
            console.log('[SaveSystem] Game saved successfully');
            return true;
        } catch (error) {
            console.error('[SaveSystem] Failed to save game:', error);
            return false;
        }
    }
    
    public loadGame(): PlayerData | null {
        try {
            const saveJson = this.loadFromStorage(GameConfig.SAVE_FILE_NAME);
            
            if (saveJson) {
                this.currentSaveData = JSON.parse(saveJson);
                console.log('[SaveSystem] Game loaded successfully');
                return this.currentSaveData;
            } else {
                console.log('[SaveSystem] No save data found');
                return null;
            }
        } catch (error) {
            console.error('[SaveSystem] Failed to load game:', error);
            return null;
        }
    }
    
    public hasSaveData(): boolean {
        return this.loadFromStorage(GameConfig.SAVE_FILE_NAME) !== null;
    }
    
    public deleteSave(): boolean {
        try {
            this.removeFromStorage(GameConfig.SAVE_FILE_NAME);
            this.currentSaveData = null;
            console.log('[SaveSystem] Save data deleted');
            return true;
        } catch (error) {
            console.error('[SaveSystem] Failed to delete save:', error);
            return false;
        }
    }
    
    private saveToStorage(key: string, data: string) {
        if (typeof localStorage !== 'undefined') {
            localStorage.setItem(key, data);
        } else {
            console.warn('[SaveSystem] localStorage not available');
        }
    }
    
    private loadFromStorage(key: string): string | null {
        if (typeof localStorage !== 'undefined') {
            return localStorage.getItem(key);
        }
        return null;
    }
    
    private removeFromStorage(key: string) {
        if (typeof localStorage !== 'undefined') {
            localStorage.removeItem(key);
        }
    }
    
    private callPlatformSaveAPI() {
        const platform = this.currentSaveData?.meta.platform;
        
        switch (platform) {
            case GameConfig.PLATFORM_SDK.WECHAT:
                console.log('[SaveSystem] Calling WeChat cloud save API');
                break;
            case GameConfig.PLATFORM_SDK.DOUYIN:
                console.log('[SaveSystem] Calling Douyin cloud save API');
                break;
            case GameConfig.PLATFORM_SDK.TAPTAP:
                console.log('[SaveSystem] Calling TapTap cloud save API');
                break;
            default:
                console.log('[SaveSystem] Using local save only');
        }
    }
    
    private scheduleAutoSave() {
        console.log('[SaveSystem] Auto-save scheduled');
    }
    
    private loadSettings() {
        const settingsJson = this.loadFromStorage('game_settings');
        if (settingsJson && this.currentSaveData) {
            this.currentSaveData.settings = { ...this.getDefaultSettings(), ...JSON.parse(settingsJson) };
        }
    }
    
    public updateSettings(newSettings: Partial<GameSettings>) {
        if (!this.currentSaveData) return;
        
        this.currentSaveData.settings = { ...this.currentSaveData.settings, ...newSettings };
        this.saveToStorage('game_settings', JSON.stringify(this.currentSaveData.settings));
        console.log('[SaveSystem] Settings updated');
    }
    
    public getPlayerData(): PlayerData | null {
        return this.currentSaveData;
    }
    
    public startNewPlaythrough(): PlayerData | null {
        if (!this.currentSaveData) return null;
        
        this.currentSaveData.playthroughCount++;
        this.currentSaveData.currentPlaythrough = this.currentSaveData.playthroughCount + 1;
        
        this.applyNewGamePlusModifiers();
        
        console.log(`[SaveSystem] Starting playthrough #${this.currentSaveData.currentPlaythrough}`);
        return this.currentSaveData;
    }
    
    public exportSaveData(): string {
        if (!this.currentSaveData) return '';
        return JSON.stringify(this.currentSaveData, null, 2);
    }
    
    public importSaveData(jsonData: string): boolean {
        try {
            this.currentSaveData = JSON.parse(jsonData);
            console.log('[SaveSystem] Save data imported successfully');
            return true;
        } catch (error) {
            console.error('[SaveSystem] Failed to import save data:', error);
            return false;
        }
    }
}
