<svelte:head>
	<title>Flashcard Studying</title>
</svelte:head>
<div class="ui hidden warning message">
    <div class="header">
        <i class="exclamation triangle icon"></i>
        Login or Register to Save Your Progress!
    </div>
    <p>
        Unlock the full potential of Flashcards by creating your own decks, tracking your progress, and competing with friends.
    </p>
</div>

<div class="ui segment">
    Check out some popular and upcoming decks to enhance your learning experience.
    Dive into topics that interest you and master them at your own pace.
    Want to make studying more fun? Collaborate with your friends and challenge each other to learn faster and better!
</div>

{#if data.recent.length == 0}
<div class="ui placeholder segment">
    <div class="ui icon header">
        <i class="folder open outline icon"></i>
        No decks have been created yet.
    </div>
</div>
{:else}
<div class="ui segment">
    <h3 class="ui header">
        <i class="chartline icon"></i>
        <div class="content">Popular Decks</div>
    </h3>
    <div class="ui four cards">
        {#each data.popular! as deck}
            <Deck deck={deck}/>
        {/each}
    </div>
</div>

<div class="ui segment">
    <h3 class="ui header">
        <i class="rss icon"></i>
        <div class="content">New Decks</div>
    </h3>
    <div class="ui four cards">
        {#each data.recent! as deck}
            <Deck deck={deck}/>
        {/each}
    </div>
</div>
{/if}

<script lang="ts">
    import Deck from '$lib/Deck.svelte';

    let { data } = $props();
</script>