<div class="ui small card">
    <div class="content">
        <i class="right floated bookmark outline icon"></i>
        {#if false}<!--TODO: Check for AI tag--> 
        <i class="right floated magic icon" title="This Deck uses AI generated content"></i>
        {/if}
        <a class="header" href="/{deck.creatorName}/{deck.encodedName}">{deck.name}</a>
        <div class="meta">by <a href="/{deck.creatorName}">{deck.creatorName}</a> - {deck.statistics?.total ?? 0} Cards</div>
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
    import type { Deck } from "./api/schema";

    let { deck, editable = false }: {
        deck: Deck,
        editable?: boolean
    } = $props();

    let percentNotSeen = $derived(deck.statistics ? (deck.statistics.notSeen / deck.statistics.total) * 100 : 0);
    let percentAgain = $derived(deck.statistics ? (deck.statistics.incorrect / deck.statistics.total) * 100 : 0);
    let percentGood = $derived(deck.statistics ? (deck.statistics.correct / deck.statistics.total) * 100 : 0);
</script>