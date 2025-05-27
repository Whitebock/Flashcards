<div class="ui small card">
    <div class="content">
        <i class="right floated bookmark outline icon"></i>
        {#if deck.tags?.includes("AI")} 
        <i class="right floated magic icon" title="This Deck uses AI generated content"></i>
        {/if}
        <a class="header" href="/{deck.creatorName}/{deck.encodedName}">{deck.name}</a>
        <div class="meta">by <a href="/{deck.creatorName}">{deck.creatorName}</a> - {deck.cardCount} Cards</div>
        <div class="description">{deck.description}</div>
    </div>
    {#if deck.statistics}
    <div class="ui bottom attached multiple progress" data-percent="{percentGood},{percentAgain},{percentNotSeen}">
        <div class="green bar" style="width: {percentGood}%;"></div>
        <div class="red bar" style="width: {percentAgain}%;"></div>
        <div class="grey bar" style="width: {percentNotSeen}%;"></div>
    </div>
    {/if}
</div>

<script lang="ts">
    import type { Deck } from "$lib/api/schema";

    let { deck }: {
        deck: Deck
    } = $props();

    let percentNotSeen = $derived(deck.statistics ? (deck.statistics.notSeen / deck.cardCount) * 100 : 0);
    let percentAgain = $derived(deck.statistics ? (deck.statistics.incorrect / deck.cardCount) * 100 : 0);
    let percentGood = $derived(deck.statistics ? (deck.statistics.correct / deck.cardCount) * 100 : 0);
</script>

<style>
    .description {
        display: -webkit-box;
        line-clamp: 2;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;  
        overflow: hidden;
        height: 3em;
    }
</style>