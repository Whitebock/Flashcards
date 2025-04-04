<div class="ui cards">
    {#each data.stacks as stack}
        <div class="ui card">
            <div class="content">
                <div class="header">{stack.name}</div>
                <div class="meta">{stack.cardCount} Cards</div>
            </div>
        </div>
    {/each}
</div>

<script>
    let { data } = $props();
</script>