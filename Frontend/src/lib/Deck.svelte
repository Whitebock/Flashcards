<div class="ui small card">
    <div class="content">
        <div class="header">{deck.name}</div>
        <div class="meta">by <a href="/{deck.creatorName}">{deck.creatorName}</a> - {deck.statistics?.total ?? 0} Cards</div>
        <div class="description">{deck.description}</div>
    </div>
    {#if editable}
    <div class="ui two buttons">
        <a class="ui button" href="/{deck.creatorName}/{deck.encodedName}/edit">
            <i class="edit icon"></i>
            Edit
        </a>
        <a class="ui button" href="/{deck.creatorName}/{deck.encodedName}/study">
            <i class="graduation cap icon"></i>
            Study
        </a>
    </div>
    {:else}
    <a class="ui button" href="/{deck.creatorName}/{deck.encodedName}/study">
        <i class="graduation cap icon"></i>
        Study
    </a>
    {/if}
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