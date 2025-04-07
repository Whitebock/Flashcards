export interface Card {
    front: string;
    back: string;
}

export interface DeckStats {
    notSeen: number;
    correct: number;
    incorrect: number;
    total: number;
}

export interface Deck {
    id: string;
    name: string;
    stats: DeckStats;
}

export enum CardStatus {
    NotSeen,
    Correct,
    Incorrect
}