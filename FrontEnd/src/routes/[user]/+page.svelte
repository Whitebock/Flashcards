<h1 class="ui header">User Info</h1>

<div class="ui segment">
    <h3 class="ui header">
        <div class="content">Decks</div>
    </h3>
    <div class="ui four cards">
        {#each data.decks as deck}
            <Deck deck={deck} editable/>
        {/each}
    </div>
    <br>
    <a class="ui button" href="decks/create">Add new deck</a>
</div>

<script lang="ts">
    import Deck from '$lib/Deck.svelte';

    let { data } = $props();
</script>