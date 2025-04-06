<div class="ui container">
    <nav class="ui centered fluid menu">
        <a class="item" href="/">Home</a>
    </nav>

    <div class="ui basic segment">
    {@render children()}
    </div>
</div>

<script>
    import "fomantic-ui-css/semantic.min.css";
    let { children } = $props();
</script>