<div class="ui secondary menu">
    <div class="item">
        <div class="ui icon input">
          <input type="search" placeholder="Search...">
          <i class="search link icon"></i>
        </div>
    </div>
    <div class="right menu">
        <div class="item">
            <i class="filter icon"></i>
        </div>
    </div>
</div>
{#if data.decks.length == 0}
<div class="ui placeholder segment">
    <div class="ui icon header">
        <i class="folder open outline icon"></i>
        No decks have been created yet
    </div>
</div>
{:else}
<div class="ui four cards">
    {#each data.decks as deck}
    <Deck deck={deck}/>
    {/each}
</div>
{/if}

<script>
    import Deck from '$lib/components/Deck.svelte';
    import { page } from '$app/state';

    let { data } = $props();
    let search = $derived(page.url.searchParams.get("s"))
</script>