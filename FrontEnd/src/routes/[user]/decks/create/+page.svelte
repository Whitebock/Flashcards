<div class="ui three steps">
    <div class="active step">
      <i class="folder open outline icon"></i>
      <div class="content">
        <div class="title">Deck Setup</div>
        <div class="description">Provide a name and description for your deck</div>
      </div>
    </div>
    <div class="step">
      <i class="edit outline icon"></i>
      <div class="content">
        <div class="title">Add Questions</div>
        <div class="description">Create and organize questions for your deck</div>
      </div>
    </div>
    <div class="step">
      <i class="book reader icon"></i>
      <div class="content">
        <div class="title">Review & Study</div>
        <div class="description">Practice and review your flashcards</div>
      </div>
    </div>
</div>

<div class="ui segment">
  <form class="ui form" method="POST" use:enhance>
    <div class="field">
        <label for="name">Deck Name</label>
        <input type="text" id="name" name="name" placeholder="Name" bind:value={name}>
    </div>
    <div class="field">
        <label for="description">Description</label>
        <textarea rows="2" id="description" name="description" placeholder="Description"></textarea>
    </div>
    <div class="disabled field">
        <label for="url">URL</label>
        <input type="text" id="url" value={"/user/" + name.toLowerCase().replaceAll(" ", "_")}>
    </div>
    <button class="ui primary button" formaction="?/deck">Next</button>
</form>


</div>

<script lang="ts">
  import { enhance } from '$app/forms';

  let name = $state("");

</script>