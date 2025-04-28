<nav class="ui fluid menu">
    <a class="item" href="/">Home</a>
    <a class="item" href="/explore">Explore</a>
    {#if data.user}
    <a class="item" href="/{data.user.username}">My Decks</a>
    {/if}

    <div class="right menu">
        <div class="item">
            <div class="ui icon input">
            <input type="text" placeholder="Search...">
            <i class="search link icon"></i>
            </div>
        </div>
        {#if data.user}
        <div class="item">{data.user?.username}</div>
        <a class="ui item" href="/auth/logout">
            <i class="sign out icon"></i>
            Logout
        </a>
        {:else}
        <a class="ui item" href="/auth/login">
            <i class="sign in icon"></i>
            Login
        </a>
        {/if}
    </div>
</nav>

<div class="ui container">
{@render children()}
</div>

<script>
    import "fomantic-ui-css/semantic.min.css";
    let { children, data } = $props();
</script>