<svelte:head>
	<title>Edit | {data.deck.name}</title>
</svelte:head>
<div class="ui horizontal equal width basic segments">
    <div class="ui segment">
        <QuestionList cards={data.cards} bind:selected editable />
        <!--
        <div class="ui scrolling basic segment">
            <div class="ui selection list">
                {#each data.cards as card}
                <div class="item" onclick={() => selected = card} role="button">
                    <i class="question middle aligned {selected.id == card.id ? "blue" : ""} icon"></i>
                    <div class="content">
                        <div class="header"><span class="ui {selected.id == card.id ? "blue" : ""} text">{card.front}</span></div>
                        <div class="description">{card.back}</div>
                    </div>
                </div>
                {/each}
            </div>
        </div>
        <button class="fluid ui button" onclick={() => selected = emptyCard}>Add Card</button>
        -->
    </div>
    <div class="ui segment">
        <h3 class="ui dividing header">Details</h3>
        <form class="ui form" method="POST" use:enhance>
            <div class="field">
                <label for="front">Front</label>
                <textarea rows="2" id="front" name="front" bind:value={selected.front}></textarea>
            </div>
            <div class="field">
                <label for="back">Back</label>
                <textarea rows="2" id="back" name="back" bind:value={selected.back}></textarea>
            </div>
            
            {#if selected.id == ""}
            <button class="ui right floated primary button" formaction="?/add">Add Card</button>
            <input type="hidden" name="deckId" value={data.deck.id}/>
            {:else}
            <div class="ui right floated buttons">
                <button class="ui button" formaction="?/remove">Remove</button>
                <button class="ui primary button" formaction="?/update">Save</button>
            </div>
            <input type="hidden" name="cardId" value={selected.id}/>
            {/if}
        </form>
        
    </div>
</div>

<script lang="ts">
    import { enhance } from '$app/forms';
    import type { Card } from '$lib/api/schema';
    import QuestionList from "$lib/components/QuestionList.svelte";

    let { data } = $props();
    let selected = $state<Card>()!;

    const emptyCard: Card = {front: "", back: "", id: ""}
    if(data.cards.length > 0) {
        selected = data.cards[0]!;
    } else {
        selected = emptyCard;
    }
    
</script>