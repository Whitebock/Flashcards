<svelte:head>
	<title>Profile | {data.profile.username}</title>
</svelte:head>
<h2 class="ui header">
    {#if data.profile.picture}
    <img src={data.profile.picture} alt="Profile" class="ui circular image">
    {:else}
    <i class="user circle icon"></i>
    {/if}
    <div class="content">
        {data.profile.username}
        <div class="sub header">{data.profile?.id}</div>
    </div>
</h2>

<h3 class="ui top attached header">
    <div class="content">
        Decks
    </div>
</h3>
{#if data.decks && data.decks.length > 0}
<div class="ui attached segment">
    <div class="ui four cards">
        {#each data.decks as deck}
        <Deck deck={deck} editable={data.profile.id == data.user?.id}/>
        {/each}
    </div>
</div>
{:else}
<div class="ui attached placeholder segment">
    <div class="ui icon header">
        <i class="folder open outline icon"></i>
        This user has not created any decks yet.
    </div>
</div>
{/if}
{#if data.profile.id == data.user?.id}
<div class="ui bottom attached compact segment">
    <a class="ui button" href="decks/create">Add new deck</a>
</div>
{/if}

<script lang="ts">
    import Deck from '$lib/Deck.svelte';

    let { data } = $props();
</script>