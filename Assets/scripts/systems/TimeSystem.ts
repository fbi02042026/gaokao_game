import { _decorator, Component } from 'cc';
import { GameConfig } from '../core/GameConfig';
const { ccclass, property } = _decorator;

export interface GameTime {
    year: number;
    month: number;
    day: number;
    hour: number;
    minute: number;
    gameDay: number;
}

export interface GamePhase {
    id: string;
    name: string;
    startYear: number;
    startMonth: number;
    startDay: number;
    endYear: number;
    endMonth: number;
    endDay: number;
    isCompleted: boolean;
}

@ccclass('TimeSystem')
export class TimeSystem extends Component {
    private static instance: TimeSystem;
    
    private currentTime: GameTime = {
        year: 2024,
        month: 9,
        day: 1,
        hour: 8,
        minute: 0,
        gameDay: 1
    };
    
    private gamePhases: GamePhase[] = [];
    private currentPhaseIndex: number = 0;
    private isPaused: boolean = false;
    private lastUpdateTime: number = 0;
    
    private timeScale: number = 1.0;
    private readonly BASE_TICK_INTERVAL = 1000;
    
    public static getInstance(): TimeSystem {
        if (!TimeSystem.instance) {
            TimeSystem.instance = new TimeSystem();
        }
        return TimeSystem.instance;
    }
    
    onLoad() {
        TimeSystem.instance = this;
        this.initializePhases();
    }
    
    start() {
        this.lastUpdateTime = Date.now();
        this.loadTimeFromSave();
        console.log('[TimeSystem] Time system started');
    }
    
    update(deltaTime: number) {
        if (this.isPaused) return;
        
        const currentTime = Date.now();
        const elapsed = (currentTime - this.lastUpdateTime) * this.timeScale;
        
        if (elapsed >= this.BASE_TICK_INTERVAL) {
            this.advanceTime(Math.floor(elapsed / this.BASE_TICK_INTERVAL));
            this.lastUpdateTime = currentTime;
        }
    }
    
    private initializePhases() {
        this.gamePhases = [
            {
                id: GameConfig.GAME_PHASES.HIGH_SCHOOL,
                name: '高中三年',
                startYear: 2024,
                startMonth: 9,
                startDay: 1,
                endYear: 2027,
                endMonth: 6,
                endDay: 7,
                isCompleted: false
            },
            {
                id: GameConfig.GAME_PHASES.GAOKAO,
                name: '高考',
                startYear: 2027,
                startMonth: 6,
                startDay: 7,
                endYear: 2027,
                endMonth: 6,
                endDay: 9,
                isCompleted: false
            },
            {
                id: GameConfig.GAME_PHASES.CHOOSE_MAJOR,
                name: '填报志愿',
                startYear: 2027,
                startMonth: 6,
                startDay: 25,
                endYear: 2027,
                startMonth: 7,
                startDay: 15,
                isCompleted: false
            },
            {
                id: GameConfig.GAME_PHASES.UNIVERSITY,
                name: '大学四年',
                startYear: 2027,
                startMonth: 9,
                startDay: 1,
                endYear: 2031,
                endMonth: 6,
                endDay: 30,
                isCompleted: false
            },
            {
                id: GameConfig.GAME_PHASES.CAREER,
                name: '职场人生',
                startYear: 2031,
                startMonth: 7,
                startDay: 1,
                endYear: 2050,
                endMonth: 12,
                endDay: 31,
                isCompleted: false
            }
        ];
        
        console.log(`[TimeSystem] Initialized ${this.gamePhases.length} game phases`);
    }
    
    private advanceTime(ticks: number) {
        for (let i = 0; i < ticks; i++) {
            this.currentTime.minute += 30;
            
            if (this.currentTime.minute >= 60) {
                this.currentTime.minute = 0;
                this.currentTime.hour++;
                
                if (this.currentTime.hour >= 24) {
                    this.currentTime.hour = 0;
                    this.advanceDay();
                }
            }
        }
        
        this.checkPhaseTransition();
        this.emitTimeUpdate();
    }
    
    private advanceDay() {
        this.currentTime.day++;
        this.currentTime.gameDay++;
        
        const daysInMonth = this.getDaysInMonth(this.currentTime.month, this.currentTime.year);
        
        if (this.currentTime.day > daysInMonth) {
            this.currentTime.day = 1;
            this.currentTime.month++;
            
            if (this.currentTime.month > 12) {
                this.currentTime.month = 1;
                this.currentTime.year++;
            }
        }
        
        console.log(`[TimeSystem] Day ${this.currentTime.gameDay}: ${this.formatDate()}`);
        this.emitDayChange();
    }
    
    private checkPhaseTransition() {
        const currentPhase = this.gamePhases[this.currentPhaseIndex];
        if (!currentPhase) return;
        
        if (this.isDateAfter(this.currentTime, {
            year: currentPhase.endYear,
            month: currentPhase.endMonth,
            day: currentPhase.endDay,
            hour: 0,
            minute: 0,
            gameDay: 0
        })) {
            this.completePhase(currentPhase.id);
        }
    }
    
    private completePhase(phaseId: string) {
        const phase = this.gamePhases.find(p => p.id === phaseId);
        if (phase) {
            phase.isCompleted = true;
            this.currentPhaseIndex++;
            console.log(`[TimeSystem] Phase completed: ${phase.name}`);
            
            this.emitPhaseChange(phase);
            
            if (this.currentPhaseIndex >= this.gamePhases.length) {
                this.emitGameComplete();
            }
        }
    }
    
    private isDateAfter(time1: GameTime, time2: any): boolean {
        if (time1.year !== time2.year) return time1.year > time2.year;
        if (time1.month !== time2.month) return time1.month > time2.month;
        if (time1.day !== time2.day) return time1.day > time2.day;
        return false;
    }
    
    private getDaysInMonth(month: number, year: number): number {
        return new Date(year, month, 0).getDate();
    }
    
    public getCurrentTime(): GameTime {
        return { ...this.currentTime };
    }
    
    public formatDate(): string {
        return `${this.currentTime.year}年${this.currentTime.month}月${this.currentTime.day}日`;
    }
    
    public formatTime(): string {
        return `${String(this.currentTime.hour).padStart(2, '0')}:${String(this.currentTime.minute).padStart(2, '0')}`;
    }
    
    public getCurrentPhase(): GamePhase | null {
        return this.gamePhases[this.currentPhaseIndex] || null;
    }
    
    public getAllPhases(): GamePhase[] {
        return [...this.gamePhases];
    }
    
    public getGameDay(): number {
        return this.currentTime.gameDay;
    }
    
    public setTimeScale(scale: number) {
        this.timeScale = Math.max(0.1, Math.min(10, scale));
        console.log(`[TimeSystem] Time scale set to ${this.timeScale}x`);
    }
    
    public pause() {
        this.isPaused = true;
        console.log('[TimeSystem] Game paused');
    }
    
    public resume() {
        this.isPaused = false;
        this.lastUpdateTime = Date.now();
        console.log('[TimeSystem] Game resumed');
    }
    
    public skipToNextDay() {
        this.currentTime.hour = 23;
        this.currentTime.minute = 59;
        this.emitTimeUpdate();
    }
    
    public advanceDays(days: number) {
        for (let i = 0; i < days; i++) {
            this.advanceDay();
        }
    }
    
    public simulateCycle() {
        const cycleHours = GameConfig.CYCLE_HOURS;
        const ticksPerHour = 2;
        const totalTicks = cycleHours * ticksPerHour;
        this.advanceTime(totalTicks);
        console.log(`[TimeSystem] Simulated ${cycleHours} game hours (${GameConfig.CYCLE_DAYS} days)`);
    }
    
    private emitTimeUpdate() {
        this.node.emit('timeUpdate', this.getCurrentTime());
    }
    
    private emitDayChange() {
        this.node.emit('dayChange', this.currentTime.gameDay);
    }
    
    private emitPhaseChange(phase: GamePhase) {
        this.node.emit('phaseChange', phase);
    }
    
    private emitGameComplete() {
        this.node.emit('gameComplete', null);
        console.log('[TimeSystem] Game completed all phases!');
    }
    
    public saveTimeToSave(): object {
        return {
            currentTime: this.currentTime,
            currentPhaseIndex: this.currentPhaseIndex,
            phases: this.gamePhases,
            timestamp: Date.now()
        };
    }
    
    private loadTimeFromSave() {
        try {
            const SaveSystem = require('./SaveSystem');
            const saveData = SaveSystem?.loadGame();
            
            if (saveData && saveData.gameTime) {
                this.currentTime = saveData.gameTime.currentTime || this.currentTime;
                this.currentPhaseIndex = saveData.gameTime.currentPhaseIndex || 0;
                this.gamePhases = saveData.gameTime.phases || this.gamePhases;
                console.log('[TimeSystem] Loaded time from save');
            }
        } catch (e) {
            console.log('[TimeSystem] No saved time data found');
        }
    }
}
