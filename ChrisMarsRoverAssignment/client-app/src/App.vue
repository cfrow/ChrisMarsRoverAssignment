<template>
  <div id="app">
    <MarsRover />
  </div>
</template>

<script>
import MarsRover from './components/MarsRover.vue';

export default {
  name: 'App',
  components: {
    MarsRover
  }
}
</script>

<style>
:root {
    --blue: #1a5dad;
    --white: #FFFFFF;
    --text: #4A4A4A;
}

#app {
  font-family: Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: var(--text);
  margin-top: 60px;
  background: #f5f5f5;
}
</style>
