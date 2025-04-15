<div class="ui small card">
    <div class="content">
        <div class="header">{deck.name}</div>
        <div class="meta">by System User - {deck.stats.total} Cards</div>
        <div class="description">{deck.description}</div>
    </div>
    {#if editable}
    <div class="ui two buttons">
        <a class="ui button" href="/user/{deck.friendlyId}/edit">
            <i class="edit icon"></i>
            Edit
        </a>
        <a class="ui button" href="/user/{deck.friendlyId}/study">
            <i class="graduation cap icon"></i>
            Study
        </a>
    </div>
    {:else}
    <a class="ui button" href="/user/{deck.friendlyId}/study">
        <i class="graduation cap icon"></i>
        Study
    </a>
    {/if}
    
    <div class="ui bottom attached multiple progress" data-percent="{percentGood},{percentAgain},{percentNotSeen}">
        <div class="green bar" style="width: {percentGood}%;"></div>
        <div class="red bar" style="width: {percentAgain}%;"></div>
        <div class="grey bar" style="width: {percentNotSeen}%;"></div>
    </div>
</div>

<script lang="ts">
    import type { Deck } from "./types";
    let { deck, editable = false }: {
        deck: Deck,
        editable?: boolean
    } = $props();

    let percentNotSeen = $derived((deck.stats.notSeen / deck.stats.total) * 100);
    let percentAgain = $derived((deck.stats.incorrect / deck.stats.total) * 100);
    let percentGood = $derived((deck.stats.correct / deck.stats.total) * 100);
</script>