<svelte:head>
	<title>Flashcard Studying</title>
</svelte:head>
<h1 class="ui header">Flashcard Studying Website</h1>
<p>Here you can improve your knowledge by studying with flashcards. Below are some example card decks with their current statistics:</p>

<div class="ui cards">
    {#each data.decks as deck}
        {@const rightPercent = (deck.stats.correct / deck.stats.total) * 100}
        {@const wrongPercent = (deck.stats.incorrect / deck.stats.total) * 100}
        {@const unansweredPercent = (deck.stats.notSeen / deck.stats.total) * 100}
        <div class="ui card">
            <div class="content">
                <div class="header">{deck.name}</div>
                <div class="meta">{deck.stats.total} Cards</div>
            </div>
            <div class="ui two buttons">
                <a class="ui button" href="{deck.id}/edit">
                    <i class="edit icon"></i>
                    Edit
                </a>
                <a class="ui button" href="{deck.id}">
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
    let { data } = $props();
</script>