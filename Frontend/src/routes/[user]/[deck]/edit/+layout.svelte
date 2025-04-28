<h2 class="ui header">Editing Deck "{data.deck.name}"</h2>
<div class="ui top attached tabular menu">
    <a class="item {tab == "deck" ? "active" : ""}" href="/{page.params.user}/{data.deck.encodedName}/edit">Deck Info</a>
    <a class="item {tab == "cards" ? "active" : ""}" href="/{page.params.user}/{data.deck.encodedName}/edit/cards">Cards</a>
</div>

<div class="ui bottom attached active tab segment">
    {@render children()}
</div>

<script lang="ts">
	import { page } from '$app/state';
	import type { LayoutProps } from './$types';

    let { children, data }: LayoutProps  = $props();
    let tab = $derived(page.url.pathname.endsWith("cards") ? "cards" : "deck");
</script>