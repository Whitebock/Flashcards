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
<div class="ui basic segment">
    {#if search == "saved"}
    Showing saved Decks
    {:else}
    Showing all Decks
    {/if}
</div>

<script>
    import { page } from '$app/state';
    let search = $derived(page.url.searchParams.get("s"))
</script>