<svelte:head>
	<title>Flashcard Studying</title>
</svelte:head>
<div class="ui container flashcard-container">
    <div class="ui small progress" data-percent={100 - (data.left / data.deck.statistics!.total) * 100}>
        <div class="grey bar" style="width: {100 - (data.left / data.deck.statistics!.total) * 100}%;"></div>
        <div class="label">{data.left} Cards left</div>
    </div>
    <form method="POST" data-sveltekit-replacestate use:enhance>
        <div class="ui massive centered raised flash card">
            <div class="content">
                {data.card.front}
                <div class="ui horizontal divider">
                    <i class="question icon"></i>
                </div>
                {#if showAnswer}
                {data.card.back}
                {:else}
                &nbsp;
                {/if}
            </div>
            <button class="ui bottom attached button" name="answer" value={showAnswer ? "hide" : "show"}>
                {showAnswer ? "Hide Answer" : "Show Answer"}
            </button>
        </div>
        <div class="ui three buttons">
            <button class="ui button" name="choice" value="Again">Again</button>
            <button class="ui button" name="choice" value="Good">Good</button>
            <button class="ui button" name="choice" value="Easy">Easy</button>
        </div>
        <input type="hidden" name="cardId" value={data.card.id} />
    </form>
</div>

<script lang="ts">
    import { enhance } from '$app/forms';
    let { data, form } = $props();
    
    let showAnswer = $derived(form?.showAnswer == true);
</script>

<style>
    .flash.card {
        width: 400px;
    }
</style>