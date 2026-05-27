import { _decorator, Component } from 'cc';
const { ccclass, property } = _decorator;

export interface TalentData {
    id: string;
    name: string;
    description: string;
    icon: string;
    rarity: 'common' | 'rare' | 'epic' | 'legendary';
    effects: TalentEffect[];
    unlockCondition: string;
    maxLevel: number;
}

export interface TalentEffect {
    type: 'stat_boost' | 'unlock_feature' | 'special_ability' | 'story_branch';
    target: string;
    value: number;
    description: string;
}

export interface PlayerTalent {
    talentId: string;
    currentLevel: number;
    isUnlocked: boolean;
    unlockTime?: number;
}

@ccclass('TalentSystem')
export class TalentSystem extends Component {
    private static instance: TalentSystem;
    private talents: Map<string, TalentData> = new Map();
    private playerTalents: PlayerTalent[] = [];
    private readonly MAX_TALENTS = 8;
    
    public static getInstance(): TalentSystem {
        if (!TalentSystem.instance) {
            TalentSystem.instance = new TalentSystem();
        }
        return TalentSystem.instance;
    }
    
    onLoad() {
        TalentSystem.instance = this;
        this.initializeTalents();
    }
    
    start() {
        this.loadTalentsFromSave();
    }
    
    private initializeTalents() {
        const defaultTalents: TalentData[] = [
            {
                id: 'memory_past_life',
                name: '前世记忆',
                description: '二周目专属天赋，记得所有选择带来的结果',
                icon: 'talent_memory',
                rarity: 'legendary',
                effects: [
                    {
                        type: 'special_ability',
                        target: 'all_choices',
                        value: 1,
                        description: '重复选择时触发既视感提示'
                    }
                ],
                unlockCondition: 'complete_one_playthrough',
                maxLevel: 1
            },
            {
                id: 'photographic_memory',
                name: '过目不忘',
                description: '记忆力大幅提升，学习效率+50%',
                icon: 'talent_memory',
                rarity: 'rare',
                effects: [
                    {
                        type: 'stat_boost',
                        target: 'study_efficiency',
                        value: 50,
                        description: '学习效率提升50%'
                    }
                ],
                unlockCondition: 'random_chance',
                maxLevel: 3
            },
            {
                id: 'emotional_control',
                name: '情绪管理',
                description: '考试时不易紧张，发挥稳定',
                icon: 'talent_calm',
                rarity: 'common',
                effects: [
                    {
                        type: 'stat_boost',
                        target: 'exam_stability',
                        value: 30,
                        description: '考试发挥稳定性+30%'
                    }
                ],
                unlockCondition: 'story_event',
                maxLevel: 5
            },
            {
                id: 'time_manipulation',
                name: '时间操控',
                description: '每天比别人多出2小时学习时间',
                icon: 'talent_time',
                rarity: 'epic',
                effects: [
                    {
                        type: 'stat_boost',
                        target: 'daily_study_time',
                        value: 120,
                        description: '每日可用学习时间+2小时'
                    }
                ],
                unlockCondition: 'special_choice',
                maxLevel: 1
            },
            {
                id: 'social_butterfly',
                name: '社交达人',
                description: '人脉广泛，获得更多隐藏事件',
                icon: 'talent_social',
                rarity: 'rare',
                effects: [
                    {
                        type: 'unlock_feature',
                        target: 'hidden_events',
                        value: 5,
                        description: '解锁5个隐藏社交事件'
                    }
                ],
                unlockCondition: 'relationship_max',
                maxLevel: 1
            },
            {
                id: 'lucky_star',
                name: '幸运星',
                description: '运势爆棚，关键时刻运气+40%',
                icon: 'talent_lucky',
                rarity: 'epic',
                effects: [
                    {
                        type: 'stat_boost',
                        target: 'luck',
                        value: 40,
                        description: '关键时刻成功率+40%'
                    }
                ],
                unlockCondition: 'random_chance',
                maxLevel: 3
            },
            {
                id: 'quick_learner',
                name: '速学者',
                description: '快速掌握新知识，理解力+30%',
                icon: 'talent_study',
                rarity: 'common',
                effects: [
                    {
                        type: 'stat_boost',
                        target: 'comprehension',
                        value: 30,
                        description: '理解力提升30%'
                    }
                ],
                unlockCondition: 'auto_unlock_early',
                maxLevel: 5
            },
            {
                id: 'physical_genius',
                name: '运动天才',
                description: '体育满分，强身健体',
                icon: 'talent_sport',
                rarity: 'common',
                effects: [
                    {
                        type: 'stat_boost',
                        target: 'physical_score',
                        value: 100,
                        description: '体育成绩必定满分'
                    }
                ],
                unlockCondition: 'auto_unlock_early',
                maxLevel: 1
            }
        ];
        
        defaultTalents.forEach(talent => {
            this.talents.set(talent.id, talent);
        });
        
        console.log(`[TalentSystem] Initialized ${this.talents.size} talents`);
    }
    
    public getTalent(talentId: string): TalentData | undefined {
        return this.talents.get(talentId);
    }
    
    public getAllTalents(): TalentData[] {
        return Array.from(this.talents.values());
    }
    
    public getTalentsByRarity(rarity: string): TalentData[] {
        return this.getAllTalents().filter(t => t.rarity === rarity);
    }
    
    public unlockTalent(talentId: string): boolean {
        if (this.playerTalents.length >= this.MAX_TALENTS) {
            console.warn('[TalentSystem] Maximum talents reached');
            return false;
        }
        
        const talent = this.talents.get(talentId);
        if (!talent) {
            console.error(`[TalentSystem] Talent ${talentId} not found`);
            return false;
        }
        
        const existingTalent = this.playerTalents.find(t => t.talentId === talentId);
        if (existingTalent) {
            console.warn(`[TalentSystem] Talent ${talentId} already unlocked`);
            return false;
        }
        
        const playerTalent: PlayerTalent = {
            talentId: talentId,
            currentLevel: 1,
            isUnlocked: true,
            unlockTime: Date.now()
        };
        
        this.playerTalents.push(playerTalent);
        console.log(`[TalentSystem] Unlocked talent: ${talent.name}`);
        return true;
    }
    
    public upgradeTalent(talentId: string): boolean {
        const playerTalent = this.playerTalents.find(t => t.talentId === talentId);
        const talent = this.talents.get(talentId);
        
        if (!playerTalent || !talent) {
            return false;
        }
        
        if (playerTalent.currentLevel >= talent.maxLevel) {
            console.warn(`[TalentSystem] Talent ${talentId} already at max level`);
            return false;
        }
        
        playerTalent.currentLevel++;
        console.log(`[TalentSystem] Upgraded talent ${talentId} to level ${playerTalent.currentLevel}`);
        return true;
    }
    
    public getPlayerTalents(): PlayerTalent[] {
        return [...this.playerTalents];
    }
    
    public getActiveTalents(): TalentData[] {
        return this.playerTalents
            .filter(pt => pt.isUnlocked)
            .map(pt => this.talents.get(pt.talentId))
            .filter(t => t !== undefined) as TalentData[];
    }
    
    public calculateTalentBonus(statType: string): number {
        let totalBonus = 0;
        
        this.getActiveTalents().forEach(talent => {
            talent.effects.forEach(effect => {
                if (effect.type === 'stat_boost' && effect.target === statType) {
                    totalBonus += effect.value * this.getTalentLevel(talent.id);
                }
            });
        });
        
        return totalBonus;
    }
    
    private getTalentLevel(talentId: string): number {
        const playerTalent = this.playerTalents.find(t => t.talentId === talentId);
        return playerTalent ? playerTalent.currentLevel : 0;
    }
    
    public hasTalent(talentId: string): boolean {
        return this.playerTalents.some(t => t.talentId === talentId && t.isUnlocked);
    }
    
    public triggerPastLifeMemory(): boolean {
        return this.hasTalent('memory_past_life');
    }
    
    public saveTalentsToSave(): object {
        return {
            talents: this.playerTalents,
            timestamp: Date.now()
        };
    }
    
    private loadTalentsFromSave() {
        const SaveSystem = require('./SaveSystem');
        const saveData = SaveSystem?.loadGame();
        
        if (saveData && saveData.talents) {
            this.playerTalents = saveData.talents;
            console.log(`[TalentSystem] Loaded ${this.playerTalents.length} talents from save`);
        }
    }
}
