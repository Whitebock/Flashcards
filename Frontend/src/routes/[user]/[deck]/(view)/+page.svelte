<svelte:head>
	<title>Deck | {data.deck.name}</title>
</svelte:head>
<h2 class="ui header">
    <div class="content">
        {data.deck.name}
        <div class="sub header">by {data.deck.creatorName}</div>
    </div>
</h2>
<p>{data.deck.description}</p>
<QuestionList cards={data.cards}/>
{#if data.user}
<a class="ui button" href="{data.deck.encodedName}/study">
    <i class="graduation cap icon"></i>
    Study
</a>

{#if data.user.id == data.deck.creatorId}
<a class="ui button" href="{data.deck.encodedName}/edit">
    <i class="edit icon"></i>
    Edit
</a>
{/if}

{:else}
<a class="ui button" href="/auth/login">Login to Study</a>
{/if}
<script>
    import QuestionList from "$lib/components/QuestionList.svelte";

    let { data } = $props();
</script>