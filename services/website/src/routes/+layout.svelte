<nav class="ui fluid borderless menu">
    <a class="item" href="/">
        <i class="compass outline icon"></i>
        Explore
    </a>
    {#if data.user}
    <a class="item" href="/decks?s=saved">
        <i class="bookmark outline icon"></i>
        Saved Decks
    </a>
    {/if}

    <div class="right menu">
        {#if data.user}
        <a class="item" href="/{data.user.username}">
            <i class="user circle outline icon"></i>
            Me
        </a>
        <!--
        <a class="item" href="/auth/logout">
            <i class="sign out icon"></i>
            Logout
        </a>
        -->
        {:else}
        <a class="item" href="/auth/login">
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