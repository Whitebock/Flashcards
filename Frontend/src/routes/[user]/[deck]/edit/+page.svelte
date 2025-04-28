<form class="ui form" method="POST" use:enhance={() => async ({ update }) => {
    await update({reset: false});
}}>
    <div class="field">
        <label for="name">Deck Name</label>
        <input type="text" id="name" name="name" placeholder="Name" bind:value={deck.name}>
    </div>
    <div class="field">
        <label for="description">Description</label>
        <textarea rows="2" id="description" name="description" placeholder="Description" bind:value={deck.description}></textarea>
    </div>
    <div class="disabled field">
        <label for="url">URL</label>
        <input type="text" id="url" value={`/${data.user?.username}/${deck.encodedName}`}>
    </div>
    <button class="ui button" formaction="?/delete">Delete</button>
    <button class="ui primary button" formaction="?/update">Save</button>
</form>

<script lang="ts">
    import { enhance } from '$app/forms';

    let { data } = $props(); 
    let deck = $state(data.deck);
</script>