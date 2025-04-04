<nav class="ui menu">
    <a class="item" href="/">Dash</a>
    <a class="item" href="/stacks">Cardstacks</a>
    <div class="right menu">
        <div class="item">Account</div>
    </div>
</nav>

<div class="ui container">
    {@render children()}
</div>

<script>
    import "fomantic-ui-css/semantic.min.css";
    let { children } = $props();
</script>