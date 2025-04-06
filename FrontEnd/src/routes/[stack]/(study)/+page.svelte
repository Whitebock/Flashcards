<script lang="ts">
    let { data } = $props();

    let currentCardIndex = $state(0);
    let showAnswer = $state(false);

    function nextCard(result: "again"|"good"|"easy") {
        showAnswer = false;
        currentCardIndex = (currentCardIndex + 1) % data.cards.length;
    }
</script>

<div class="ui container flashcard-container">
    <div class="ui small progress" data-percent="10">
        <div class="grey bar" style="width: 10%;"></div>
        <div class="label">7 Cards left</div>
    </div>
    <div class="ui fluid massive centered raised flash card">
        <div class="content">
            {data.cards[currentCardIndex].question}
            <div class="ui horizontal divider">
                <i class="question icon"></i>
            </div>
            {#if showAnswer}
            {data.cards[currentCardIndex].answer}
            {:else}
            &nbsp;
            {/if}
        </div>
        <button class="ui bottom attached button" onclick={() => showAnswer = !showAnswer}>
            {showAnswer ? "Hide Answer" : "Show Answer"}
        </button>
    </div>
    <div class="ui three buttons">
        <button class="ui button" onclick={() => nextCard("again")}>Again</button>
        <button class="ui button" onclick={() => nextCard("good")}>Good</button>
        <button class="ui button" onclick={() => nextCard("easy")}>Easy</button>
    </div>
    
    
</div>

<style>
    .flash.card {
        margin-top: 10vh;
        margin-bottom: 10vh;
        width: 400px;
    }
</style>