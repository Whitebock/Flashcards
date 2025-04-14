<nav class="ui fluid menu">
    <a class="item" href="/">Home</a>
    <a class="item" href="/explore">Explore</a>
    <a class="item" href="/user/decks">My Decks</a>

    <div class="right menu">
        <div class="item">
            <div class="ui icon input">
            <input type="text" placeholder="Search...">
            <i class="search link icon"></i>
            </div>
        </div>
        <a class="ui item" href="/login">
            <i class="sign in icon"></i>
            Login
        </a>
    </div>
</nav>

<div class="ui container">
{@render children()}
</div>

<script>
    import "fomantic-ui-css/semantic.min.css";
    let { children } = $props();
</script>