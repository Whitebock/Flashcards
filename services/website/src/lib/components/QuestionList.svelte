<table class="ui {editable ? 'selectable' : ''} scrolling table">
    <thead>
        <tr>
            <th>Front</th>
            <th>Back</th>
        </tr>
    </thead>
    <tbody>
        {#each cards as card}
        <tr onclick={() => selected = card} role="button" class={{active: editable && card.id == selected?.id}}>
            <td>{card.front}</td>
            <td>{card.back}</td>
        </tr>
        {/each}
    </tbody>
    <tfoot>
        <tr>
            <th>{cards.length} Cards</th>
            <th>
                {#if editable}
                <button class="ui right floated button" onclick={() => selected = emptyCard}>
                    <i class="icon plus"></i>
                    Add Card
                </button>
                {/if}
            </th>
        </tr>
    </tfoot>
</table>

<script lang="ts">
    import type { Card } from '$lib/api/schema';

    let { cards, editable = false, selected = $bindable() }: {
        cards: Card[],
        editable?: boolean,
        selected?: Card|null
    } = $props();

    const emptyCard: Card = {front: "", back: "", id: ""}
    if(cards.length > 0) {
        selected = cards[0]!;
    } else {
        selected = emptyCard;
    }
    
</script>