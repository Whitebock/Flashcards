<svelte:head>
	<title>Flashcards | Explore</title>
</svelte:head>
<div class="ui hidden message">
    <i class="close icon"></i>
    <div class="header">
        Ready to Level Up Your Learning?
    </div>
    <p>
        Discover trending and upcoming decks designed to make studying more engaging and effective.
        Explore topics you're passionate about, learn at your own pace, and make it stick.
        Want to make it even more fun? Team up with friends and turn learning into a challenge!
    </p>
</div>

<div class="ui stackable two column grid">
    <div class="ten wide column">
            <h3 class="ui header">Popular Decks</h3>
            {#if data.popular.length == 0}
            <div class="ui placeholder segment">
                <div class="ui icon header">
                    <i class="chartline icon"></i>
                    No decks have been created yet
                </div>
            </div>
        {:else}
        <div class="ui two cards">
            {#each data.popular.slice(0, 4) as deck}
                <Deck deck={deck}/>
            {/each}
        </div>
        {/if}

        <h3 class="ui header">New Decks</h3>
        {#if data.recent.length == 0}
        <div class="ui placeholder segment">
            <div class="ui icon header">
                <i class="folder open outline icon"></i>
                No decks have been created yet
            </div>
        </div>
        {:else}
        <div class="ui two cards">
            {#each data.recent.slice(0, 4) as deck}
            <Deck deck={deck}/>
            {/each}
        </div>
        {/if}
    </div>
    <div class="six wide column">
        <h3 class="ui header">
            <div class="content">Latest Activities</div>
        </h3>
        <div class="ui placeholder segment">
            <div class="ui icon header">
                <i class="feed icon"></i>
                No updates to show
            </div>
        </div>
    </div>
</div>


<script lang="ts">
    import Deck from '$lib/components/Deck.svelte';

    let { data } = $props();
</script>