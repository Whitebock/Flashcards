<script lang="ts">
    import type { Card } from '$lib/types';

    let { data } = $props();
    let selected = $state<Card>();

    function addQuestion() {
        // TODO: Implement add logic
    }

    function deleteQuestion(card: Card) {
        // TODO: Implement delete logic
    }
</script>

<div class="ui container">
    <h2 class="ui header">Edit Deck</h2>
    <div class="ui grid">
        <div class="eight wide column">
            <h3 class="ui dividing header">Cards</h3>
            <div class="ui divided selection list">
                {#each data.cards as card}
                    <div class="item">
                        <i class="question circle middle aligned icon"></i>
                        <div class="content" onclick={() => selected = card}>
                            <a class="header">{card.front}</a>
                            <div class="description">{card.back}</div>
                        </div>
                    </div>
                {/each}
                
                <button class="fluid ui button" onclick={addQuestion}>Add Question</button>
            </div>
        </div>
        <div class="eight wide column">
            <h3 class="ui dividing header">Details</h3>
            {#if selected}
                <div class="ui form">
                    <div class="field">
                        <label>Front</label>
                        <textarea bind:value={selected.front}></textarea>
                    </div>
                    <div class="field">
                        <label>Back</label>
                        <textarea bind:value={selected.back}></textarea>
                    </div>
                    <button class="ui primary button">Save</button>
                    <button class="ui red button" onclick={() => selected && deleteQuestion(selected)}>Delete</button>
                </div>
            {:else}
                <p>Select a question to view and edit its details.</p>
            {/if}
        </div>
    </div>
</div>
