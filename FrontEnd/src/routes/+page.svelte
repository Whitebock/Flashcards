<svelte:head>
	<title>Flashcard Studying</title>
</svelte:head>
<h1 class="ui header">Flashcard Studying Website</h1>
<p>Here you can improve your knowledge by studying with flashcards. Below are some example card decks with their current statistics:</p>

<div class="ui cards">
    {#each data.stacks as stack}
        {@const rightPercent = (stack.cards.filter(c => c.status == CardStatus.Correct).length / stack.cards.length) * 100}
        {@const wrongPercent = (stack.cards.filter(c => c.status == CardStatus.Incorrect).length / stack.cards.length) * 100}
        {@const unansweredPercent = (stack.cards.filter(c => c.status == CardStatus.NotSeen).length / stack.cards.length) * 100}
        <div class="ui card">
            <div class="content">
                <div class="header">{stack.name}</div>
                <div class="meta">{stack.cards.length} Cards</div>
            </div>
            <div class="ui two buttons">
                <a class="ui button" href="{stack.id}/edit">
                    <i class="edit icon"></i>
                    Edit
                </a>
                <a class="ui button" href="{stack.id}">
                    <i class="graduation cap icon"></i>
                    Study
                </a>
              </div>
            
            <div class="ui bottom attached multiple progress" data-percent="{rightPercent},{wrongPercent},{unansweredPercent}">
                <div class="green bar" style="width: {rightPercent}%;"></div>
                <div class="red bar" style="width: {wrongPercent}%;"></div>
                <div class="grey bar" style="width: {unansweredPercent}%;"></div>
            </div>
        </div>
    {/each}
</div>

<script lang="ts">
    import { CardStatus } from '$lib/types/index.js';

    let { data } = $props();
</script>