export interface Card {
    question: string;
    answer: string;
    status: CardStatus;
}

export interface CardStack {
    id: string;
    name: string;
    cards: Card[];
}

export enum CardStatus {
    NotSeen,
    Correct,
    Incorrect
}